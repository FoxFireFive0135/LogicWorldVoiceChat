using EccsLogicWorldAPI.Client.Hooks;
using EccsLogicWorldAPI.Client.Injectors;
using FancyInput;
using FoxFireFive_LogicWorldVoiceChat.Shared;
using LogicAPI.Client;
using LogicLog;
using LogicSettings;
using LogicWorld;
using LogicWorld.Interfaces;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace FoxFireFive_LogicWorldVoiceChat.Client
{
    public class LogicWorldVoiceChat : ClientMod
    {
        public static ILogicLogger logger;
        public static Dictionary<string, LWVC_VoicePlayer> players = new Dictionary<string, LWVC_VoicePlayer>();
        public static GameObject player;
        public static bool push_to_talk_state = false;

        protected override void Initialize()
        {
            RawPacketHandlerInjector.addPacketHandler(new LWVC_ClientDataHandler());

            logger = Logger;
            WorldHook.worldLoading += () =>
            {
                try
                {
                    LWVC_Setup();
                }
                catch (Exception e)
                {
                    Logger.Error("LWVC_Setup() failed!!");
                    SceneAndNetworkManager.TriggerErrorScreen(e);
                }
            };

            CustomInput.Register<LWVC_PTT_Context, LWVC_PTT_Trigger>("LogicWorldVoiceChat");
        }

        [Setting_Toggle("FoxFireFive.LogicWorldVoiceChat.VoiceEnabled")]
        public static bool voice_enabled { get; set; } = false;

        [Setting_Toggle("FoxFireFive.LogicWorldVoiceChat.MuteEveryone")]
        public static bool mute_everyone { get; set; } = false;

        [Setting_Toggle("FoxFireFive.LogicWorldVoiceChat.PushToTalk")]
        public static bool push_to_talk { get; set; } = false;

        [Setting_SliderInt("FoxFireFive.LogicWorldVoiceChat.OthersVoiceVolume")]
        public static int others_voice_volume { get; set; } = 80;

        [Setting_DropdownDynamic("FoxFireFive.LogicWorldVoiceChat.VoiceInputDevice", typeof(VoiceDevicesProvider), nameof(VoiceDevicesProvider.GetDevices))]
        public static string voice_input_device { get; set; } = "";

        public static class VoiceDevicesProvider
        {
            public static List<string> GetDevices()
            {
                var devices = new List<string>();

                if (Microphone.devices == null || Microphone.devices.Length == 0)
                {
                    devices.Add("No devices Found");
                    return devices;
                }

                for (int i = 0; i < Microphone.devices.Length; i++)
                {
                    devices.Add(Microphone.devices[i]);
                }

                return devices;
            }
        }

        public static void TogglePushToTalk()
        {
            if (!push_to_talk) return;
            if (CustomInput.Held(LWVC_PTT_Trigger.PushToTalk))
            {
                push_to_talk_state = true;
                LWVC_GUI.muted_icon.SetActive(false);
                LWVC_GUI.unmuted_icon.SetActive(true);
            }
            else
            {
                push_to_talk_state = false;
                LWVC_GUI.muted_icon.SetActive(true);
                LWVC_GUI.unmuted_icon.SetActive(false);
            }
        }

        public void LWVC_Setup()
        {
            GameObject microphone_obj = new GameObject("LWVC_Microphone");
            var mic = microphone_obj.AddComponent<LWVC_Microphone>();

            player = GameObject.Find("Player Model: " + LogicWorld.Networking.CurrentPlayer.Username);

            LWVC_GUI.Setup();

            mic.OnAudioFrame += pcm =>
            {
                if (player == null)
                {
                    player = GameObject.Find("Player Model: " + LogicWorld.Networking.CurrentPlayer.Username);
                }

                if (voice_enabled)
                {
                    LWVC_GUI.hidden = false;
                    if (push_to_talk)
                    {
                        if (push_to_talk_state)
                        {
                            Instances.SendData.Send(new LWVC_MicrophoneData()
                            {
                                sender = LogicWorld.Networking.CurrentPlayer.Username,
                                world_pos = player != null ? player.transform.position : Vector3.zero,
                                mic_data = pcm
                            });
                        }
                    }
                    else
                    {
                        LWVC_GUI.unmuted_icon.SetActive(true);
                        Instances.SendData.Send(new LWVC_MicrophoneData()
                        {
                            sender = LogicWorld.Networking.CurrentPlayer.Username,
                            world_pos = player != null ? player.transform.position : Vector3.zero,
                            mic_data = pcm
                        });
                    }
                }
                else
                {
                    LWVC_GUI.HideIcons();
                }
            };
        }
    }
}

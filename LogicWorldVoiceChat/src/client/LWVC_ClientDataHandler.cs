using FoxFireFive_LogicWorldVoiceChat.Shared;
using LogicWorld.SharedCode.Networking;
using UnityEngine;

namespace FoxFireFive_LogicWorldVoiceChat.Client
{
    public class LWVC_ClientDataHandler : PacketHandler<LWVC_MicrophoneData>
    {
        public override void Handle(LWVC_MicrophoneData packet, HandlerContext context)
        {
            LogicWorldVoiceChat.players.TryGetValue(packet.sender, out LWVC_VoicePlayer aud);

            if (aud == null)
            {
                GameObject go = new GameObject("LWVC_Voice - " + packet.sender);

                AudioSource aud_src = go.AddComponent<AudioSource>();
                aud_src.loop = true;
                aud_src.playOnAwake = false;
                aud_src.volume = LogicWorldVoiceChat.others_voice_volume / 100f;
                aud_src.spatialBlend = 1f;
                aud_src.rolloffMode = AudioRolloffMode.Linear;
                aud_src.minDistance = 1f;
                aud_src.maxDistance = 35f;
                aud_src.dopplerLevel = 0f;

                LWVC_VoicePlayer voicePlayer = go.AddComponent<LWVC_VoicePlayer>();

                voicePlayer.Init(aud_src);

                aud = voicePlayer;
                LogicWorldVoiceChat.players[packet.sender] = aud;
            }

            aud.transform.position = packet.world_pos;

            aud.source.volume = LogicWorldVoiceChat.others_voice_volume / 100f;

            if (LogicWorldVoiceChat.mute_everyone) return;

            aud.Push(packet.mic_data);
        }
    }
}

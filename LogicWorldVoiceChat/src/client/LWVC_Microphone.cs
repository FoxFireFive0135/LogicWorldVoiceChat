using System;
using System.Linq;
using UnityEngine;

namespace FoxFireFive_LogicWorldVoiceChat.Client
{
    public class LWVC_Microphone : MonoBehaviour
    {
        public int sample_rate = 48000;
        public int frame_size = 960;

        private AudioClip micClip;
        private string device;

        private float[] sample_buffer;
        private short[] pcm_buffer;

        private int last_sample_pos;
        private int mic_clip_length;

        private bool running;

        public Action<short[]> OnAudioFrame;

        public void Start()
        {
            if (Microphone.devices == null || Microphone.devices.Length == 0)
            {
                Debug.LogWarning("[LWVC] No microphone devices available.");
                return;
            }

            device = ResolveDevice();

            micClip = Microphone.Start(device, true, 1, sample_rate);

            if (micClip == null)
            {
                Debug.LogError("[LWVC] Failed to start microphone.");
                return;
            }

            int safety = 0;
            while (Microphone.GetPosition(device) <= 0 && safety < 50)
            {
                safety++;
            }

            sample_buffer = new float[frame_size];
            pcm_buffer = new short[frame_size];

            last_sample_pos = 0;
            mic_clip_length = micClip.samples;

            running = true;
        }

        public void Update()
        {
            if (!running || micClip == null) return;

            LogicWorldVoiceChat.TogglePushToTalk();

            int pos = Microphone.GetPosition(device);
            if (pos < 0) return;

            int diff = pos - last_sample_pos;
            if (diff < 0) diff += mic_clip_length;

            while (diff >= frame_size)
            {
                ProcessFrame();

                last_sample_pos += frame_size;
                if (last_sample_pos >= mic_clip_length)
                    last_sample_pos -= mic_clip_length;

                diff -= frame_size;
            }
        }

        private void ProcessFrame()
        {
            micClip.GetData(sample_buffer, last_sample_pos);

            for (int i = 0; i < frame_size; i++)
            {
                float v = sample_buffer[i];
                if (v > 1f) v = 1f;
                else if (v < -1f) v = -1f;

                pcm_buffer[i] = (short)(v * short.MaxValue);
            }

            OnAudioFrame?.Invoke(pcm_buffer);
        }

        private string ResolveDevice()
        {
            var preferred = LogicWorldVoiceChat.voice_input_device;

            if (!string.IsNullOrEmpty(preferred) &&
                Microphone.devices.Contains(preferred))
            {
                return preferred;
            }

            return Microphone.devices[0];
        }

        private void OnDestroy()
        {
            running = false;

            if (!string.IsNullOrEmpty(device))
            {
                Microphone.End(device);
            }

            micClip = null;
            OnAudioFrame = null;
        }
    }
}
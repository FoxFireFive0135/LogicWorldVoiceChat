using System;
using System.Linq;
using UnityEngine;

namespace FoxFireFive_LogicWorldVoiceChat.Client
{
    public class LWVC_Microphone : MonoBehaviour
    {
        public int sampleRate = 48000;
        public int frameSize = 960;

        private AudioClip micClip;
        private string device;

        private float[] sampleBuffer;

        private int lastSamplePos;
        private int micClipLength;

        public Action<short[]> OnAudioFrame;

        public void Start()
        {
            if (Microphone.devices.Length == 0)
                return;

            device = LogicWorldVoiceChat.voice_input_device;

            if (string.IsNullOrEmpty(device) ||
                !Microphone.devices.Contains(device))
            {
                device = Microphone.devices[0];
            }

            micClip = Microphone.Start(device, true, 1, sampleRate);

            sampleBuffer = new float[frameSize];
            lastSamplePos = 0;
            micClipLength = micClip.samples;
        }

        public void Update()
        {
            if (micClip == null) return;

            LogicWorldVoiceChat.TogglePushToTalk();

            int pos = Microphone.GetPosition(device);
            if (pos < 0) return;

            int diff = pos - lastSamplePos;
            if (diff < 0)
                diff += micClipLength;

            if (diff < frameSize)
                return;

            micClip.GetData(sampleBuffer, lastSamplePos);

            short[] pcm = new short[frameSize];

            for (int i = 0; i < frameSize; i++)
            {
                pcm[i] =
                    (short)(Mathf.Clamp(sampleBuffer[i], -1f, 1f) * short.MaxValue);
            }

            lastSamplePos += frameSize;

            if (lastSamplePos >= micClipLength)
                lastSamplePos -= micClipLength;

            OnAudioFrame?.Invoke(pcm);
        }

        private void OnDestroy()
        {
            if (!string.IsNullOrEmpty(device))
                Microphone.End(device);
        }
    }
}
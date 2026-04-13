using UnityEngine;

namespace FoxFireFive_LogicWorldVoiceChat.Client
{
    public class LWVC_VoicePlayer : MonoBehaviour
    {
        public AudioSource source;

        private const int SampleRate = 48000;
        private const int BufferSeconds = 2;

        private float[] buffer;
        private int buffer_size;
        private int write_pos;
        private int read_pos;
        private float lastSample;

        public void Init(AudioSource src)
        {
            source = src;

            buffer_size = SampleRate * BufferSeconds;
            buffer = new float[buffer_size];

            write_pos = 0;
            read_pos = 0;

            AudioClip clip = AudioClip.Create("LWVC_VoiceStream", SampleRate, 1, SampleRate, true, OnAudioRead);

            source.clip = clip;
            source.loop = true;
            source.Play();
        }

        public void Push(short[] pcm)
        {
            if (buffer == null)
                return;

            for (int i = 0; i < pcm.Length; i++)
            {
                float sample = pcm[i] * (1f / 32768f);

                buffer[write_pos] = sample;

                write_pos = (write_pos + 1) % buffer_size;

                if (write_pos == read_pos)
                {
                    read_pos = (read_pos + 1) % buffer_size;
                }
            }
        }

        private void OnAudioRead(float[] data)
        {
            if (buffer == null)
            {
                for (int i = 0; i < data.Length; i++)
                    data[i] = 0f;
                return;
            }

            for (int i = 0; i < data.Length; i++)
            {
                if (read_pos != write_pos)
                {
                    lastSample = buffer[read_pos];
                    data[i] = lastSample;

                    read_pos = (read_pos + 1) % buffer_size;
                }
                else
                {
                    data[i] = lastSample;
                }
            }
        }

        private void OnDestroy()
        {
            buffer = null;

            if (source != null)
            {
                source.Stop();
                source.clip = null;
            }
        }
    }
}
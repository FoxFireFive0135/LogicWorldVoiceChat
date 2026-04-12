using UnityEngine;
using System.Collections.Concurrent;

namespace FoxFireFive_LogicWorldVoiceChat.Client
{
    public class LWVC_VoicePlayer : MonoBehaviour
    {
        private const int sample_rate = 48000;
        private ConcurrentQueue<float> queue = new ConcurrentQueue<float>();

        public AudioSource source;

        public void Init(AudioSource src)
        {
            source = src;

            AudioClip clip = AudioClip.Create(
                "LWVC_MicrophoneStream",
                sample_rate,
                1,
                sample_rate,
                true,
                OnAudioRead
            );

            source.clip = clip;
            source.loop = true;
            source.Play();
        }

        public void Push(short[] pcm)
        {
            for (int i = 0; i < pcm.Length; i++)
                queue.Enqueue(pcm[i] / 32768f);
        }

        private void OnAudioRead(float[] data)
        {
            for (int i = 0; i < data.Length; i++)
            {
                if (queue.TryDequeue(out float val))
                    data[i] = val;
                else
                    data[i] = 0f;
            }
        }
    }
}
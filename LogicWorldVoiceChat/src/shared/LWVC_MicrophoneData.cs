using UnityEngine;
using LogicAPI.Networking.Packets;
using MessagePack;

namespace FoxFireFive_LogicWorldVoiceChat.Shared
{
    [MessagePackObject]
    public sealed class LWVC_MicrophoneData : Packet
    {
        [Key(0)]
        public string sender;
        [Key(1)]
        public Vector3 world_pos;
        [Key(2)]
        public short[] mic_data;
    }
}

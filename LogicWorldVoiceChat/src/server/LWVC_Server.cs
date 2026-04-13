using EccsLogicWorldAPI.Server;
using EccsLogicWorldAPI.Server.Hooks;
using EccsLogicWorldAPI.Server.Injectors;
using FoxFireFive_LogicWorldVoiceChat.Shared;
using LogicAPI.Data;
using LogicAPI.Networking;
using LogicAPI.Server;
using LogicAPI.Server.Networking;
using System.Collections.Generic;
using UnityEngine;
using static EccsLogicWorldAPI.Server.Hooks.PlayerJoiningHook;

namespace FoxFireFive_LogicWorldVoiceChat.Server
{
    public class LWVC_Server : ServerMod
    {
        private NetworkServer network;
        public Dictionary<string, Connection> usernames = new Dictionary<string, Connection>();
        private List<ServerConnection> names = new List<ServerConnection>();

        protected override void Initialize()
        {
            network = ServiceGetter.getService<NetworkServer>();

            RawPacketHandlerInjector.addPacketHandler(new LWVC_ServerDataHandler(this));

            network.RemoteDisconnected += (ServerConnection con) =>
            {
                usernames.Clear();
            };
        }

        public void SendMicDataToClients(Connection sender, LWVC_MicrophoneData packet)
        {
            usernames.TryGetValue(packet.sender, out Connection username);
            if (username == null)
            {
                usernames.Add(packet.sender, sender);
            }

            foreach (Connection con in usernames.Values)
            {
                if (con == sender) continue;
                network.Send(con, packet);
            }
        }
    }
}
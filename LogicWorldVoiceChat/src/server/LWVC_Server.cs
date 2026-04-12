using EccsLogicWorldAPI.Server;
using EccsLogicWorldAPI.Server.Injectors;
using LogicAPI.Server;
using LogicAPI.Server.Networking;
using LogicAPI.Networking;
using FoxFireFive_LogicWorldVoiceChat.Shared;
using System.Collections.Generic;

namespace FoxFireFive_LogicWorldVoiceChat.Server
{
    public class LWVC_Server : ServerMod
    {
        private NetworkServer network;
        public Dictionary<string, Connection> usernames = new Dictionary<string, Connection>();

        protected override void Initialize()
        {
            network = ServiceGetter.getService<NetworkServer>();

            RawPacketHandlerInjector.addPacketHandler(new LWVC_ServerDataHandler(this));
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
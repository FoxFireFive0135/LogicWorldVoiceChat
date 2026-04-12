using FoxFireFive_LogicWorldVoiceChat.Shared;
using LogicWorld.SharedCode.Networking;

namespace FoxFireFive_LogicWorldVoiceChat.Server
{
    public class LWVC_ServerDataHandler : PacketHandler<LWVC_MicrophoneData>
    {
        private readonly LWVC_Server server;

        public LWVC_ServerDataHandler(LWVC_Server server_)
        {
            server = server_;
        }

        public override void Handle(LWVC_MicrophoneData packet, HandlerContext context)
        {
            server.SendMicDataToClients(context.Sender, packet);
        }
    }
}

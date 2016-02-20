using System;

namespace Snowbull.API {
    public interface IContext {
        string Name {
            get;
        }

        void Send(Packets.ISendPacket packet);
    }
}


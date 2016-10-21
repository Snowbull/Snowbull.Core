using System;

namespace Snowbull.Core.Game.Room {
    public interface IRoom : IContext {
        int InternalID {
            get;
        }

        int ExternalID {
            get;
        }

        string Name {
            get;
        }

        IZone Zone {
            get;
        }
    }
}


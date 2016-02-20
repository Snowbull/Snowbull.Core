using System;

namespace Snowbull.API {
    public interface IPlugin {
        string Name {
            get;
        }

        Guid UID {
            get;
        }

        Version Version {
            get;
        }
    }
}


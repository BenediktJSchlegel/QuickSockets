
using System.Collections;

namespace QuickSockets.Communication;

internal class ConnectionList : List<Connection>
{
    internal bool Contains(string ip)
    {
        return this.FirstOrDefault(c => c.IP == ip) != null;
    }

    internal bool Contains(int id)
    {
        return this.FirstOrDefault(c => c.UniqueId == id) != null;
    }

}

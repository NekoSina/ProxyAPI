using System.Diagnostics.Contracts;
using System.Net;

namespace ProxyAPI.IP2Location
{
    public interface IpLocator
    {
        [Pure]
        Location Locate(IPAddress ip);
    }
}
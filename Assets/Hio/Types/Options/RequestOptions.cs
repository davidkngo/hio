using System.Collections.Generic;

namespace HioCom.Options
{
    public class RequestOptions
    {
        public string BaseUrl;
        public Dictionary<string, string> Headers = new();

        public int ConnectionTimeout;
    }
}

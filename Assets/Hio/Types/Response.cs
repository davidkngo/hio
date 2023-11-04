using System.Collections.Generic;

namespace HioCom
{
    public class Response<T>
    {
        public T Data;
        public Dictionary<string, string> Headers;
        public long StatusCode;
        public string ErrorMessage;
    }
}

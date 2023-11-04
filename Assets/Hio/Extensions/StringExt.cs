using System.Collections.Generic;
using System.Net;
using System.Linq;
using System.IO;

namespace HioCom.Extentions
{
    public static class StringExtentions
    {
        public static string ToQueryUrl(
            this string path,
            string basePath,
            Dictionary<string, object> parameters = null
        )
        {
            parameters ??= new Dictionary<string, object>();
            string query = string.Join(
                "&",
                parameters.Select(
                    param =>
                        $"{WebUtility.UrlEncode(param.Key)}={WebUtility.UrlEncode(param.Value.ToString())}"
                )
            );

            var _path = path;
            if (basePath != null)
            {
                _path = Path.Join(basePath, path);
            }

            if (query != "")
            {
                return $"{_path}?{query}";
            }

            return _path;
        }
    }
}

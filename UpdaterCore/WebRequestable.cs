using System;
using System.IO;
using System.Net;

namespace UpdaterCore
{
    public class WebRequestable
    {
        protected string UserAgent { get { return $"Mozilla/5.0 (compatible; MiMU/{this.CoreVersion})"; } }
        protected string CoreVersion { get { return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString(); } }
        protected HttpWebResponse CreateGetRequest(string url, string accept = "*/*")
        {
            HttpWebRequest wreq = HttpWebRequest.CreateHttp(url);
            wreq.Accept = accept;
            wreq.Method = "GET";
            wreq.UserAgent = this.UserAgent;
            wreq.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            wreq.AllowAutoRedirect = true;
            return wreq.GetResponse() as HttpWebResponse;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using UpdaterCore;

namespace MiMU
{
    class DownloadUtils : WebRequestable
    {
        public void DownloadToFile(string url, string filename)
        {
            using (HttpWebResponse wres = this.CreateGetRequest(url))
            using (Stream str = wres.GetResponseStream())
            using (FileStream fstr = new FileStream(filename, FileMode.Create))
            {
                str.CopyTo(fstr);
            }
        }
        public string DownloadAsString(string url)
        {
            using (HttpWebResponse wres = this.CreateGetRequest(url))
            using (Stream str = wres.GetResponseStream())
            using (StreamReader sre = new StreamReader(str))
            {
                return sre.ReadToEnd();
            }
        }
    }
}

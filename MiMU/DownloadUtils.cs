using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using UpdaterCore;
using System.Security.Cryptography;

namespace MiMU
{
    delegate void DownloadProgress(long totalBytes, long downloadedBytes);
    class DownloadUtils : WebRequestable
    {
        public event DownloadProgress DownloadFileProgress;
        public event Action DownloadFileComplete;
        public DownloadUtils()
        {
            DownloadFileProgress += (a, b) => { };
            DownloadFileComplete += () => { };
        }
        public void DownloadToFile(string url, string filename, bool reportProgress = false)
        {
            using (HttpWebResponse wres = this.CreateGetRequest(url))
            using (Stream str = wres.GetResponseStream())
            using (FileStream fstr = new FileStream(filename, FileMode.Create))
            {
                if (reportProgress)
                {
                    // from https://stackoverflow.com/a/1933764
                    int num;
                    long downloaded = 0, contentLength = wres.ContentLength;
                    const int bufferSize = 81920;
                    byte[] buffer = new byte[bufferSize];
                    DownloadFileProgress(contentLength, downloaded);
                    while ((num = str.Read(buffer, 0, buffer.Length)) != 0)
                    {
                        fstr.Write(buffer, 0, num);
                        downloaded += num;
                        DownloadFileProgress(contentLength, downloaded);
                    }
                    DownloadFileComplete();
                }
                else
                {
                    str.CopyTo(fstr);
                }
            }
        }
        public bool checkMD5(string filename, string md5sum)
        {
            MD5Cng md5 = new MD5Cng();
            string computedString = "";
            using (FileStream fstr = new FileStream(filename, FileMode.Open)) {
                byte[] computed = md5.ComputeHash(fstr);
                for (int i = 0; i < computed.Length; i++)
                    computedString += computed[i].ToString("X2");
            }
            return computedString.ToLower() == md5sum.ToLower();
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

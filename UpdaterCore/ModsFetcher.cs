using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace UpdaterCore
{
    public class ModPackVersion : IComparable<ModPackVersion>
    {
        private static Regex filenamePattern = new Regex("mods([0-9]{4})([0-9]{2})([0-9]{2})([0-9]{3})\\.zip", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        public DateTime ReleaseDate { get; private set; }
        public int ReleaseNo { get; private set; }
        public string Identifier { get
            {
                return $"{ReleaseDate.Year}-{ReleaseDate.Month}-{ReleaseDate.Day}-{ReleaseNo}";
            } }
        public string PackageUrl { get; private set; }
        public ModPackVersion(string baseUrl, string filename)
        {
            PackageUrl = baseUrl + "/" + filename;
            Match match = filenamePattern.Match(filename);
            ReleaseNo = int.Parse(match.Groups[4].Value);
            ReleaseDate = new DateTime(int.Parse(match.Groups[1].Value), int.Parse(match.Groups[2].Value), int.Parse(match.Groups[3].Value));
        }
        public int CompareTo(ModPackVersion other)
        {
            if (other.ReleaseDate > this.ReleaseDate)
                return 1;
            else if (other.ReleaseDate < this.ReleaseDate)
                return -1;
            return other.ReleaseNo - this.ReleaseNo;
        }
        internal ModPackVersion()
        {

        }
    }
    public class ModPackFetcher : WebRequestable
    {
        private string updateUrl;
        private static Regex linkPattern = new Regex("\\<a href=\"([a-zA-Z0-9]+\\.zip)\"\\>", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Multiline);
        public ModPackFetcher(string updateUrl)
        {
            this.updateUrl = updateUrl;
        }
        public IEnumerable<ModPackVersion> GetModPacks()
        {
            string responseHtml = getListHtml();
            MatchCollection links = linkPattern.Matches(responseHtml);
            foreach(Match link in links)
            {
                yield return new ModPackVersion(updateUrl, link.Groups[1].Value);
            }
        }
        private string getListHtml()
        {
            using (HttpWebResponse wres = CreateGetRequest(updateUrl, "text/html"))
            using (Stream str = wres.GetResponseStream())
            using (StreamReader sre = new StreamReader(str))
            {
                return sre.ReadToEnd();
            }
        }
    }
}

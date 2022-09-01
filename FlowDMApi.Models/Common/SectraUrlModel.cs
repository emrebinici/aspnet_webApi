using System;
using System.Security.Cryptography;
using System.Text;

namespace FlowDMApi.Models.Common
{
    public enum ViewerType
    {
        LiteView = 0,
        UniView = 1
    }

    /// <summary>
    /// Sectra Pacs entegrasyonu
    /// </summary>
    public class SectraUrlModel
    {
        /// <summary>
        /// Ör: pacs.antalyaeah.gov.tr (sabit parametre)
        /// </summary>
        public string HostName { get; set; }
        public string SistemPassword { get; set; }
        public string UserId { get; set; }

        public ViewerType ViewerType { get; set; }

        public string MrnGroup
        {
            get { return "URL"; }
        }

        public string PatId { get; set; }
        private string time { get; set; }
        public string Time
        {
            get
            {
                if (string.IsNullOrEmpty(time))
                {
                    TimeSpan t = (DateTime.UtcNow - new DateTime(1970, 1, 1));
                    //TimeSpan t2 = new TimeSpan(DateTime.UtcNow.Ticks);
                    var _time = (long)t.TotalSeconds;
                    time = _time.ToString();
                }

                return time;
            }
        }

        /// <summary>
        /// Algoritma ile oluşturulan key ( Değişken)
        /// DevelopersGuide.UrlLaunch” dokümanı bölüm 5.1
        /// </summary>
        public string Key
        {
            get
            {
                SHA1 sha = new SHA1CryptoServiceProvider();
                string parameters = "";
                byte[] utf8Bytes;
                byte[] sha1Hash;
                string accessKey;

                switch (ViewerType)
                {
                    case ViewerType.UniView:
                        parameters += PatId;
                        parameters += Time;
                        parameters += UserId;
                        parameters += MrnGroup;
                        parameters += "show_images";
                        parameters += "0";
                        parameters += "";
                        parameters += SistemPassword; // system password, always last
                        utf8Bytes = Encoding.UTF8.GetBytes(parameters);
                        sha1Hash = sha.ComputeHash(utf8Bytes);
                        accessKey = BitConverter.ToString(sha1Hash).Replace("-", "").ToLower();
                        return accessKey;
                    case ViewerType.LiteView:
                    default: // LiteView
                        parameters += PatId;
                        parameters += Time;
                        parameters += UserId;
                        parameters += MrnGroup;
                        parameters += SistemPassword; // system password, always last
                        utf8Bytes = Encoding.UTF8.GetBytes(parameters);
                        sha1Hash = sha.ComputeHash(utf8Bytes);
                        accessKey = BitConverter.ToString(sha1Hash).Replace("-", "").ToLower();
                        return accessKey;
                }
            }
        }

        public string UrlBilgisi
        {
            get
            {
                string urlTemplate;
                switch (ViewerType)
                {
                    case ViewerType.UniView:
                        //https://10.207.25.20/UniView/#/apiLaunch?pat_id=206297&time=1539166481&user_id=his&mrn_group=URL&uniview_cmd=show_images&allow_pat_change=0&key=d9d2386c37a58b49fd6a61b2aeb56d3e007940e1
                        urlTemplate = "{0}?pat_id={1}&time={2}&user_id={3}&mrn_group={4}&uniview_cmd=show_images&allow_pat_change=0&key={5}";
                        return string.Format(urlTemplate, HostName, PatId, Time, UserId, MrnGroup, Key);
                    case ViewerType.LiteView:
                    default:
                        urlTemplate = "{0}?pat_id={1}&time={2}&user_id={3}&mrn_group={4}&key={5}";
                        return string.Format(urlTemplate, HostName, PatId, Time, UserId, MrnGroup, Key);
                }

                //return string.Format("{0}?pat_id={1}&time={2}&user_id={3}&mrn_group={4}&key={5}",
                //    HostName, PatId, Time, UserId, MrnGroup, Key);
                /*
                 * https://pacs.antalyaeah.gov.tr/LiteView/index.html#launch?pat_id=2002126612&time=1430821824&user_id=yusuf&mrn_group=URL&key=d807462f5f9d2eeb7eab67eaa021551301d1f6d3
                */
            }
        }

        public SectraUrlModel(string hostName, string sistemPassword, string userId, string patId, ViewerType viewerType = ViewerType.LiteView)
        {
            HostName = hostName;
            SistemPassword = sistemPassword;
            UserId = userId;
            PatId = patId;
            ViewerType = viewerType;
        }
    }
}

using System.IO;
using System.Net;
using System.Text;
using UnityEngine;

namespace MotionGenerator
{
    public class JsonHttpFetcher
    {
        private readonly string _hostname;
        private readonly int _port;

        public JsonHttpFetcher(string hostname, int port = 80)
        {
            _hostname = hostname;
            _port = port;
        }

        public virtual string Post(string endpoint, string body)
        {
            var postDataBytes = Encoding.ASCII.GetBytes(body);
            var req =
                WebRequest.Create(string.Format("http://{0}:{1}/{2}", _hostname, _port, endpoint));
            req.Method = "POST";
            req.ContentType = "application/json";
            req.ContentLength = postDataBytes.Length;
            var reqStream = req.GetRequestStream();
            reqStream.Write(postDataBytes, 0, postDataBytes.Length);
            reqStream.Close();

            var res = req.GetResponse();
            var resStream = res.GetResponseStream();
            var sr = new StreamReader(resStream);
            var returnString = sr.ReadToEnd();
            sr.Close();
            return returnString;
        }

        public string Post(string endpoint)
        {
            return Post(endpoint, JsonUtility.ToJson(""));
        }
    }
}
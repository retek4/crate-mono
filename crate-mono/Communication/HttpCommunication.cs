using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using Crate.Exceptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Crate.Communication
{
    public class HttpCommunication : ICommunication
    {
        private readonly int _timeout;

        public HttpCommunication(int timeout = 3000)
        {
            _timeout = timeout;
        }

        private HttpWebRequest GetWebRequest(string uri, SqlRequest request)
        {
            var wreq = (HttpWebRequest)WebRequest.Create(uri);
            wreq.Method = "POST";
            wreq.ContentType = "application/json";
            wreq.Timeout = _timeout;

            if (request == null)
                throw new ArgumentNullException("request");

            var dataload = JsonConvert.SerializeObject(request,
                new JsonSerializerSettings() {NullValueHandling = NullValueHandling.Ignore,Formatting = Formatting.None});
            var buffer = Encoding.UTF8.GetBytes(dataload);
            using (var sw = wreq.GetRequestStream())
            {
                sw.Write(buffer, 0, buffer.Length);
            }

            return wreq;
        }

        public SqlResponse Get(string uri, SqlRequest request)
        {
            var wreq = GetWebRequest(uri,request);
            var httpResponse = wreq.GetResponse();

            var rawdata = "";
            var validresponse = false;
            using (var stream = httpResponse.GetResponseStream())
            {
                if (stream != null)
                    using (var sr = new StreamReader(stream))
                    {
                        rawdata = sr.ReadToEnd();
                        validresponse = true;
                    }
            }

            if (!validresponse) 
                 throw new CrateException("Invalid or empty response!");
            if(((HttpWebResponse)httpResponse).StatusCode!=HttpStatusCode.OK)
                throw new CrateException("Invalid response status code: "+((HttpWebResponse)httpResponse).StatusCode);
            
            return JsonConvert.DeserializeObject<SqlResponse>(rawdata);
        }

        public async Task<SqlResponse> GetAsync(string uri, SqlRequest request)
        {
            var wreq = GetWebRequest(uri, request);
            var httpResponse = await wreq.GetResponseAsync();

            var rawdata = "";
            var validresponse = false;
            using (var stream = httpResponse.GetResponseStream())
            {
                if (stream != null)
                    using (var sr = new StreamReader(stream))
                    {
                        rawdata = sr.ReadToEnd();
                        validresponse = true;
                    }
            }

            if (!validresponse)
                throw new CrateException("Invalid or empty response!");
            if (((HttpWebResponse)httpResponse).StatusCode != HttpStatusCode.OK)
                throw new CrateException("Invalid response status code: " + ((HttpWebResponse)httpResponse).StatusCode);
            
            return JsonConvert.DeserializeObject<SqlResponse>(rawdata);
        }
    }
}

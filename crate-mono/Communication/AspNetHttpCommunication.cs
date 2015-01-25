using System;
using System.Net.Http;
using System.Threading.Tasks;
using Crate.Exceptions;

namespace Crate.Communication
{
    [Obsolete("Replaced with internal solution, reducing dependencies")]
    internal class AspNetHttpCommunication:ICommunication
    {
        public SqlResponse Get(string uri, SqlRequest request)
        {
            return GetAsync(uri, request).Result;
        }

        public async Task<SqlResponse> GetAsync(string uri, SqlRequest request)
        {
            using (var client = new HttpClient())
            {
                var resp = await client.PostAsJsonAsync(uri, request);
                if (!resp.IsSuccessStatusCode)
                {
                    throw new CrateDbException(resp.ReasonPhrase + " " + resp.Content.ReadAsStringAsync().Result);
                }
                return await resp.Content.ReadAsAsync<SqlResponse>();
            }
        }
    }
}

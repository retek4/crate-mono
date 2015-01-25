using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crate.Communication
{
    internal interface ICommunication
    {
        SqlResponse Get(string uri,SqlRequest request);

        Task<SqlResponse> GetAsync(string uri, SqlRequest request);
    }
}

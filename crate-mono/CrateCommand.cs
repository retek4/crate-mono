using System;
using System.IO;
using System.Linq;
using System.Data;
using System.Data.Common;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net;
using Crate.Communication;
using Crate.Exceptions;

namespace Crate
{
    public class CrateCommand : IDbCommand
    {
        private readonly CrateConnection _connection;
        private readonly CrateParameterCollection _parameters = new CrateParameterCollection();

        public string CommandText { get; set; }
        public int CommandTimeout { get; set; }

        public IDbConnection Connection
        {
            get
            {
                return _connection;
            }
            set
            {
                throw new InvalidOperationException();
            }
        }

        public CrateCommand(string commandText, CrateConnection connection)
        {
            CommandText = commandText;
            this._connection = connection;
        }

        #region IDbCommand implementation

        public void Cancel()
        {
            throw new NotImplementedException();
        }

        public IDbDataParameter CreateParameter()
        {
            return new CrateParameter();
        }

        public int ExecuteNonQuery()
        {
            var task = ExecuteNonQueryAsync();
            task.Wait();
            return task.Result;
        }

        public async Task<int> ExecuteNonQueryAsync()
        {
            return (await Execute()).rowcount;
        }

        public IDataReader ExecuteReader()
        {
            var task = ExecuteReaderAsync();
            task.Wait();
            return task.Result;
        }

        protected Task<SqlResponse> Execute()
        {
            return Execute(0);
        }

        private async Task<SqlResponse> Execute(int currentRetry)
        {
            var server = _connection.NextServer();
            if (server == null)
                throw new CrateException("No servers online!");
            try
            {
                /*switch (server.Scheme)
                {
                    case "http":
                    case "https":
                    {
                        
                    }
                    case "tcp"://??
                }*/
                return
                    await
                        (new HttpCommunication()).GetAsync(server.SqlUri(),
                            new SqlRequest(CommandText, _parameters.Select(x => x.Value).ToArray()));
            }
            catch (WebException ex)
            {
                if (ex.Response == null ||
                    ((HttpWebResponse)ex.Response).StatusCode == HttpStatusCode.InternalServerError)
                {
                    _connection.MarkAsFailed(server);
                    if (currentRetry > 3)
                    {
                        throw new CrateDbException("Retry count exceeded", ex);
                    }
                }
                else
                {
                    if (ex.Response == null)
                        throw new CrateDbException(ex.Status.ToString());

                    switch (((HttpWebResponse)ex.Response).StatusCode)
                    {
                        case HttpStatusCode.NotFound:
                            {
                                throw new CrateNotFound(ex.Message);
                            }
                        default:
                            {
                                var respone = ex.Response.GetResponseStream();
                                if (respone != null)
                                {
                                    using (var t = new StreamReader(respone))
                                    {
                                        throw new CrateDbException(t.ReadToEnd());
                                    }
                                }
                                throw new CrateDbException("unknown error");
                            }
                    }
                }
            }
            return await Execute(currentRetry++);
        }

        public async Task<IDataReader> ExecuteReaderAsync()
        {
            return new CrateDataReader(await Execute());
        }

        public IDataReader ExecuteReader(CommandBehavior behavior)
        {
            return ExecuteReader();
        }

        public object ExecuteScalar()
        {
            using (var reader = ExecuteReader())
            {
                reader.Read();
                return reader[0];
            }
        }

        public void Prepare()
        {
        }


        public CommandType CommandType { get; set; }

        public IDataParameterCollection Parameters
        {
            get
            {
                return _parameters;
            }
        }

        public IDbTransaction Transaction
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public UpdateRowSource UpdatedRowSource
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        #endregion

        #region IDisposable implementation

        public void Dispose()
        {
        }

        #endregion
    }
}


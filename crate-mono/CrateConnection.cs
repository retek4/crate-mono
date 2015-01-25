using System;
using System.Linq;
using System.Data;
using System.Net.Http;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Net;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Crate
{
    [DebuggerDisplay("<CrateServer {Hostname}:{Port}>")]
    public class CrateServer
    {
        private readonly Regex serverRex = new Regex(@"^(https?)?(://)?([^:]*):?(\d*)$");

        public string Scheme { get; set; }
        public string Hostname { get; set; }
        public int Port { get; set; }

        public CrateServer() : this(null) { }

        public CrateServer(String server)
        {
            Hostname = "localhost";
            Scheme = "http";
            Port = 4200;
            if (server == null)
            {
                return;
            }

            var m = serverRex.Match(server);
            if (!m.Success) return;

            Scheme = string.IsNullOrEmpty(m.Groups[1].Value) ? "http" : m.Groups[1].Value;
            Hostname = string.IsNullOrEmpty(m.Groups[3].Value) ? "localhost" : m.Groups[3].Value;
            Port = int.Parse(string.IsNullOrEmpty(m.Groups[4].Value) ? "4200" : m.Groups[4].Value);
        }

        public string SqlUri()
        {
            return string.Format("{0}://{1}:{2}/_sql", Scheme, Hostname, Port);
        }
    }

    public class CrateConnection : IDbConnection
    {
        private string _connectionString;
        private ConnectionState _state;
        private readonly List<CrateServer> _allServers;
        private int _currentServer = 0;
        private readonly object _lockObj = new object();
        public List<CrateServer> ActiveServers { get; private set; }

        public CrateConnection() : this("localhost:4200") { }

        public CrateConnection(string connectionString)
        {
            _allServers = new List<CrateServer>();
            foreach (var server in connectionString.Split(','))
            {
                _allServers.Add(new CrateServer(server.Trim()));
            }
            this.ActiveServers = _allServers;
            this._connectionString = connectionString;
            this._state = ConnectionState.Closed;
        }

        public CrateServer NextServer()
        {
            lock (_lockObj)
            {
                if (ActiveServers.Count == 0)
                    return null;
                var server = ActiveServers[_currentServer];
                _currentServer++;
                if (_currentServer >= ActiveServers.Count)
                {
                    _currentServer = 0;
                }
                return server;
            }
        }

        public void MarkAsFailed(CrateServer server)
        {
            lock (_lockObj)
            {
                if (ActiveServers.Count == 1)
                {
                    ActiveServers = _allServers;
                }
                ActiveServers.Remove(server);
                Task.Delay(TimeSpan.FromMinutes(3)).ContinueWith(x => AddServer(server));
                _currentServer = 0;
            }
        }

        private void AddServer(CrateServer server)
        {
            lock (_lockObj)
            {
                if (!ActiveServers.Contains(server))
                {
                    ActiveServers.Add(server);
                }
            }
        }

        #region IDbConnection implementation

        public IDbTransaction BeginTransaction()
        {
            throw new NotImplementedException();
        }

        public IDbTransaction BeginTransaction(IsolationLevel il)
        {
            throw new NotImplementedException();
        }

        public void ChangeDatabase(string databaseName)
        {
            throw new NotImplementedException();
        }

        public void Close()
        {
            this._state = ConnectionState.Closed;
        }

        public IDbCommand CreateCommand()
        {
            return new CrateCommand(null, this);
        }

        public void Open()
        {
            this._state = ConnectionState.Connecting;
            using (var cmd = CreateCommand())
            {
                cmd.CommandText = "select id from sys.cluster";
                var reader = cmd.ExecuteReader();
                reader.Read();
            }
            this._state = ConnectionState.Open;
        }

        public string ConnectionString
        {
            get
            {
                return _connectionString;
            }
            set
            {
                _connectionString = value;
            }
        }

        public int ConnectionTimeout
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string Database
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public ConnectionState State
        {
            get
            {
                return _state;
            }
        }

        #endregion

        #region IDisposable implementation

        public void Dispose()
        {
            if (_state != ConnectionState.Closed)
            {
                _state = ConnectionState.Closed;
            }
        }

        #endregion
    }
}


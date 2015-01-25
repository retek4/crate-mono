using System;
using System.Data;
using System.Linq;
using System.Xml;
using System.Collections.Generic;
using Crate.Helpers;
using Newtonsoft.Json.Linq;

namespace Crate
{
    public class CrateDataReader : IDataReader
    {
        internal readonly SqlResponse SqlResponse;
        private int _currentRow = -1;
        private bool _closed = false;
        private readonly DateTime _unixDt = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        private readonly Type _type;


        public CrateDataReader(SqlResponse sqlResponse)
        {
            SqlResponse = sqlResponse;
            _type = null;
        }

        public CrateDataReader(SqlResponse sqlResponse, Type type)
        {
            SqlResponse = sqlResponse;
            _type = type;
        }

        #region IDataReader implementation

        public void Close()
        {
            _closed = true;
        }

        /*var type = dt.Columns[j].DataType;
                   object obj = null;

                   switch (type.GetExtendedTypeCode())
                   {
                       case ExtendedTypeCode.Int32: { obj = GetInt32(j); break; }
                       case ExtendedTypeCode.Int16: { obj = GetInt32(j); break; }
                       case ExtendedTypeCode.Int64: { obj = GetInt32(j); break; }
                       case ExtendedTypeCode.Decimal: { obj = GetInt32(j); break; }
                       case ExtendedTypeCode.Boolean: { obj = GetInt32(j); break; }
                       case ExtendedTypeCode.String: { obj = GetString(j); break; }
                       case ExtendedTypeCode.DateTime: { obj = GetDateTime(j); break; }
                       case ExtendedTypeCode.Guid: { obj = GetGuid(j); break; }
                       case ExtendedTypeCode.Byte: { obj = GetByte(j); break; }
                       case ExtendedTypeCode.Float: { obj = GetFloat(j); break; }
                       case ExtendedTypeCode.Double: { obj = GetChar(j); break; }
                       case ExtendedTypeCode.JObject: { obj = _sqlResponse.rows[_currentRow][j]; break; }
                       default: { obj = GetString(j); break; }
                   };
         _currentRow = -1;
            while (Read())
            {
                var ol = new List<object>();
                var row = _sqlResponse.rows[_currentRow];
                for (var j = 0; j < row.Length; j++)
                {
                    ol.Add(_sqlResponse.rows[_currentRow][j]);
                }

                dt.Rows.Add(ol.ToArray());
            }
            _currentRow = -1;*/

        public DataTable GetSchemaTable()
        {
            var dt = new DataTable();
            if (_type == null)
            {
                foreach (var col in SqlResponse.cols)
                {
                    dt.Columns.Add(col);
                } foreach (var row in SqlResponse.rows)
                {
                    dt.Rows.Add(row);
                }
            }
            else
            {
                dt = this.GetSchemaTable(_type);//TODO REVIEW
            }
            return dt;
        }


        public static T GetValue<T>(string value)
        {
            return (T)Convert.ChangeType(value, typeof(T));
        }

        public bool NextResult()
        {
            return SqlResponse.rows.Length > _currentRow;
        }

        public bool Read()
        {
            _currentRow++;
            return NextResult();
        }

        public int Depth
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public bool IsClosed
        {
            get
            {
                return _closed;
            }
        }

        public int RecordsAffected
        {
            get
            {
                return SqlResponse.rowcount;
            }
        }

        #endregion

        #region IDataRecord implementation

        public bool GetBoolean(int i)
        {
            return (bool)SqlResponse.rows[_currentRow][i];
        }

        public byte GetByte(int i)
        {
            return (byte)SqlResponse.rows[_currentRow][i];
        }

        public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            throw new NotImplementedException();
        }

        public char GetChar(int i)
        {
            return (char)SqlResponse.rows[_currentRow][i];
        }

        public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
        {
            throw new NotImplementedException();
        }

        public IDataReader GetData(int i)
        {
            throw new NotImplementedException();
        }

        public string GetDataTypeName(int i)
        {
            throw new NotImplementedException();
        }

        public DateTime GetDateTime(int i)
        {
            return _unixDt.AddMilliseconds(GetInt64(i));
        }

        public decimal GetDecimal(int i)
        {
            return (decimal)SqlResponse.rows[_currentRow][i];
        }

        public double GetDouble(int i)
        {
            return (double)SqlResponse.rows[_currentRow][i];
        }
        public long GetLong(int i)
        {
            return (long)SqlResponse.rows[_currentRow][i];
        }

        public Type GetFieldType(int i)
        {
            throw new NotImplementedException();
        }

        public float GetFloat(int i)
        {
            return (float)SqlResponse.rows[_currentRow][i];
        }

        public Guid GetGuid(int i)
        {
            return Guid.Parse((string)SqlResponse.rows[_currentRow][i]);
        }

        public short GetInt16(int i)
        {
            return (short)SqlResponse.rows[_currentRow][i];
        }

        public int GetInt32(int i)
        {
            return (int)SqlResponse.rows[_currentRow][i];
        }

        public long GetInt64(int i)
        {
            return (long)SqlResponse.rows[_currentRow][i];
        }

        public string GetName(int i)
        {
            return SqlResponse.cols[i];
        }

        public int GetOrdinal(string name)
        {
            return Array.BinarySearch(SqlResponse.cols, name);
        }

        public string GetString(int i)
        {
            return (string)SqlResponse.rows[_currentRow][i];
        }

        public object GetValue(int i)
        {
            return SqlResponse.rows[_currentRow][i];
        }

        public int GetValues(object[] values)
        {
            int i = 0;
            int j = 0;
            for (; i < values.Length && j < SqlResponse.cols.Length; i++, j++)
            {
                values[i] = SqlResponse.rows[_currentRow][j];
            }
            return i;
        }

        public bool IsDBNull(int i)
        {
            return SqlResponse.rows[_currentRow][i] == null;
        }

        public int FieldCount
        {
            get
            {
                return SqlResponse.cols.Length;
            }
        }

        public object this[string name]
        {
            get
            {
                return SqlResponse.rows[_currentRow][GetOrdinal(name)];
            }
        }

        public object this[int index]
        {
            get
            {
                return SqlResponse.rows[_currentRow][index];
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


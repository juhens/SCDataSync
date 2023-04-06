using SCDataSync.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCDataSync.Communication.IpcProtocol
{
    internal class DataProtocol
    {
        //데이터 영역은 구조화 하지않음
        internal DataProtocol(JhMemory j, ulong baseAddress, uint maxUserSize, uint userCp)
        {
            this._j = j;
            this._maxUserSize = maxUserSize;
            this._baseAddress = baseAddress + maxUserSize * userCp;
            
        }
        private readonly JhMemory _j;
        private readonly ulong _baseAddress;
        private readonly uint _maxUserSize;

        private bool ReadData(ref byte[] buffer, uint index)
        {
            return _j.Read(_baseAddress + index, ref buffer);
        }

        internal bool ReceiveData(ref byte[] buffer, uint index)
        {
            if (buffer.Length + index > _maxUserSize)
            {
                throw new Exception("out of index");
            }
            return ReadData(ref buffer, index);
        }
    }
}

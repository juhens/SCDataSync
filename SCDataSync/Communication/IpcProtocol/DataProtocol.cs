using SCDataSync.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SCDataSync.Memory.Extensions;

namespace SCDataSync.Communication.IpcProtocol
{
    internal class DataProtocol
    {
        internal DataProtocol(JhMemory j, ulong baseAddress, uint maxUserSize, uint userCp)
        {
            this._j = j;
            this._maxUserSize = maxUserSize;
            this._baseAddress = baseAddress + maxUserSize * userCp;
            
        }
        private readonly JhMemory _j;
        private readonly ulong _baseAddress;
        private readonly uint _maxUserSize;

        private bool ReadData(Span<byte> buffer, uint index)
        {
            return _j.Read(_baseAddress + index, buffer);
        }

        internal bool ReceiveData(Span<byte> buffer, uint index)
        {
            if (buffer.Length + index > _maxUserSize)
            {
                throw new Exception("out of index");
            }
            return ReadData(buffer, index);
        }
    }
}

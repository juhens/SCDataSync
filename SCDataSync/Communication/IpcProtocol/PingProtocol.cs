﻿using SCDataSync.Memory;
using System.Runtime.InteropServices;
using System.Text;

namespace SCDataSync.Communication.IpcProtocol
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct PingStruct
    {
        internal byte updatePing;

        private Span<byte> GetByteSpan()
        {
            var thisStructSpan = MemoryMarshal.CreateSpan(ref this, 1);
            return MemoryMarshal.Cast<PingStruct, byte>(thisStructSpan);
        }
        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach (var b in GetByteSpan())
            {
                sb.Append($"{b:X2} ");
            }
            return sb.ToString();
        }
    }
    internal class PingProtocol
    {
        internal PingProtocol(JhMemory j, ulong baseAddress)
        {
            this._j = j;
            this._baseAddress = baseAddress;
        }

        private readonly JhMemory _j;
        private readonly ulong _baseAddress;
        private bool WritePingStruct(byte updatePing)
        {
            PingStruct pingStruct = new PingStruct
            {
                updatePing = updatePing
            };
            return _j.Write(_baseAddress, pingStruct);
        }

        internal bool SendUpdatePing()
        {
            return WritePingStruct((byte)1);
        }
    }

}
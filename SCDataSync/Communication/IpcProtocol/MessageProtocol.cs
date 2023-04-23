using SCDataSync.Memory;
using System.Runtime.InteropServices;
using System.Text;
using SCDataSync.Memory.Extensions;

namespace SCDataSync.Communication.IpcProtocol
{
    internal enum MessageType : byte
    {
        None = 0,
        Custom = 1,
        Normal = 2,
        Waring = 3,
        Error = 4,
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct MessageStruct
    {
        internal MessageType messageType;
        private readonly byte unused0;
        private readonly byte unused1;
        private readonly byte unused2;

        private readonly _512 messageBuffer;

        private Span<byte> GetByteSpan()
        {
            var thisStructSpan = MemoryMarshal.CreateSpan(ref this, 1);
            return MemoryMarshal.Cast<MessageStruct, byte>(thisStructSpan);
        }
        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach (byte b in GetByteSpan())
            {
                sb.Append($"{b:X2} ");
            }
            return sb.ToString();
        }


        internal void SetMessage(string str)
        {
            var strByteSpan = Encoding.UTF8.GetBytes(str).AsSpan();
            //4 means the sum of size messageType, unused0, unused1, unused2
            var sliceSpan = GetByteSpan()[4..];

            //Set the copy length to the smaller value between the target span and the current span,
            //and subtract 1 from the length of the slice span to leave the last byte empty.
            var copyLength = Math.Min(strByteSpan.Length, sliceSpan.Length - 1);
            strByteSpan[..copyLength].CopyTo(sliceSpan);
        }
    }
    internal class MessageProtocol
    {
        internal MessageProtocol(JhMemory j, ulong baseAddress)
        {
            this._j = j;
            this._baseAddress = baseAddress;
        }

        private readonly JhMemory _j;
        private readonly ulong _baseAddress;
        private bool WriteMessageStruct(MessageType messageType, string str)
        {
            var messageStruct = new MessageStruct
            {
                messageType = messageType
            };
            messageStruct.SetMessage(str);
            return _j.Write(_baseAddress, messageStruct.AsByteSpan());
        }

        internal bool SendMessage(MessageType messageType, string str)
        {
            return WriteMessageStruct(messageType, str);
        }
    }

}

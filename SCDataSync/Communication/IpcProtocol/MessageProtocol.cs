using SCDataSync.Memory;
using System.Runtime.InteropServices;
using System.Text;

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

        private readonly long dummy0;
        private readonly long dummy1;
        private readonly long dummy2;
        private readonly long dummy3;
        private readonly long dummy4;
        private readonly long dummy5;
        private readonly long dummy6;
        private readonly long dummy7;
        private readonly long dummy8;
        private readonly long dummy9;
        private readonly long dummy10;
        private readonly long dummy11;
        private readonly long dummy12;
        private readonly long dummy13;
        private readonly long dummy14;
        private readonly long dummy15;
        private readonly long dummy16;
        private readonly long dummy17;
        private readonly long dummy18;
        private readonly long dummy19;
        private readonly long dummy20;
        private readonly long dummy21;
        private readonly long dummy22;
        private readonly long dummy23;
        private readonly long dummy24;
        private readonly long dummy25;
        private readonly long dummy26;
        private readonly long dummy27;
        private readonly long dummy28;
        private readonly long dummy29;
        private readonly long dummy30;
        private readonly long dummy31;
        private readonly long dummy32;
        private readonly long dummy33;
        private readonly long dummy34;
        private readonly long dummy35;
        private readonly long dummy36;
        private readonly long dummy37;
        private readonly long dummy38;
        private readonly long dummy39;
        private readonly long dummy40;
        private readonly long dummy41;
        private readonly long dummy42;
        private readonly long dummy43;
        private readonly long dummy44;
        private readonly long dummy45;
        private readonly long dummy46;
        private readonly long dummy47;
        private readonly long dummy48;
        private readonly long dummy49;
        private readonly long dummy50;
        private readonly long dummy51;
        private readonly long dummy52;
        private readonly long dummy53;
        private readonly long dummy54;
        private readonly long dummy55;
        private readonly long dummy56;
        private readonly long dummy57;
        private readonly long dummy58;
        private readonly long dummy59;
        private readonly long dummy60;
        private readonly long dummy61;
        private readonly long dummy62;
        private readonly long dummy63;

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
            MessageStruct messageStruct = new MessageStruct
            {
                messageType = messageType
            };
            messageStruct.SetMessage(str);
            return _j.Write(_baseAddress, messageStruct);
        }

        internal bool SendMessage(MessageType messageType, string str)
        {
            return WriteMessageStruct(messageType, str);
        }
    }

}

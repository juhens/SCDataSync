using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SCDataSync.Memory.Extensions
{
    internal static class SpanExtension
    {
        internal static Span<byte> AsByteSpan<T>(ref this T value) where T : unmanaged
        {
            var span = MemoryMarshal.CreateSpan(ref value, 1);
            var byteSpan = MemoryMarshal.Cast<T, byte>(span);
            return byteSpan;
        }

        internal static Span<byte> AsByteSpan<T>(this T[] value) where T : unmanaged
        {
            var byteSpan = MemoryMarshal.Cast<T, byte>(value);
            return byteSpan;
        }
    }

}

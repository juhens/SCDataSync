using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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
            var byteSpan = MemoryMarshal.AsBytes(span);
            return byteSpan;
        }

        internal static ReadOnlySpan<byte> AsReadOnlyByteSpan<T>(ref this T value) where T : unmanaged
        {
            var span = MemoryMarshal.CreateReadOnlySpan(ref value, 1);
            var byteSpan = MemoryMarshal.AsBytes(span);
            return byteSpan;
        }

        internal static Span<byte> AsByteSpan<T>(this T[] value) where T : unmanaged
        {
            var byteSpan = MemoryMarshal.AsBytes(value.AsSpan());
            return byteSpan;
        }

        internal static ReadOnlySpan<byte> AsReadOnlyByteSpan<T>(this T[] value) where T : unmanaged
        {
            var byteSpan = MemoryMarshal.AsBytes(value.AsSpan());
            return byteSpan;
        }

        internal static Span<TTo> AsCastSpan<TFrom, TTo>(ref this TFrom value) where TFrom : unmanaged where TTo : unmanaged
        {
            var span = MemoryMarshal.CreateSpan(ref value, 1);
            var castSpan = MemoryMarshal.Cast<TFrom, TTo>(span);
            return castSpan;
        }
        
        internal static Span<TTo> AsCastSpan<TFrom, TTo>(this TFrom[] value) where TFrom : unmanaged where TTo : unmanaged
        {
            var castSpan = MemoryMarshal.Cast<TFrom, TTo>(value.AsSpan());
            return castSpan;
        }

        //when attempting to modify an immutable property (such as a string)
        //System.AccessViolationException with the message
        //"Attempted to read or write protected memory. This is often an indication that other memory is corrupt."
        //internal static Span<byte> AsByteSpan<T>(this ReadOnlySpan<T> value) where T : unmanaged
        //{
        //    var span = MemoryMarshal.CreateSpan(ref MemoryMarshal.GetReference(value), value.Length);
        //    var byteSpan = MemoryMarshal.AsBytes(span);
        //    return byteSpan;
        //}
    }
}

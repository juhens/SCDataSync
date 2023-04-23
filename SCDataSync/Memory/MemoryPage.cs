namespace SCDataSync.Memory;

internal readonly struct MemoryPage
{
    internal MemoryPage(ulong baseAddress, byte[] byteArray)
    {
        this.BaseAddress = baseAddress;
        this.ByteArray = byteArray;
    }
    internal readonly ulong BaseAddress;
    internal readonly byte[] ByteArray;
}
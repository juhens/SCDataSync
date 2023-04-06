namespace SCDataSync.Memory;

internal readonly struct MemoryPage
{
    internal MemoryPage(ulong baseAddress, byte[] byteArray)
    {
        this.baseAddress = baseAddress;
        this.byteArray = byteArray;
    }
    internal readonly ulong baseAddress;
    internal readonly byte[] byteArray;
}
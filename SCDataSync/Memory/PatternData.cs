namespace SCDataSync.Memory;

internal readonly struct PatternData
{
    internal PatternData(byte[] byteArray)
    {
        //00 ~ FF range
        JumpTable = new int[256];

        for (var i = 0; i < JumpTable.Length; i++)
            JumpTable[i] = byteArray.Length;

        //set jump index
        for (var i = 0; i < byteArray.Length; i++)
        {
            if (i < byteArray.Length - 1)
                JumpTable[byteArray[i]] = byteArray.Length - i - 1;
        }
        this.ByteArray = byteArray;
    }

    internal readonly byte[] ByteArray;
    internal readonly int[] JumpTable;
}
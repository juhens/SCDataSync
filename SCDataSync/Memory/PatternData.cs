namespace SCDataSync.Memory;

internal struct PatternData
{
    internal PatternData(byte[] byteArray)
    {
        //00 ~ FF range
        jumpTable = new int[256];

        for (var i = 0; i < jumpTable.Length; i++)
            jumpTable[i] = byteArray.Length;

        //set jump index
        for (var i = 0; i < byteArray.Length; i++)
        {
            if (i < byteArray.Length - 1)
                jumpTable[byteArray[i]] = byteArray.Length - i - 1;
        }
        this.byteArray = byteArray;
    }

    internal readonly byte[] byteArray;
    internal readonly int[] jumpTable;
}
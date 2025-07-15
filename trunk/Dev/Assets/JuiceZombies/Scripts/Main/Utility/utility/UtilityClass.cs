using Unity.Collections;
using Unity.Entities;

/// <summary>
/// π§æﬂ¿‡
/// </summary>
public static class UtilityClass
{
    public static NativeArray<T> BlobArrayToNativeArray<T>(ref BlobArray<T> blobArray) where T : unmanaged
    {
        var length = blobArray.Length;
        var nativeArray = new NativeArray<T>(length, Allocator.Temp);
        for (int i = 0; i < length; ++i)
        {
            nativeArray[i] = blobArray[i];
        }

        return nativeArray;
    }
}
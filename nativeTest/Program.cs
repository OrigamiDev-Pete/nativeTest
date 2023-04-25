using System;
using System.Reflection;
using System.Runtime.InteropServices;

namespace nativeTest;

public static class Program
{
    public static void Main(string[] args)
    {
        var r = Sum(18, 1);
        var a = new[] { 1, 2, 3, 4, 5 };
        var r2 = Sum(a);

        var r3 = Native.Sum(1, 2);
        unsafe
        {
            // Proves that passing array types directly automatically fix the array without copy
            fixed (int* ap = a) { }
            var r4 = Native.SumArray(a, (ulong)a.Length);
            // The runtime calls this method when marshalling the array
            // System.Runtime.InteropServices.Marshalling.ArrayMarshaller<int, int>.ManagedToUnmanagedIn
            //    .GetPinnableReference(b);
        }

        Native.ZeroArray(a, (ulong)a.Length);

        var b = new[] { 1, 2, 3, 4, 5 };
        var bb = Marshal.AllocHGlobal(b.Length * sizeof(int));
        Marshal.Copy(b, 0, bb, b.Length);
        var r5 = Native.SumArray(bb, 5);
        Marshal.FreeHGlobal(bb);

        
        unsafe
        {
            fixed (int* bp = b)
            {
                var r6 = Native.SumArray(bp, 5);
            }
        }

        // Structs

        Native.B struct1 = new();
        var struct1Backing = new[] { 1, 2, 3, 4, 5 };
        unsafe
        {
            fixed (void* p = struct1Backing)
            {
                struct1.Data = p;
                Native.StructTest(struct1);
            }
        }

        // struct2Backing does not get referenced correctly when passing the struct to C++.
        // Making Native.A a class doesn't change the outcome.
        Native.A struct2 = new();
        var struct2Backing = new[] { 1, 2, 3, 4, 5 };
        struct2.Data = struct2Backing;
        //Native.StructTest(struct1);

        // This is a cool option because it lets you get a pinned array in a loop
        var struct3Backing = new[] { 1, 2, 3, 4, 5 };
        var handle = GCHandle.Alloc(struct3Backing, GCHandleType.Pinned);
        Native.C struct3 = new(handle.AddrOfPinnedObject());
        Native.StructTest(struct3);
        handle.Free();

        var meshStreams = new List<List<int>>
        {
            new() { 1, 2, 3, 4, 5 },
            new() { 6, 7, 8, 9, 10 }
        };

        var streams = new Native.C[meshStreams.Count];
        for (int i = 0; i < meshStreams.Count; i++)
        {
            // GetBackingBuffer is a bit a trick because we're using reflection to pull the List's _items field out.
            // Alternatively you could implement your own exanding IList<T> that exposes its backing field.
            var handleToMeshStreamBacking = GCHandle.Alloc(meshStreams[i].GetBackingBuffer(), GCHandleType.Pinned);
            streams[i].Size = 4;
            streams[i].Data = handleToMeshStreamBacking.AddrOfPinnedObject();
        }
        
        Native.StructArrayTest(streams, 2);

    }

    private static int Sum(int a, int b)
    {
        return a + b;
    }

    private static int Sum(int[] a)
    {
        var sum = 0;
        foreach (var n in a)
        {
            sum += n;
        }

        return sum;
    }
}

public static class ListExtensions
{
    public static T[] GetBackingBuffer<T>(this List<T> list)
    {
        return (T[]) typeof(List<T>).GetField("_items", BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(list) ?? Array.Empty<T>();
    }
}

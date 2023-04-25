using System.Runtime.InteropServices;

namespace nativeTest;

internal static partial class Native
{
    private const string DllName = "test.dll";
    
    [DllImport(DllName)]
    public static extern int Sum(int a, int b);

    [DllImport(DllName)]
    public static extern int SumArray(int[] a, ulong length);
    
    [DllImport(DllName)]
    public static extern int SumArray(nint a, ulong length);
    
    [DllImport(DllName)]
    public static extern unsafe int SumArray(int* a, ulong length);
    
    [DllImport(DllName)]
    public static extern void ZeroArray(int[] a, ulong length);
    
    [DllImport(DllName)]
    public static extern void StructTest(A a);
    
    [DllImport(DllName)]
    public static extern void StructArrayTest(A[] a, ulong length);
    
    [DllImport(DllName)]
    public static extern void StructTest(B a);
    
    [DllImport(DllName)]
    public static extern void StructArrayTest(B[] a, ulong length);
    
    [DllImport(DllName)]
    public static extern void StructTest(C a);
    
    [DllImport(DllName)]
    public static extern void StructArrayTest(C[] a, ulong length);

    [StructLayout(LayoutKind.Sequential)]
    public struct A
    {
        public int[]? Data = null;
        public int Size = 0;

        public A()
        {
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct B
    {
        public void* Data = null;
        public int Size = 0;

        public B()
        {
        }
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct C
    {
        public nint Data;
        public int Size;

        public C(nint dataPtr)
        {
            Data = dataPtr;
            Size = 0;
        }

    }
}
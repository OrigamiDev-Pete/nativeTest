#include <windows.h>

BOOL APIENTRY DllMain(HMODULE hModule, DWORD  ul_reason_for_call, LPVOID lpReserved)
{
    switch (ul_reason_for_call)
    {
    case DLL_PROCESS_ATTACH:
    case DLL_THREAD_ATTACH:
    case DLL_THREAD_DETACH:
    case DLL_PROCESS_DETACH:
        break;
    }
	
    return TRUE;
}

struct A
{
    void* data;
    size_t size;
};

#define API __declspec(dllexport)

extern "C" {
    API int Sum(const int a, const int b)
    {
        return a + b;
    }
    
    API int SumArray(const int* a, const size_t length)
    {
        int sum = 0;
        for (size_t i = 0; i < length; i++)
        {
            sum += a[i];
        }
        return sum;
    }

    API void ZeroArray(int* a, const size_t length)
    {
        for (size_t i = 0; i < length; i++)
        {
            a[i] = 0;
        }
    }

    API void StructTest(A a)
    {
        ZeroArray((int*)a.data, 5);
        a.size = 16;
    }

    API void StructArrayTest(A* as, size_t length)
    {
        for (size_t i = 0; i < length; i++)
        {
            ZeroArray((int*)as[i].data, 5);
            as[i].size = 16;
        }
    }
}

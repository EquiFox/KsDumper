using System;
using System.Runtime.InteropServices;

namespace KsDumperClient.Utility
{
    public static class MarshalUtility
    {
        public static IntPtr CopyStructToMemory<T>(T obj) where T : struct
        {
            IntPtr unmanagedAddress = AllocEmptyStruct<T>();
            Marshal.StructureToPtr(obj, unmanagedAddress, true);

            return unmanagedAddress;
        }

        public static IntPtr AllocEmptyStruct<T>() where T : struct
        {
            int structSize = Marshal.SizeOf<T>();
            IntPtr structPointer = AllocZeroFilled(Marshal.SizeOf<T>());

            return structPointer;
        }

        public static IntPtr AllocZeroFilled(int size)
        {
            IntPtr allocatedPointer = Marshal.AllocHGlobal(size);
            ZeroMemory(allocatedPointer, size);

            return allocatedPointer;
        }

        public static void ZeroMemory(IntPtr pointer, int size)
        {
            for (int i = 0; i < size; i++)
            {
                Marshal.WriteByte(pointer + i, 0x0);
            }
        }

        public static T GetStructFromMemory<T>(IntPtr unmanagedAddress, bool freeMemory = true) where T : struct
        {
            T structObj = Marshal.PtrToStructure<T>(unmanagedAddress);

            if (freeMemory)
            {
                Marshal.FreeHGlobal(unmanagedAddress);
            }
            return structObj;
        }
    }
}

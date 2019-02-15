using System.Runtime.InteropServices;

using static KsDumperClient.Utility.WinApi;

namespace KsDumperClient.Driver
{
    public static class Operations
    {
        public static readonly uint IO_GET_PROCESS_LIST = CTL_CODE(FILE_DEVICE_UNKNOWN, 0x1724, METHOD_BUFFERED, FILE_ANY_ACCESS);

        public static readonly uint IO_COPY_MEMORY = CTL_CODE(FILE_DEVICE_UNKNOWN, 0x1725, METHOD_BUFFERED, FILE_ANY_ACCESS);

        [StructLayout(LayoutKind.Sequential)]
        public struct KERNEL_PROCESS_LIST_OPERATION
        {
            public ulong bufferAddress;
            public int bufferSize;
            public int processCount;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct KERNEL_COPY_MEMORY_OPERATION
        {
            public int targetProcessId;
            public ulong targetAddress;
            public ulong bufferAddress;
            public int bufferSize;
        }

        private static uint CTL_CODE(int deviceType, int function, int method, int access)
        {
            return (uint)(((deviceType) << 16) | ((access) << 14) | ((function) << 2) | (method));
        }
    }
}

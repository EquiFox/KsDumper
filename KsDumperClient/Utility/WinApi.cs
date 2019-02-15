using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace KsDumperClient.Utility
{
    public static class WinApi
    {
        public static readonly int FILE_DEVICE_UNKNOWN = 0x22;
        public static readonly int METHOD_BUFFERED = 0x0;
        public static readonly int FILE_ANY_ACCESS = 0x0;

        public static readonly IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);

        [DllImport("kernel32.dll", CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern IntPtr CreateFileA(
            [MarshalAs(UnmanagedType.LPStr)] string filename,
            [MarshalAs(UnmanagedType.U4)] FileAccess access,
            [MarshalAs(UnmanagedType.U4)] FileShare share,
            IntPtr securityAttributes,
            [MarshalAs(UnmanagedType.U4)] FileMode creationDisposition,
            [MarshalAs(UnmanagedType.U4)] FileAttributes flagsAndAttributes,
            IntPtr templateFile);

        [DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool DeviceIoControl(IntPtr hDevice, uint dwIoControlCode,
            IntPtr lpInBuffer, int nInBufferSize,
            IntPtr lpOutBuffer, int nOutBufferSize,
            IntPtr lpBytesReturned, IntPtr lpOverlapped);

        [DllImport("kernel32.dll")]
        public static extern int GetLongPathName(string path, StringBuilder pszPath, int cchPath);
    }
}

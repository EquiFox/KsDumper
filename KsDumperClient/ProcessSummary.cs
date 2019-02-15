using KsDumperClient.Utility;
using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace KsDumperClient
{
    public class ProcessSummary
    {
        public int ProcessId { get; private set; }
        public string ProcessName { get; private set; }
        public ulong MainModuleBase { get; private set; }
        public string MainModuleFileName { get; private set; }
        public uint MainModuleImageSize { get; private set; }
        public ulong MainModuleEntryPoint { get; private set; }
        public bool IsWOW64 { get; private set; }

        private ProcessSummary(int processId, ulong mainModuleBase, string mainModuleFileName, uint mainModuleImageSize, ulong mainModuleEntryPoint, bool isWOW64)
        {
            ProcessId = processId;
            MainModuleBase = mainModuleBase;
            MainModuleFileName = FixFileName(mainModuleFileName);
            MainModuleImageSize = mainModuleImageSize;
            MainModuleEntryPoint = mainModuleEntryPoint;
            ProcessName = Path.GetFileName(MainModuleFileName);
            IsWOW64 = isWOW64;
        }

        private string FixFileName(string fileName)
        {
            if (fileName.StartsWith(@"\"))
            {
                return fileName;
            }

            StringBuilder sb = new StringBuilder(256);
            int length = WinApi.GetLongPathName(fileName, sb, sb.Capacity);

            if (length > sb.Capacity)
            {
                sb.Capacity = length;
                length = WinApi.GetLongPathName(fileName, sb, sb.Capacity);
            }
            return sb.ToString();
        }

        public static ProcessSummary FromStream(BinaryReader reader)
        {
            return new ProcessSummary 
            (
                reader.ReadInt32(),
                reader.ReadUInt64(),
                Encoding.Unicode.GetString(reader.ReadBytes(512)).Split('\0')[0],
                reader.ReadUInt32(),
                reader.ReadUInt64(),
                reader.ReadBoolean()
            );
        }
    }
}

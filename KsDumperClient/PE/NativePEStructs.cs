using System;
using System.Runtime.InteropServices;

namespace KsDumperClient.PE
{
    public static class NativePEStructs
    {
        public const uint IMAGE_NT_OPTIONAL_HDR32_MAGIC = 0x10b;
        public const uint IMAGE_NT_OPTIONAL_HDR64_MAGIC = 0x20b;

        public const uint IMAGE_NUMBEROF_DIRECTORY_ENTRIES = 16;

        public const uint IMAGE_DIRECTORY_ENTRY_EXPORT = 0;
        public const uint IMAGE_DIRECTORY_ENTRY_IMPORT = 1;
        public const uint IMAGE_DIRECTORY_ENTRY_RESOURCE = 2;
        public const uint IMAGE_DIRECTORY_ENTRY_EXCEPTION = 3;
        public const uint IMAGE_DIRECTORY_ENTRY_SECURITY = 4;
        public const uint IMAGE_DIRECTORY_ENTRY_BASERELOC = 5;
        public const uint IMAGE_DIRECTORY_ENTRY_DEBUG = 6;
        public const uint IMAGE_DIRECTORY_ENTRY_ARCHITECTURE = 7;
        public const uint IMAGE_DIRECTORY_ENTRY_GLOBALPTR = 8;
        public const uint IMAGE_DIRECTORY_ENTRY_TLS = 9;
        public const uint IMAGE_DIRECTORY_ENTRY_LOAD_CONFIG = 10;
        public const uint IMAGE_DIRECTORY_ENTRY_BOUND_IMPORT = 11;
        public const uint IMAGE_DIRECTORY_ENTRY_IAT = 12;
        public const uint IMAGE_DIRECTORY_ENTRY_DELAY_IMPORT = 13;
        public const uint IMAGE_DIRECTORY_ENTRY_COM_DESCRIPTOR = 14;

        [StructLayout(LayoutKind.Sequential)]
        public struct IMAGE_DOS_HEADER
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public char[] e_magic;
            public ushort e_cblp;
            public ushort e_cp;
            public ushort e_crlc;
            public ushort e_cparhdr;
            public ushort e_minalloc;
            public ushort e_maxalloc;
            public ushort e_ss;
            public ushort e_sp;
            public ushort e_csum;
            public ushort e_ip;
            public ushort e_cs;
            public ushort e_lfarlc;
            public ushort e_ovno;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public ushort[] e_res1;
            public ushort e_oemid;
            public ushort e_oeminfo;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
            public ushort[] e_res2;
            public int e_lfanew;

            private string _e_magic
            {
                get { return new string(e_magic); }
            }

            public bool IsValid
            {
                get { return _e_magic == "MZ"; }
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct IMAGE_NT_HEADERS32
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public char[] Signature;

            public IMAGE_FILE_HEADER FileHeader;

            public IMAGE_OPTIONAL_HEADER32 OptionalHeader;

            private string _Signature
            {
                get { return new string(Signature); }
            }

            public bool IsValid
            {
                get { return _Signature == "PE\0\0" && OptionalHeader.Magic == IMAGE_NT_OPTIONAL_HDR32_MAGIC; }
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct IMAGE_NT_HEADERS64
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public char[] Signature;

            public IMAGE_FILE_HEADER FileHeader;

            public IMAGE_OPTIONAL_HEADER64 OptionalHeader;

            private string _Signature
            {
                get { return new string(Signature); }
            }

            public bool IsValid
            {
                get { return _Signature == "PE\0\0" && OptionalHeader.Magic == IMAGE_NT_OPTIONAL_HDR64_MAGIC; }
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct IMAGE_FILE_HEADER
        {
            internal ushort Machine;
            internal ushort NumberOfSections;
            internal uint TimeDateStamp;
            internal uint PointerToSymbolTable;
            internal uint NumberOfSymbols;
            internal ushort SizeOfOptionalHeader;
            internal ushort Characteristics;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct IMAGE_OPTIONAL_HEADER32
        {
            internal ushort Magic;
            internal byte MajorLinkerVersion;
            internal byte MinorLinkerVersion;
            internal uint SizeOfCode;
            internal uint SizeOfInitializedData;
            internal uint SizeOfUninitializedData;
            internal uint AddressOfEntryPoint;
            internal uint BaseOfCode;
            internal uint BaseOfData;
            internal uint ImageBase;
            internal uint SectionAlignment;
            internal uint FileAlignment;
            internal ushort MajorOperatingSystemVersion;
            internal ushort MinorOperatingSystemVersion;
            internal ushort MajorImageVersion;
            internal ushort MinorImageVersion;
            internal ushort MajorSubsystemVersion;
            internal ushort MinorSubsystemVersion;
            internal uint Win32VersionValue;
            internal uint SizeOfImage;
            internal uint SizeOfHeaders;
            internal uint CheckSum;
            internal ushort Subsystem;
            internal ushort DllCharacteristics;
            internal uint SizeOfStackReserve;
            internal uint SizeOfStackCommit;
            internal uint SizeOfHeapReserve;
            internal uint SizeOfHeapCommit;
            internal uint LoaderFlags;
            internal uint NumberOfRvaAndSizes;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            internal IMAGE_DATA_DIRECTORY[] DataDirectory;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct IMAGE_OPTIONAL_HEADER64
        {
            internal ushort Magic;
            internal byte MajorLinkerVersion;
            internal byte MinorLinkerVersion;
            internal uint SizeOfCode;
            internal uint SizeOfInitializedData;
            internal uint SizeOfUninitializedData;
            internal uint AddressOfEntryPoint;
            internal uint BaseOfCode;
            internal ulong ImageBase;
            internal uint SectionAlignment;
            internal uint FileAlignment;
            internal ushort MajorOperatingSystemVersion;
            internal ushort MinorOperatingSystemVersion;
            internal ushort MajorImageVersion;
            internal ushort MinorImageVersion;
            internal ushort MajorSubsystemVersion;
            internal ushort MinorSubsystemVersion;
            internal uint Win32VersionValue;
            internal uint SizeOfImage;
            internal uint SizeOfHeaders;
            internal uint CheckSum;
            internal ushort Subsystem;
            internal ushort DllCharacteristics;
            internal ulong SizeOfStackReserve;
            internal ulong SizeOfStackCommit;
            internal ulong SizeOfHeapReserve;
            internal ulong SizeOfHeapCommit;
            internal uint LoaderFlags;
            internal uint NumberOfRvaAndSizes;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            internal IMAGE_DATA_DIRECTORY[] DataDirectory;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct IMAGE_DATA_DIRECTORY
        {
            internal uint VirtualAddress;
            internal uint Size;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct IMAGE_SECTION_HEADER
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public char[] Name;

            public uint VirtualSize;

            public uint VirtualAddress;

            public uint SizeOfRawData;

            public uint PointerToRawData;

            public uint PointerToRelocations;

            public uint PointerToLinenumbers;

            public ushort NumberOfRelocations;

            public ushort NumberOfLinenumbers;

            public DataSectionFlags Characteristics;

            public string SectionName
            {
                get { return new string(Name); }
            }
        }

        [Flags]
        public enum DataSectionFlags : uint
        {
            TypeReg = 0x00000000,
            TypeDsect = 0x00000001,
            TypeNoLoad = 0x00000002,
            TypeGroup = 0x00000004,
            TypeNoPadded = 0x00000008,
            TypeCopy = 0x00000010,
            ContentCode = 0x00000020,
            ContentInitializedData = 0x00000040,
            ContentUninitializedData = 0x00000080,
            LinkOther = 0x00000100,
            LinkInfo = 0x00000200,
            TypeOver = 0x00000400,
            LinkRemove = 0x00000800,
            LinkComDat = 0x00001000,
            NoDeferSpecExceptions = 0x00004000,
            RelativeGP = 0x00008000,
            MemPurgeable = 0x00020000,
            Memory16Bit = 0x00020000,
            MemoryLocked = 0x00040000,
            MemoryPreload = 0x00080000,
            Align1Bytes = 0x00100000,
            Align2Bytes = 0x00200000,
            Align4Bytes = 0x00300000,
            Align8Bytes = 0x00400000,
            Align16Bytes = 0x00500000,
            Align32Bytes = 0x00600000,
            Align64Bytes = 0x00700000,
            Align128Bytes = 0x00800000,
            Align256Bytes = 0x00900000,
            Align512Bytes = 0x00A00000,
            Align1024Bytes = 0x00B00000,
            Align2048Bytes = 0x00C00000,
            Align4096Bytes = 0x00D00000,
            Align8192Bytes = 0x00E00000,
            LinkExtendedRelocationOverflow = 0x01000000,
            MemoryDiscardable = 0x02000000,
            MemoryNotCached = 0x04000000,
            MemoryNotPaged = 0x08000000,
            MemoryShared = 0x10000000,
            MemoryExecute = 0x20000000,
            MemoryRead = 0x40000000,
            MemoryWrite = 0x80000000
        }
    }
}

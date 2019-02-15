using System.IO;
using System.Linq;

using static KsDumperClient.PE.NativePEStructs;

namespace KsDumperClient.PE
{
    public class PE64Header
    {
        public string Signature { get; private set; }

        public PE64FileHeader FileHeader { get; private set; }

        public PE64OptionalHeader OptionalHeader { get; private set; }


        public void AppendToStream(BinaryWriter writer)
        {
            writer.Write(Signature.ToCharArray());
            FileHeader.AppendToStream(writer);
            OptionalHeader.AppendToStream(writer);
        }

        public static PE64Header FromNativeStruct(IMAGE_NT_HEADERS64 nativeStruct)
        {
            return new PE64Header
            {
                Signature = new string(nativeStruct.Signature),
                FileHeader = PE64FileHeader.FromNativeStruct(nativeStruct.FileHeader),
                OptionalHeader = PE64OptionalHeader.FromNativeStruct(nativeStruct.OptionalHeader)
            };            
        }


        public class PE64FileHeader
        {
            public ushort Machine { get; set; }
            public ushort NumberOfSections { get; set; }
            public uint TimeDateStamp { get; set; }
            public uint PointerToSymbolTable { get; set; }
            public uint NumberOfSymbols { get; set; }
            public ushort SizeOfOptionalHeader { get; set; }
            public ushort Characteristics { get; set; }

            public void AppendToStream(BinaryWriter writer)
            {
                writer.Write(Machine);
                writer.Write(NumberOfSections);
                writer.Write(TimeDateStamp);
                writer.Write(PointerToSymbolTable);
                writer.Write(NumberOfSymbols);
                writer.Write(SizeOfOptionalHeader);
                writer.Write(Characteristics);
            }

            public static PE64FileHeader FromNativeStruct(IMAGE_FILE_HEADER nativeStruct)
            {
                return new PE64FileHeader
                {
                    Machine = nativeStruct.Machine,
                    NumberOfSections = nativeStruct.NumberOfSections,
                    TimeDateStamp = nativeStruct.TimeDateStamp,
                    PointerToSymbolTable = nativeStruct.PointerToSymbolTable,
                    NumberOfSymbols = nativeStruct.NumberOfSymbols,
                    SizeOfOptionalHeader = nativeStruct.SizeOfOptionalHeader,
                    Characteristics = nativeStruct.Characteristics
                };
            }
        }
        
        public class PE64OptionalHeader
        {
            public ushort Magic { get; set; }
            public byte MajorLinkerVersion { get; set; }
            public byte MinorLinkerVersion { get; set; }
            public uint SizeOfCode { get; set; }
            public uint SizeOfInitializedData { get; set; }
            public uint SizeOfUninitializedData { get; set; }
            public uint AddressOfEntryPoint { get; set; }
            public uint BaseOfCode { get; set; }
#if WIN32
            public uint BaseOfData { get; set; }
            public uint ImageBase { get; set; }
#else
            public ulong ImageBase { get; set; }
#endif
            public uint SectionAlignment { get; set; }
            public uint FileAlignment { get; set; }
            public ushort MajorOperatingSystemVersion { get; set; }
            public ushort MinorOperatingSystemVersion { get; set; }
            public ushort MajorImageVersion { get; set; }
            public ushort MinorImageVersion { get; set; }
            public ushort MajorSubsystemVersion { get; set; }
            public ushort MinorSubsystemVersion { get; set; }
            public uint Win32VersionValue { get; set; }
            public uint SizeOfImage { get; set; }
            public uint SizeOfHeaders { get; set; }
            public uint CheckSum { get; set; }
            public ushort Subsystem { get; set; }
            public ushort DllCharacteristics { get; set; }
#if WIN32
            public uint SizeOfStackReserve { get; set; }
            public uint SizeOfStackCommit { get; set; }
            public uint SizeOfHeapReserve { get; set; }
            public uint SizeOfHeapCommit { get; set; }
#else
            public ulong SizeOfStackReserve { get; set; }
            public ulong SizeOfStackCommit { get; set; }
            public ulong SizeOfHeapReserve { get; set; }
            public ulong SizeOfHeapCommit { get; set; }
#endif
            public uint LoaderFlags { get; set; }
            public uint NumberOfRvaAndSizes { get; set; }
            public PE64DataDirectory[] DataDirectory { get; private set; }


            public void AppendToStream(BinaryWriter writer)
            {
                writer.Write(Magic);
                writer.Write(MajorLinkerVersion);
                writer.Write(MinorLinkerVersion);
                writer.Write(SizeOfCode);
                writer.Write(SizeOfInitializedData);
                writer.Write(SizeOfUninitializedData);
                writer.Write(AddressOfEntryPoint);
                writer.Write(BaseOfCode);
#if WIN32
                writer.Write(BaseOfData);
#endif
                writer.Write(ImageBase);
                writer.Write(SectionAlignment);
                writer.Write(FileAlignment);
                writer.Write(MajorOperatingSystemVersion);
                writer.Write(MinorOperatingSystemVersion);
                writer.Write(MajorImageVersion);
                writer.Write(MinorImageVersion);
                writer.Write(MajorSubsystemVersion);
                writer.Write(MinorSubsystemVersion);
                writer.Write(Win32VersionValue);
                writer.Write(SizeOfImage);
                writer.Write(SizeOfHeaders);
                writer.Write(CheckSum);
                writer.Write(Subsystem);
                writer.Write(DllCharacteristics);
                writer.Write(SizeOfStackReserve);
                writer.Write(SizeOfStackCommit);
                writer.Write(SizeOfHeapReserve);
                writer.Write(SizeOfHeapCommit);
                writer.Write(LoaderFlags);
                writer.Write(NumberOfRvaAndSizes);

                foreach (PE64DataDirectory dataDirectory in DataDirectory)
                {
                    dataDirectory.AppendToStream(writer);
                }
            }

            public static PE64OptionalHeader FromNativeStruct(IMAGE_OPTIONAL_HEADER64 nativeStruct)
            {
                PE64DataDirectory[] directories = nativeStruct.DataDirectory.Select(d => PE64DataDirectory.FromNativeStruct(d)).ToArray();

                return new PE64OptionalHeader
                {
                    Magic = nativeStruct.Magic,
                    MajorLinkerVersion = nativeStruct.MajorLinkerVersion,
                    MinorLinkerVersion = nativeStruct.MinorLinkerVersion,
                    SizeOfCode = nativeStruct.SizeOfCode,
                    SizeOfInitializedData = nativeStruct.SizeOfInitializedData,
                    SizeOfUninitializedData = nativeStruct.SizeOfUninitializedData,
                    AddressOfEntryPoint = nativeStruct.AddressOfEntryPoint,
                    BaseOfCode = nativeStruct.BaseOfCode,
#if WIN32
                    BaseOfData = nativeStruct.BaseOfData,
#endif
                    ImageBase = nativeStruct.ImageBase,
                    SectionAlignment = nativeStruct.SectionAlignment,
                    FileAlignment = nativeStruct.FileAlignment,
                    MajorOperatingSystemVersion = nativeStruct.MajorOperatingSystemVersion,
                    MinorOperatingSystemVersion = nativeStruct.MinorOperatingSystemVersion,
                    MajorImageVersion = nativeStruct.MajorImageVersion,
                    MinorImageVersion = nativeStruct.MinorImageVersion,
                    MajorSubsystemVersion = nativeStruct.MajorSubsystemVersion,
                    MinorSubsystemVersion = nativeStruct.MinorSubsystemVersion,
                    Win32VersionValue = nativeStruct.Win32VersionValue,
                    SizeOfImage = nativeStruct.SizeOfImage,
                    SizeOfHeaders = nativeStruct.SizeOfHeaders,
                    CheckSum = nativeStruct.CheckSum,
                    Subsystem = nativeStruct.Subsystem,
                    DllCharacteristics = nativeStruct.DllCharacteristics,
                    SizeOfStackReserve = nativeStruct.SizeOfStackReserve,
                    SizeOfStackCommit = nativeStruct.SizeOfStackCommit,
                    SizeOfHeapReserve = nativeStruct.SizeOfHeapReserve,
                    SizeOfHeapCommit = nativeStruct.SizeOfHeapCommit,
                    LoaderFlags = nativeStruct.LoaderFlags,
                    NumberOfRvaAndSizes = nativeStruct.NumberOfRvaAndSizes,
                    DataDirectory = directories
                };
            }

            public class PE64DataDirectory
            {
                public uint VirtualAddress { get; set; }
                public uint Size { get; set; }


                public void AppendToStream(BinaryWriter writer)
                {
                    writer.Write(VirtualAddress);
                    writer.Write(Size);
                }

                public static PE64DataDirectory FromNativeStruct(IMAGE_DATA_DIRECTORY nativeStruct)
                {
                    return new PE64DataDirectory
                    {
                        VirtualAddress = nativeStruct.VirtualAddress,
                        Size = nativeStruct.Size
                    };
                }
            }
        }           
    }
}

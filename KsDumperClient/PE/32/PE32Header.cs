using System.IO;
using System.Linq;

using static KsDumperClient.PE.NativePEStructs;

namespace KsDumperClient.PE
{
    public class PE32Header
    {
        public string Signature { get; private set; }

        public PE32FileHeader FileHeader { get; private set; }

        public PE32OptionalHeader OptionalHeader { get; private set; }


        public void AppendToStream(BinaryWriter writer)
        {
            writer.Write(Signature.ToCharArray());
            FileHeader.AppendToStream(writer);
            OptionalHeader.AppendToStream(writer);
        }

        public static PE32Header FromNativeStruct(IMAGE_NT_HEADERS32 nativeStruct)
        {
            return new PE32Header
            {
                Signature = new string(nativeStruct.Signature),
                FileHeader = PE32FileHeader.FromNativeStruct(nativeStruct.FileHeader),
                OptionalHeader = PE32OptionalHeader.FromNativeStruct(nativeStruct.OptionalHeader)
            };            
        }


        public class PE32FileHeader
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

            public static PE32FileHeader FromNativeStruct(IMAGE_FILE_HEADER nativeStruct)
            {
                return new PE32FileHeader
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
        
        public class PE32OptionalHeader
        {
            public ushort Magic { get; set; }
            public byte MajorLinkerVersion { get; set; }
            public byte MinorLinkerVersion { get; set; }
            public uint SizeOfCode { get; set; }
            public uint SizeOfInitializedData { get; set; }
            public uint SizeOfUninitializedData { get; set; }
            public uint AddressOfEntryPoint { get; set; }
            public uint BaseOfCode { get; set; }
            public uint BaseOfData { get; set; }
            public uint ImageBase { get; set; }
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
            public uint SizeOfStackReserve { get; set; }
            public uint SizeOfStackCommit { get; set; }
            public uint SizeOfHeapReserve { get; set; }
            public uint SizeOfHeapCommit { get; set; }
            public uint LoaderFlags { get; set; }
            public uint NumberOfRvaAndSizes { get; set; }
            public PE32DataDirectory[] DataDirectory { get; private set; }


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
                writer.Write(BaseOfData);
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

                foreach (PE32DataDirectory dataDirectory in DataDirectory)
                {
                    dataDirectory.AppendToStream(writer);
                }
            }

            public static PE32OptionalHeader FromNativeStruct(IMAGE_OPTIONAL_HEADER32 nativeStruct)
            {
                PE32DataDirectory[] directories = nativeStruct.DataDirectory.Select(d => PE32DataDirectory.FromNativeStruct(d)).ToArray();

                return new PE32OptionalHeader
                {
                    Magic = nativeStruct.Magic,
                    MajorLinkerVersion = nativeStruct.MajorLinkerVersion,
                    MinorLinkerVersion = nativeStruct.MinorLinkerVersion,
                    SizeOfCode = nativeStruct.SizeOfCode,
                    SizeOfInitializedData = nativeStruct.SizeOfInitializedData,
                    SizeOfUninitializedData = nativeStruct.SizeOfUninitializedData,
                    AddressOfEntryPoint = nativeStruct.AddressOfEntryPoint,
                    BaseOfCode = nativeStruct.BaseOfCode,
                    BaseOfData = nativeStruct.BaseOfData,
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

            public class PE32DataDirectory
            {
                public uint VirtualAddress { get; set; }
                public uint Size { get; set; }


                public void AppendToStream(BinaryWriter writer)
                {
                    writer.Write(VirtualAddress);
                    writer.Write(Size);
                }

                public static PE32DataDirectory FromNativeStruct(IMAGE_DATA_DIRECTORY nativeStruct)
                {
                    return new PE32DataDirectory
                    {
                        VirtualAddress = nativeStruct.VirtualAddress,
                        Size = nativeStruct.Size
                    };
                }
            }
        }           
    }
}

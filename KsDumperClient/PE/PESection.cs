using System.IO;

using static KsDumperClient.PE.NativePEStructs;

namespace KsDumperClient.PE
{
    public class PESection
    {
        public PESectionHeader Header { get; set; }

        public byte[] Content { get; set; }

        public int InitialSize { get; set; }

        public int DataSize { get; set; }

        public class PESectionHeader
        {
            public string Name { get; set; }

            public uint VirtualSize { get; set; }

            public uint VirtualAddress { get; set; }

            public uint SizeOfRawData { get; set; }

            public uint PointerToRawData { get; set; }

            public uint PointerToRelocations { get; set; }

            public uint PointerToLinenumbers { get; set; }

            public ushort NumberOfRelocations { get; set; }

            public ushort NumberOfLinenumbers { get; set; } 

            public DataSectionFlags Characteristics { get; set; }


            public void AppendToStream(BinaryWriter writer)
            {
                writer.Write(Name.ToCharArray());
                writer.Write(VirtualSize);
                writer.Write(VirtualAddress);
                writer.Write(SizeOfRawData);
                writer.Write(PointerToRawData);
                writer.Write(PointerToRelocations);
                writer.Write(PointerToLinenumbers);
                writer.Write(NumberOfRelocations);
                writer.Write(NumberOfLinenumbers);
                writer.Write((uint)Characteristics);
            }

            public static PESectionHeader FromNativeStruct(IMAGE_SECTION_HEADER nativeStruct)
            {
                return new PESectionHeader
                {
                    Name = nativeStruct.SectionName,
                    VirtualSize = nativeStruct.VirtualSize,
                    VirtualAddress = nativeStruct.VirtualAddress,
                    SizeOfRawData = nativeStruct.SizeOfRawData,
                    PointerToRawData = nativeStruct.PointerToRawData,
                    PointerToRelocations = nativeStruct.PointerToRelocations,
                    PointerToLinenumbers = nativeStruct.PointerToLinenumbers,
                    NumberOfRelocations = nativeStruct.NumberOfRelocations,
                    NumberOfLinenumbers = nativeStruct.NumberOfLinenumbers,
                    Characteristics = nativeStruct.Characteristics
                };
            }
        }
    }   
}

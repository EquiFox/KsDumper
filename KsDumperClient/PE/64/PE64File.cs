using System.IO;
using System.Runtime.InteropServices;

using static KsDumperClient.PE.NativePEStructs;

namespace KsDumperClient.PE
{
    public class PE64File : PEFile
    {
        public DOSHeader DOSHeader { get; private set; }

        public byte[] DOS_Stub { get; private set; }

        public PE64Header PEHeader { get; private set; }

        public PE64File(IMAGE_DOS_HEADER dosHeader, IMAGE_NT_HEADERS64 peHeader, byte[] dosStub)
        {
            Type = PEType.PE64;
            DOSHeader = DOSHeader.FromNativeStruct(dosHeader);
            PEHeader = PE64Header.FromNativeStruct(peHeader);
            Sections = new PESection[peHeader.FileHeader.NumberOfSections];
            DOS_Stub = dosStub;
        }

        public override void SaveToDisk(string fileName)
        {
            try
            {
                using (BinaryWriter writer = new BinaryWriter(new FileStream(fileName, FileMode.Create, FileAccess.Write)))
                {
                    DOSHeader.AppendToStream(writer);
                    writer.Write(DOS_Stub);
                    PEHeader.AppendToStream(writer);
                    AppendSections(writer);                    
                }
            }
            catch { }
        }

        public override int GetFirstSectionHeaderOffset()
        {
            return Marshal.OffsetOf<IMAGE_NT_HEADERS64>("OptionalHeader").ToInt32() +
                PEHeader.FileHeader.SizeOfOptionalHeader;
        }

        public override void AlignSectionHeaders()
        {
            int newFileSize = DOSHeader.e_lfanew + 0x4 +
                Marshal.SizeOf<IMAGE_FILE_HEADER>() +
                PEHeader.FileHeader.SizeOfOptionalHeader +
                    (PEHeader.FileHeader.NumberOfSections * Marshal.SizeOf<IMAGE_SECTION_HEADER>());

            OrderSectionsBy(s => s.Header.PointerToRawData);

            for (int i = 0; i < Sections.Length; i++)
            {
                Sections[i].Header.VirtualAddress = AlignValue(Sections[i].Header.VirtualAddress, PEHeader.OptionalHeader.SectionAlignment);
                Sections[i].Header.VirtualSize = AlignValue(Sections[i].Header.VirtualSize, PEHeader.OptionalHeader.SectionAlignment);
                Sections[i].Header.PointerToRawData = AlignValue((uint)newFileSize, PEHeader.OptionalHeader.FileAlignment);
                Sections[i].Header.SizeOfRawData = AlignValue((uint)Sections[i].DataSize, PEHeader.OptionalHeader.FileAlignment);

                newFileSize = (int)(Sections[i].Header.PointerToRawData + Sections[i].Header.SizeOfRawData);
            }

            OrderSectionsBy(s => s.Header.VirtualAddress);
        }

        public override void FixPEHeader()
        {
            PEHeader.OptionalHeader.DataDirectory[IMAGE_DIRECTORY_ENTRY_BOUND_IMPORT].VirtualAddress = 0;
            PEHeader.OptionalHeader.DataDirectory[IMAGE_DIRECTORY_ENTRY_BOUND_IMPORT].Size = 0;

            for (uint i = PEHeader.OptionalHeader.NumberOfRvaAndSizes; i < IMAGE_NUMBEROF_DIRECTORY_ENTRIES; i++)
            {
                PEHeader.OptionalHeader.DataDirectory[i].VirtualAddress = 0;
                PEHeader.OptionalHeader.DataDirectory[i].Size = 0;
            }

            PEHeader.OptionalHeader.NumberOfRvaAndSizes = IMAGE_NUMBEROF_DIRECTORY_ENTRIES;
            PEHeader.FileHeader.SizeOfOptionalHeader = (ushort)Marshal.SizeOf<IMAGE_OPTIONAL_HEADER64>();
            FixSizeOfImage();

            int size = DOSHeader.e_lfanew + 0x4 + Marshal.SizeOf<IMAGE_FILE_HEADER>();
            PEHeader.OptionalHeader.SizeOfHeaders = AlignValue((uint)(size + PEHeader.FileHeader.SizeOfOptionalHeader + (PEHeader.FileHeader.NumberOfSections * Marshal.SizeOf<IMAGE_SECTION_HEADER>())), PEHeader.OptionalHeader.FileAlignment);

            RemoveIatDirectory();
        }

        private uint AlignValue(uint value, uint alignment)
        {
            return ((value + alignment - 1) / alignment) * alignment;
        }

        private void FixSizeOfImage()
        {
            uint lastSize = 0;

            for (int i = 0; i < PEHeader.FileHeader.NumberOfSections; i++)
            {
                if (Sections[i].Header.VirtualAddress + Sections[i].Header.VirtualSize > lastSize)
                {
                    lastSize = Sections[i].Header.VirtualAddress + Sections[i].Header.VirtualSize;
                }
            }
            PEHeader.OptionalHeader.SizeOfImage = lastSize;
        }

        private void RemoveIatDirectory()
        {
            uint iatDataAddress = PEHeader.OptionalHeader.DataDirectory[IMAGE_DIRECTORY_ENTRY_IAT].VirtualAddress;

            PEHeader.OptionalHeader.DataDirectory[IMAGE_DIRECTORY_ENTRY_IAT].VirtualAddress = 0;
            PEHeader.OptionalHeader.DataDirectory[IMAGE_DIRECTORY_ENTRY_IAT].Size = 0;

            if (iatDataAddress != 0)
            {
                for (int i = 0; i < PEHeader.FileHeader.NumberOfSections; i++)
                {
                    if (Sections[i].Header.VirtualAddress <= iatDataAddress &&
                        Sections[i].Header.VirtualAddress + Sections[i].Header.VirtualSize > iatDataAddress)
                    {
                        Sections[i].Header.Characteristics |= DataSectionFlags.MemoryRead | DataSectionFlags.MemoryWrite;
                    }
                }
            }
        }
    }
}

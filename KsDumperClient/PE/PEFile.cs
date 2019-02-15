using System;
using System.IO;
using System.Linq;

namespace KsDumperClient.PE
{
    public abstract class PEFile
    {
        public PEType Type { get; protected set; }

        public PESection[] Sections { get; protected set; }


        public abstract int GetFirstSectionHeaderOffset();

        public abstract void AlignSectionHeaders();

        public abstract void FixPEHeader();

        public abstract void SaveToDisk(string fileName);

        protected void AppendSections(BinaryWriter writer)
        {
            foreach (var sectionHeader in Sections.Select(s => s.Header))
            {
                sectionHeader.AppendToStream(writer);
            }

            foreach (var section in Sections)
            {
                if (section.Header.PointerToRawData > 0)
                {
                    if (section.Header.PointerToRawData > writer.BaseStream.Position)
                    {
                        long prePaddingSize = section.Header.PointerToRawData - writer.BaseStream.Position;
                        writer.Write(new byte[prePaddingSize]);
                    }

                    if (section.DataSize > 0)
                    {
                        writer.Write(section.Content);

                        if (section.DataSize < section.Header.SizeOfRawData)
                        {
                            long postPaddingSize = section.Header.SizeOfRawData - section.DataSize;
                            writer.Write(new byte[postPaddingSize]);
                        }
                    }
                }
            }
        }

        protected void OrderSectionsBy(Func<PESection, uint> orderFunction)
        {
            Sections = Sections.OrderBy(orderFunction).ToArray();
        }

        public enum PEType
        {
            PE32,
            PE64
        }
    }
}

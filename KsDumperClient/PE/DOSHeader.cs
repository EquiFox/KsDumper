using System.IO;

using static KsDumperClient.PE.NativePEStructs;

namespace KsDumperClient.PE
{
    public class DOSHeader
    {
        public string e_magic { get; set; }
        public ushort e_cblp { get; set; }
        public ushort e_cp { get; set; }
        public ushort e_crlc { get; set; }
        public ushort e_cparhdr { get; set; }
        public ushort e_minalloc { get; set; }
        public ushort e_maxalloc { get; set; }
        public ushort e_ss { get; set; }
        public ushort e_sp { get; set; }
        public ushort e_csum { get; set; }
        public ushort e_ip { get; set; }
        public ushort e_cs { get; set; }
        public ushort e_lfarlc { get; set; }
        public ushort e_ovno { get; set; }
        public ushort[] e_res1 { get; set; }
        public ushort e_oemid { get; set; }
        public ushort e_oeminfo { get; set; }
        public ushort[] e_res2 { get; set; }
        public int e_lfanew { get; set; }

        public void AppendToStream(BinaryWriter writer)
        {
            writer.Write(e_magic.ToCharArray());
            writer.Write(e_cblp);
            writer.Write(e_cp);
            writer.Write(e_crlc);
            writer.Write(e_cparhdr);
            writer.Write(e_minalloc);
            writer.Write(e_maxalloc);
            writer.Write(e_ss);
            writer.Write(e_sp);
            writer.Write(e_csum);
            writer.Write(e_ip);
            writer.Write(e_cs);
            writer.Write(e_lfarlc);
            writer.Write(e_ovno);

            for (int i = 0; i < e_res1.Length; i++)
            {
                writer.Write(e_res1[i]);
            }
            writer.Write(e_oemid);
            writer.Write(e_oeminfo);

            for (int i = 0; i < e_res2.Length; i++)
            {
                writer.Write(e_res2[i]);
            }
            writer.Write(e_lfanew);
        }

        public static DOSHeader FromNativeStruct(IMAGE_DOS_HEADER nativeStruct)
        {
            return new DOSHeader
            {
                e_magic = new string(nativeStruct.e_magic),
                e_cblp = nativeStruct.e_cblp,
                e_cp = nativeStruct.e_cp,
                e_crlc = nativeStruct.e_crlc,
                e_cparhdr = nativeStruct.e_cparhdr,
                e_minalloc = nativeStruct.e_minalloc,
                e_maxalloc = nativeStruct.e_maxalloc,
                e_ss = nativeStruct.e_ss,
                e_sp = nativeStruct.e_sp,
                e_csum = nativeStruct.e_csum,
                e_ip = nativeStruct.e_ip,
                e_cs = nativeStruct.e_cs,
                e_lfarlc = nativeStruct.e_lfarlc,
                e_ovno = nativeStruct.e_ovno,
                e_res1 = nativeStruct.e_res1,
                e_oemid = nativeStruct.e_oemid,
                e_oeminfo = nativeStruct.e_oeminfo,
                e_res2 = nativeStruct.e_res2,
                e_lfanew = nativeStruct.e_lfanew
            };
        }
    }
}

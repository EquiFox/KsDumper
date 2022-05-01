using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using KsDumperClient.Driver;
using KsDumperClient.PE;
using KsDumperClient.Utility;
using System.Threading;

namespace KsDumperClient
{
    public partial class Dumper : Form
    {
        #region win api imports
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool TerminateProcess(IntPtr hProcess, uint uExitCode);

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct TOKEN_PRIVILEGES
        {
            public int PrivilegeCount;
            public long Luid;
            public int Attributes;
        }

        private const int SE_PRIVILEGE_ENABLED = 0x00000002;
        private const int TOKEN_ADJUST_PRIVILEGES = 0x00000020;
        private const int TOKEN_QUERY = 0x00000008;

        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int OpenProcessToken(int ProcessHandle, int DesiredAccess, ref int tokenhandle);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        private static extern int GetCurrentProcess();

        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int LookupPrivilegeValue(string lpsystemname, string lpname, ref long lpLuid);

        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int AdjustTokenPrivileges(int tokenhandle, int disableprivs, ref TOKEN_PRIVILEGES Newstate, int bufferlength, int PreivousState, int Returnlength);

        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int GetSecurityInfo(int HANDLE, int SE_OBJECT_TYPE, int SECURITY_INFORMATION, int psidOwner, int psidGroup, out IntPtr pDACL, IntPtr pSACL, out IntPtr pSecurityDescriptor);

        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int SetSecurityInfo(int HANDLE, int SE_OBJECT_TYPE, int SECURITY_INFORMATION, int psidOwner, int psidGroup, IntPtr pDACL, IntPtr pSACL);


        [DllImport("ntdll.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool ZwSuspendProcess(IntPtr hProcess);

        [DllImport("ntdll.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool ZwResumeProcess(IntPtr hProcess);

        public enum ProcessAccess : int
        {
            /// <summary>Specifies all possible access flags for the process object.</summary>
            AllAccess = CreateThread | DuplicateHandle | QueryInformation | SetInformation | Terminate | VMOperation | VMRead | VMWrite | Synchronize,
            /// <summary>Enables usage of the process handle in the CreateRemoteThread function to create a thread in the process.</summary>
            CreateThread = 0x2,
            /// <summary>Enables usage of the process handle as either the source or target process in the DuplicateHandle function to duplicate a handle.</summary>
            DuplicateHandle = 0x40,
            /// <summary>Enables usage of the process handle in the GetExitCodeProcess and GetPriorityClass functions to read information from the process object.</summary>
            QueryInformation = 0x400,
            /// <summary>Enables usage of the process handle in the SetPriorityClass function to set the priority class of the process.</summary>
            SetInformation = 0x200,
            /// <summary>Enables usage of the process handle in the TerminateProcess function to terminate the process.</summary>
            Terminate = 0x1,
            /// <summary>Enables usage of the process handle in the VirtualProtectEx and WriteProcessMemory functions to modify the virtual memory of the process.</summary>
            VMOperation = 0x8,
            /// <summary>Enables usage of the process handle in the ReadProcessMemory function to' read from the virtual memory of the process.</summary>
            VMRead = 0x10,
            /// <summary>Enables usage of the process handle in the WriteProcessMemory function to write to the virtual memory of the process.</summary>
            VMWrite = 0x20,
            /// <summary>Enables usage of the process handle in any of the wait functions to wait for the process to terminate.</summary>
            Synchronize = 0x100000
        }




        [StructLayout(LayoutKind.Sequential)]
        public struct SYSTEM_INFO
        {
            public uint dwOemId;
            public uint dwPageSize;
            public uint lpMinimumApplicationAddress;
            public uint lpMaximumApplicationAddress;
            public uint dwActiveProcessorMask;
            public uint dwNumberOfProcessors;
            public uint dwProcessorType;
            public uint dwAllocationGranularity;
            public uint dwProcessorLevel;
            public uint dwProcessorRevision;
        }

        [DllImport("kernel32")]
        public static extern void GetSystemInfo(ref SYSTEM_INFO pSI);

        [DllImport("kernel32.dll")]
        static extern IntPtr OpenProcess(UInt32 dwDesiredAccess, Int32 bInheritHandle, UInt32 dwProcessId);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool CloseHandle(IntPtr hObject);


        private const uint PROCESS_TERMINATE = 0x0001;
        private const uint PROCESS_CREATE_THREAD = 0x0002;
        private const uint PROCESS_SET_SESSIONID = 0x0004;
        private const uint PROCESS_VM_OPERATION = 0x0008;
        private const uint PROCESS_VM_READ = 0x0010;
        private const uint PROCESS_VM_WRITE = 0x0020;
        private const uint PROCESS_DUP_HANDLE = 0x0040;
        private const uint PROCESS_CREATE_PROCESS = 0x0080;
        private const uint PROCESS_SET_QUOTA = 0x0100;
        private const uint PROCESS_SET_INFORMATION = 0x0200;
        private const uint PROCESS_QUERY_INFORMATION = 0x0400;

        //inner enum used only internally
        [Flags]
        private enum SnapshotFlags : uint
        {
            HeapList = 0x00000001,
            Process = 0x00000002,
            Thread = 0x00000004,
            Module = 0x00000008,
            Module32 = 0x00000010,
            Inherit = 0x80000000,
            All = 0x0000001F
        }
        //inner struct used only internally
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private struct PROCESSENTRY32
        {
            const int MAX_PATH = 260;
            internal UInt32 dwSize;
            internal UInt32 cntUsage;
            internal UInt32 th32ProcessID;
            internal IntPtr th32DefaultHeapID;
            internal UInt32 th32ModuleID;
            internal UInt32 cntThreads;
            internal UInt32 th32ParentProcessID;
            internal Int32 pcPriClassBase;
            internal UInt32 dwFlags;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_PATH)]
            internal string szExeFile;
        }

        [DllImport("kernel32", SetLastError = true, CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        static extern IntPtr CreateToolhelp32Snapshot([In] UInt32 dwFlags, [In] UInt32 th32ProcessID);

        [DllImport("kernel32", SetLastError = true, CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        static extern bool Process32First([In] IntPtr hSnapshot, ref PROCESSENTRY32 lppe);

        [DllImport("kernel32", SetLastError = true, CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        static extern bool Process32Next([In] IntPtr hSnapshot, ref PROCESSENTRY32 lppe);


        [DllImport("ntdll.dll", SetLastError = true)]
        static extern int NtQueryInformationProcess(IntPtr processHandle,
           int processInformationClass, ref PROCESS_BASIC_INFORMATION processInformation, uint processInformationLength,
           out int returnLength);

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct PROCESS_BASIC_INFORMATION
        {
            public int ExitStatus;
            public int PebBaseAddress;
            public int AffinityMask;
            public int BasePriority;
            public int UniqueProcessId;
            public int InheritedFromUniqueProcessId;

            public int Size
            {
                get { return (6 * 4); }
            }
        }
        #endregion

        private readonly DriverInterface driver;
        private readonly ProcessDumper dumper;

        public Dumper()
        {
            InitializeComponent();

            driver = new DriverInterface("\\\\.\\KsDumper");
            dumper = new ProcessDumper(driver);
            LoadProcessList();
        }

        private void Dumper_Load(object sender, EventArgs e)
        {
            Logger.OnLog += Logger_OnLog;
            Logger.Log("KsDumper v1.1 - By EquiFox");
        }

        private void LoadProcessList()
        {
            if (driver.HasValidHandle())
            {
                if (driver.GetProcessSummaryList(out ProcessSummary[] result))
                {
                    processList.LoadProcesses(result);
                }
                else
                {
                    MessageBox.Show("Unable to retrieve process list !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private bool DumpProcess(ProcessSummary process)
        {
            if (driver.HasValidHandle())
            {
                Logger.Log("Valid driver handle open");
                bool sucess = false;
                Task.Run(() =>
                {
                    Logger.Log("Dumping process...");
                    sucess = dumper.DumpProcess(process, out PEFile peFile);
                    if (sucess)
                    {
                        Logger.Log("Sucess!");
                        Invoke(new Action(() =>
                        {
                            using (SaveFileDialog sfd = new SaveFileDialog())
                            {
                                sfd.FileName = process.ProcessName.Replace(".exe", "_dump.exe");
                                sfd.Filter = "Executable File (.exe)|*.exe";

                                if (sfd.ShowDialog() == DialogResult.OK)
                                {
                                    peFile.SaveToDisk(sfd.FileName);
                                    Logger.Log("Saved at '{0}' !", sfd.FileName);
                                }
                            }
                        }));

                        Logger.Log(process.ProcessName + "  Killed");
                        KillProcess(process.ProcessId);
                    }
                    else
                    {
                        Logger.Log("Failure");
                        Invoke(new Action(() =>
                        {
                            MessageBox.Show("Unable to dump target process !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }));
                    }
                });
                return sucess;
            }
            else
            {
                MessageBox.Show("Unable to communicate with driver ! Make sure it is loaded.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return false;
        }

        private bool DumpProcess(Process process)
        {
            if (driver.HasValidHandle())
            {
                Logger.Log("Valid driver handle open");
                bool sucess = false;
                Logger.Log("Dumping process...");
                sucess = dumper.DumpProcess(process, out PEFile peFile);
                if (sucess)
                {
                    Logger.Log("Sucess!");
                    Invoke(new Action(() =>
                    {
                        using (SaveFileDialog sfd = new SaveFileDialog())
                        {
                            sfd.FileName = process.ProcessName + "_dump.exe";
                            sfd.Filter = "Executable File (.exe)|*.exe";

                            if (sfd.ShowDialog() == DialogResult.OK)
                            {
                                peFile.SaveToDisk(sfd.FileName);
                                Logger.Log("Saved at '{0}' !", sfd.FileName);
                            }
                        }
                    }));

                    Logger.Log(process.ProcessName + "  Killed");
                    KillProcess(process.Id);
                }
                else
                {
                    Logger.Log("Failure");
                    Logger.Log(process.ProcessName + "  Killed");
                    KillProcess(process.Id);
                    Invoke(new Action(() =>
                    {
                        MessageBox.Show("Unable to dump target process !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }));
                }
                return sucess;
            }
            else
            {
                MessageBox.Show("Unable to communicate with driver ! Make sure it is loaded.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            Logger.Log(process.ProcessName + "  Killed");
            KillProcess(process.Id);
            return false;
        }

        private void dumpMainModuleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ProcessSummary targetProcess = processList.SelectedItems[0].Tag as ProcessSummary;
            DumpProcess(targetProcess);
        }

        private void Logger_OnLog(string message)
        {
            logsTextBox.Invoke(new Action(() =>
            {
                logsTextBox.AppendText(message);
                logsTextBox.Update();
            }));
        }

        private void refreshMenuBtn_Click(object sender, EventArgs e)
        {
            LoadProcessList();
        }

        private void hideSystemProcessMenuBtn_Click(object sender, EventArgs e)
        {
            if (!processList.SystemProcessesHidden)
            {
                processList.HideSystemProcesses();
                hideSystemProcessMenuBtn.Text = "Show System Processes";
            }
            else
            {
                processList.ShowSystemProcesses();
                hideSystemProcessMenuBtn.Text = "Hide System Processes";
            }
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            e.Cancel = processList.SelectedItems.Count == 0;
        }

        private void logsTextBox_TextChanged(object sender, EventArgs e)
        {
            logsTextBox.SelectionStart = logsTextBox.Text.Length;
            logsTextBox.ScrollToCaret();
        }

        private void openInExplorerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ProcessSummary targetProcess = processList.SelectedItems[0].Tag as ProcessSummary;
            Process.Start("explorer.exe", Path.GetDirectoryName(targetProcess.MainModuleFileName));
        }

        private void suspendProcessToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ProcessSummary targetProcess = processList.SelectedItems[0].Tag as ProcessSummary;
            SuspendProcess(targetProcess.ProcessId);
        }

        private void KillProcess(int processId)
        {
            UInt32 procId = (UInt32)processId;
            IntPtr hProcess = OpenProcess(PROCESS_TERMINATE | PROCESS_QUERY_INFORMATION | PROCESS_VM_OPERATION | PROCESS_VM_WRITE | PROCESS_VM_READ, 0, procId);

            if (hProcess == IntPtr.Zero)
            {
                IntPtr pDACL, pSecDesc;

                GetSecurityInfo((int)Process.GetCurrentProcess().Handle, /*SE_KERNEL_OBJECT*/ 6, /*DACL_SECURITY_INFORMATION*/ 4, 0, 0, out pDACL, IntPtr.Zero, out pSecDesc);
                hProcess = OpenProcess(0x40000, 0, procId);
                SetSecurityInfo((int)hProcess, /*SE_KERNEL_OBJECT*/ 6, /*DACL_SECURITY_INFORMATION*/ 4 | /*UNPROTECTED_DACL_SECURITY_INFORMATION*/ 0x20000000, 0, 0, pDACL, IntPtr.Zero);
                CloseHandle(hProcess);
                hProcess = OpenProcess(PROCESS_QUERY_INFORMATION | PROCESS_VM_OPERATION | PROCESS_VM_WRITE | PROCESS_VM_READ, 0, procId);
            }

            try
            {
                TerminateProcess(hProcess, 0);
            }
            catch
            {
            }
            CloseHandle(hProcess);
        }

        private void SuspendProcess(int processId)
        {
            UInt32 procId = (UInt32)processId;
            IntPtr hProcess = OpenProcess(0x800, 0, procId);

            if (hProcess == IntPtr.Zero)
            {
                IntPtr pDACL, pSecDesc;

                GetSecurityInfo((int)Process.GetCurrentProcess().Handle, /*SE_KERNEL_OBJECT*/ 6, /*DACL_SECURITY_INFORMATION*/ 4, 0, 0, out pDACL, IntPtr.Zero, out pSecDesc);
                hProcess = OpenProcess(0x40000, 0, procId);
                SetSecurityInfo((int)hProcess, /*SE_KERNEL_OBJECT*/ 6, /*DACL_SECURITY_INFORMATION*/ 4 | /*UNPROTECTED_DACL_SECURITY_INFORMATION*/ 0x20000000, 0, 0, pDACL, IntPtr.Zero);
                CloseHandle(hProcess);
                hProcess = OpenProcess(PROCESS_QUERY_INFORMATION | PROCESS_VM_OPERATION | PROCESS_VM_WRITE | PROCESS_VM_READ, 0, procId);
            }

            try
            {
                ZwSuspendProcess(hProcess);
            }
            catch
            {
            }
            CloseHandle(hProcess);
        }

        private void ResumeProcess(int processId)
        {
            UInt32 procId = (UInt32)processId;
            IntPtr hProcess = OpenProcess(0x800, 0, procId);

            if (hProcess == IntPtr.Zero)
            {
                IntPtr pDACL, pSecDesc;

                GetSecurityInfo((int)Process.GetCurrentProcess().Handle, /*SE_KERNEL_OBJECT*/ 6, /*DACL_SECURITY_INFORMATION*/ 4, 0, 0, out pDACL, IntPtr.Zero, out pSecDesc);
                hProcess = OpenProcess(0x40000, 0, procId);
                SetSecurityInfo((int)hProcess, /*SE_KERNEL_OBJECT*/ 6, /*DACL_SECURITY_INFORMATION*/ 4 | /*UNPROTECTED_DACL_SECURITY_INFORMATION*/ 0x20000000, 0, 0, pDACL, IntPtr.Zero);
                CloseHandle(hProcess);
                hProcess = OpenProcess(PROCESS_QUERY_INFORMATION | PROCESS_VM_OPERATION | PROCESS_VM_WRITE | PROCESS_VM_READ, 0, procId);
            }

            try
            {
                ZwResumeProcess(hProcess);
            }
            catch
            {
            }
            CloseHandle(hProcess);
        }

        private void resumeProcessToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ProcessSummary targetProcess = processList.SelectedItems[0].Tag as ProcessSummary;
            ResumeProcess(targetProcess.ProcessId);

        }

        private void killProcessToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ProcessSummary targetProcess = processList.SelectedItems[0].Tag as ProcessSummary;
            KillProcess(targetProcess.ProcessId);
        }



        System.Windows.Forms.Timer t;

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            if (toolStripButton1.Checked == true)
            {
                if (t == null)
                {
                    t = new System.Windows.Forms.Timer();
                    t.Tick += T_Tick;
                    t.Interval = 100;
                    t.Start();
                }
                else
                {
                    t.Interval = 100;
                    t.Start();
                }
            }
            else
            {
                t.Stop();
            }

        }

        private void T_Tick(object sender, EventArgs e)
        {
            LoadProcessList();
        }

        private void ClearLog()
        {
            logsTextBox.Clear();
        }

        private void StartAndDumpFile(string dumpFile)
        {
            Logger.Log(Path.GetFileName(dumpFile) + "  Started");
            Process process = Process.Start(dumpFile);            
            //Thread.Sleep(750);
            process.WaitForInputIdle();

            //while (processSummary == null)
            //{
            //    Thread.Sleep(250);
            //    processSummary = ProcessSummary.ProcessSummaryFromID(this.driver, process.ProcessName);
            //    Logger.Log("Waiting...");
            //}

            SuspendProcess(process.Id);
            Logger.Log("Suspending process...");


            if (DumpProcess(process))
            {
                Logger.Log(Path.GetFileName(dumpFile) + "  Dumped");
                //KillProcess(process.Id);
                //Logger.Log(Path.GetFileName(dumpFile) + "  Killed");
            }
            else
            {
                Logger.Log("process dump failed");
                //KillProcess(process.Id);
                //Logger.Log(Path.GetFileName(dumpFile) + "  Killed");
            }

        }

        private void fileDumpBtn_Click(object sender, EventArgs e)
        {
            ClearLog();
            Logger.Log("KsDumper v1.1 - By EquiFox");

            OpenFileDialog openFileDialog = new OpenFileDialog();
            {
                openFileDialog.Filter = "Executable File (.exe)|*.exe";
                openFileDialog.Title = "File to dump";
                openFileDialog.RestoreDirectory = true;
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string dumpFile = openFileDialog.FileName;
                    StartAndDumpFile(dumpFile);
                }
            }
        }
    }
}

using System;
using System.Collections;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace KsDumperClient.Utility
{
    public class ProcessListView : ListView
    {
        public bool SystemProcessesHidden { get; private set; } = true;

        private int sortColumnIndex = 1;
        private ProcessSummary[] processCache;

        public ProcessListView()
        {
            DoubleBuffered = true;
            Sorting = SortOrder.Ascending;
        }

        public void LoadProcesses(ProcessSummary[] processSummaries)
        {
            processCache = processSummaries;
            ReloadItems();
        }

        public void ShowSystemProcesses()
        {
            SystemProcessesHidden = false;
            ReloadItems();
        }

        public void HideSystemProcesses()
        {
            SystemProcessesHidden = true;
            ReloadItems();
        }

        private void ReloadItems()
        {
            Items.Clear();

            string systemRootFolder = Environment.GetFolderPath(Environment.SpecialFolder.Windows).ToLower();

            foreach (ProcessSummary processSummary in processCache)
            {
                if (SystemProcessesHidden &&
                    (processSummary.MainModuleFileName.ToLower().StartsWith(systemRootFolder) ||
                    processSummary.MainModuleFileName.StartsWith(@"\")))
                {
                    continue;
                }

                ListViewItem lvi = new ListViewItem(processSummary.ProcessId.ToString());
                lvi.SubItems.Add(Path.GetFileName(processSummary.MainModuleFileName));
                lvi.SubItems.Add(processSummary.MainModuleFileName);
                lvi.SubItems.Add(string.Format("0x{0:x8}", processSummary.MainModuleBase));
                lvi.SubItems.Add(string.Format("0x{0:x8}", processSummary.MainModuleEntryPoint));
                lvi.SubItems.Add(string.Format("0x{0:x4}", processSummary.MainModuleImageSize));
                lvi.SubItems.Add(processSummary.IsWOW64 ? "x86" : "x64");
                lvi.Tag = processSummary;

                Items.Add(lvi);
            }

            ListViewItemSorter = new ProcessListViewItemComparer(sortColumnIndex, Sorting);
            Sort();
        }

        protected override void OnColumnClick(ColumnClickEventArgs e)
        {
            if (e.Column != sortColumnIndex)
            {
                sortColumnIndex = e.Column;
                Sorting = SortOrder.Ascending;
            }
            else
            {
                if (Sorting == SortOrder.Ascending)
                {
                    Sorting = SortOrder.Descending;
                }
                else
                {
                    Sorting = SortOrder.Ascending;
                }
            }

            ListViewItemSorter = new ProcessListViewItemComparer(e.Column, Sorting);
            Sort();
        }

        private class ProcessListViewItemComparer : IComparer
        {
            private readonly int columnIndex;
            private readonly SortOrder sortOrder;

            public ProcessListViewItemComparer(int columnIndex, SortOrder sortOrder)
            {
                this.columnIndex = columnIndex;
                this.sortOrder = sortOrder;
            }

            public int Compare(object x, object y)
            {
                if ((x is ListViewItem) && (y is ListViewItem))
                {
                    ProcessSummary p1 = ((ListViewItem)x).Tag as ProcessSummary;
                    ProcessSummary p2 = ((ListViewItem)y).Tag as ProcessSummary;

                    if (!(p1 == null || p2 == null))
                    {
                        int result = 0;

                        switch (columnIndex)
                        {
                            case 0:
                                result = p1.ProcessId.CompareTo(p2.ProcessId);
                                break;
                            case 1:
                                result = p1.ProcessName.CompareTo(p2.ProcessName);
                                break;
                            case 2:
                                result = p1.MainModuleFileName.CompareTo(p2.MainModuleFileName);
                                break;
                            case 3:
                                result = p1.MainModuleBase.CompareTo(p2.MainModuleBase);
                                break;
                            case 4:
                                result = p1.MainModuleEntryPoint.CompareTo(p2.MainModuleEntryPoint);
                                break;
                            case 5:
                                result = p1.MainModuleImageSize.CompareTo(p2.MainModuleImageSize);
                                break;
                            case 6:
                                result = p1.IsWOW64.CompareTo(p2.IsWOW64);
                                break;
                        }

                        if (sortOrder == SortOrder.Descending)
                        {
                            result = -result;
                        }
                        return result;
                    }
                }
                return 0;
            }
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x1)
            {
                SetWindowTheme(Handle, "Explorer", null);
            }
            base.WndProc(ref m);
        }

        [DllImport("uxtheme.dll", CharSet = CharSet.Unicode)]
        private extern static int SetWindowTheme(IntPtr hWnd, string pszSubAppName, string pszSubIdList);       
    }
}

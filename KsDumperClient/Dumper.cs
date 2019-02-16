using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using KsDumperClient.Driver;
using KsDumperClient.PE;
using KsDumperClient.Utility;

namespace KsDumperClient
{
    public partial class Dumper : Form
    {
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

        private void dumpMainModuleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (driver.HasValidHandle())
            {
                ProcessSummary targetProcess = processList.SelectedItems[0].Tag as ProcessSummary;

                Task.Run(() =>
                {

                    if (dumper.DumpProcess(targetProcess, out PEFile peFile))
                    {
                        Invoke(new Action(() =>
                        {
                            using (SaveFileDialog sfd = new SaveFileDialog())
                            {
                                sfd.FileName = targetProcess.ProcessName.Replace(".exe", "_dump.exe");
                                sfd.Filter = "Executable File (.exe)|*.exe";

                                if (sfd.ShowDialog() == DialogResult.OK)
                                {
                                    peFile.SaveToDisk(sfd.FileName);
                                    Logger.Log("Saved at '{0}' !", sfd.FileName);
                                }
                            }
                        }));
                    }
                    else
                    {
                        Invoke(new Action(() =>
                        {
                            MessageBox.Show("Unable to dump target process !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }));
                    }
                });
            }
            else
            {
                MessageBox.Show("Unable to communicate with driver ! Make sure it is loaded.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Logger_OnLog(string message)
        {
            logsTextBox.Invoke(new Action(() => logsTextBox.AppendText(message)));
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
    }
}

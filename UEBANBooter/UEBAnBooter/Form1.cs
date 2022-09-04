using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;

namespace UEBAnBooter
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            checkBox1.Checked = Properties.Settings.Default.check1;
            checkBox2.Checked = Properties.Settings.Default.check2;
        }
        string[] bootentry = new string[1];
        string bcdedit;
        string output;
        private void Form1_Load(object sender, EventArgs e)
        {
            bcdedit = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) + @"\bcdedit.exe";
            if (!File.Exists(bcdedit))
            {
                using (FileStream fs = new FileStream(bcdedit, FileMode.Create, FileAccess.Write))
                {
                    fs.Write(Properties.Resources.bcdedit, 0, Properties.Resources.bcdedit.Length);
                }
            }
            startBCDedit("/enum firmware");
            // richTextBox1.AppendText(output);

            int bootI = 0;

            string[] mass = output.Split('\n');
            for (int i = 0; i < mass.Length; i++)
            {
                if (mass[i].Contains("identifier") && !mass[i].Contains("identifier              {fwbootmgr}"))
                {
                    string a = mass[i];

                    a = a.Replace("identifier", "");
                    a = a.Replace(" ", "");

                    int istart = a.IndexOf("{") + "{".Length;
                    string s2 = a.Substring(istart, a.IndexOf("}") - istart);
                    Array.Resize(ref bootentry, bootentry.Length + 1);
                    bootentry[bootI] = s2;
                    bootI++;
                }
                if (mass[i].Contains("description"))
                {
                    string a = mass[i];
                    a = a.Replace("description", "");
                    a = a.Replace("             ", "");
                    listBox1.Items.Add(a);
                }
            }
        }
        DialogResult result;
        private void listBox1_Click(object sender, EventArgs e)
        {
            //Text = listBox1.SelectedIndex.ToString();
            
            if (checkBox1.Checked) { result = MessageBox.Show(listBox1.SelectedItem.ToString(), "Boot into", MessageBoxButtons.YesNo, MessageBoxIcon.Question); }
            if (result == DialogResult.Yes || !checkBox1.Checked)
            {
                if (listBox1.SelectedIndex >= 0)
                {
                    //MessageBox.Show(listBox1.SelectedItem.ToString() + "\n" + bootentry[listBox1.SelectedIndex], "Загрузиться в");
                    //bcdedit.exe /set {fwbootmgr} bootsequence {8a391ab2-e167-11ea-81d7-806e6f6e6963} /addfirst
                    //shutdown /r /t 0
                    startBCDedit("/set {fwbootmgr} bootsequence {" + bootentry[listBox1.SelectedIndex] + "} /addfirst");

                    if (checkBox2.Checked == true)
                    {
                        ProcessStartInfo shut = new ProcessStartInfo("shutdown", "/r /t 0");
                        shut.UseShellExecute = false;
                        Process.Start(shut);
                    }
                }
            }
        }
        void startBCDedit(string args)
        {
            Process process = new Process();
            process.StartInfo.FileName = bcdedit;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.Arguments = args;
            process.Start();
            output = process.StandardOutput.ReadToEnd();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                Properties.Settings.Default.check1 = true;
            }
            else { Properties.Settings.Default.check1 = false; }
            Properties.Settings.Default.Save();
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked)
            {
                Properties.Settings.Default.check2 = true;
            }
            else { Properties.Settings.Default.check2 = false; }
            Properties.Settings.Default.Save();
        }
    }
}

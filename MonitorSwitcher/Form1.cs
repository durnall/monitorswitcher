using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MonitorSwitcher
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            UpdateControlsFromRegStartUp();
        }

        private void UpdateControlsFromRegStartUp()
        {
            runOnStartupToolStripMenuItem.Click -= UpdateStartupRegistry;
            checkBox1.CheckedChanged -= UpdateStartupRegistry;

            runOnStartupToolStripMenuItem.Checked = IsApplicationInStartup();
            checkBox1.Checked = runOnStartupToolStripMenuItem.Checked;

            runOnStartupToolStripMenuItem.Click += UpdateStartupRegistry;
            checkBox1.CheckedChanged += UpdateStartupRegistry;
        }

        private void notifyIcon1_DoubleClick(object sender, EventArgs e)
        {
            Show();
            this.WindowState = FormWindowState.Normal;
            notifyIcon1.Visible = false;
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            //if the form is minimized  
            //hide it from the task bar  
            //and show the system tray icon (represented by the NotifyIcon control)  
            if (this.WindowState == FormWindowState.Minimized)
            {
                Hide();
                notifyIcon1.Visible = true;
            }
        }

        private void SetDisplayMode(DisplayMode mode)
        {
            var proc = new Process();
            proc.StartInfo.FileName = Path.Combine(Environment.SystemDirectory, "DisplaySwitch.exe");
            switch (mode)
            {
                case DisplayMode.External:
                    proc.StartInfo.Arguments = "/external";
                    break;
                case DisplayMode.Internal:
                    proc.StartInfo.Arguments = "/internal";
                    break;
                case DisplayMode.Extend:
                    proc.StartInfo.Arguments = "/extend";
                    break;
                case DisplayMode.Duplicate:
                    proc.StartInfo.Arguments = "/clone";
                    break;
            }
            proc.Start();
        }

        enum DisplayMode
        {
            Internal,
            External,
            Extend,
            Duplicate
        }

        private void SetDisplayModeExternal(object sender, EventArgs e)
        {
            SetDisplayMode(DisplayMode.External);
        }

        private void SetDisplayModeInternal(object sender, EventArgs e)
        {
            SetDisplayMode(DisplayMode.Internal);
        }

        private void SetDisplayModeExtend(object sender, EventArgs e)
        {
            SetDisplayMode(DisplayMode.Extend);
        }

        private void SetDisplayModeDuplicate(object sender, EventArgs e)
        {
            SetDisplayMode(DisplayMode.Duplicate);
        }

        private void QuitApp(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void UpdateStartupRegistry(object sender, EventArgs e)
        {
            var isApplicationInStartup = IsApplicationInStartup();
            if (isApplicationInStartup)
            {
                RemoveApplicationFromStartup();
            }
            else
            {
                AddApplicationToStartup();
            }
            UpdateControlsFromRegStartUp();
        }

        public static void AddApplicationToStartup()
        {
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true))
            {
                key.SetValue("MonitorSwitcher", "\"" + Application.ExecutablePath + "\"");
            }
        }

        public static void RemoveApplicationFromStartup()
        {
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true))
            {
                key.DeleteValue("MonitorSwitcher", false);
            }
        }

        public bool IsApplicationInStartup()
        {
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true))
            {
                return key.GetValue("MonitorSwitcher") == null ? false : true;
            }
        }
    }
}

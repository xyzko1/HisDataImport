using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Data;
using System.Drawing;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HisDataForms
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        string serviceFilePath = $"{Application.StartupPath}\\HisDataImport.exe";
        string serviceName = "HisDataImport";
        HisDataImport.Service1 service1 = new HisDataImport.Service1();
        private void button1_Click(object sender, EventArgs e)
        {
            //if (this.IsServiceExisted(serviceName)) this.UninstallService(serviceName);
            //this.InstallService(serviceFilePath);

        }

        private void button2_Click(object sender, EventArgs e)
        {
            service1.statrt();
            //if (this.IsServiceExisted(serviceName)) this.ServiceStart(serviceName);
            label1.Text = "服务运行中..";
        }

        private void button3_Click(object sender, EventArgs e)
        {
            service1.TestStop();
            label1.Text = "服务停止..";
            //if (this.IsServiceExisted(serviceName)) this.ServiceStop(serviceName);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            //if (this.IsServiceExisted(serviceName))
            //{
            //    this.ServiceStop(serviceName);
            //    this.UninstallService(serviceFilePath);
            //}

        }
        //判断服务是否存在
        private bool IsServiceExisted(string serviceName)
        {
            ServiceController[] services = ServiceController.GetServices();
            foreach (ServiceController sc in services)
            {
                if (sc.ServiceName.ToLower() == serviceName.ToLower())
                {
                    return true;
                }
            }
            return false;
        }

        //安装服务
        private void InstallService(string serviceFilePath)
        {
            using (AssemblyInstaller installer = new AssemblyInstaller())
            {
                installer.UseNewContext = true;
                installer.Path = serviceFilePath;
                IDictionary savedState = new Hashtable();
                installer.Install(savedState);
                installer.Commit(savedState);
            }
        }

        //卸载服务
        private void UninstallService(string serviceFilePath)
        {
            using (AssemblyInstaller installer = new AssemblyInstaller())
            {
                installer.UseNewContext = true;
                installer.Path = serviceFilePath;
                installer.Uninstall(null);
            }
        }
        //启动服务
        private void ServiceStart(string serviceName)
        {
            using (ServiceController control = new ServiceController(serviceName))
            {
                if (control.Status == ServiceControllerStatus.Stopped)
                {
                    control.Start();
                }
            }
        }

        //停止服务
        private void ServiceStop(string serviceName)
        {
            using (ServiceController control = new ServiceController(serviceName))
            {
                if (control.Status == ServiceControllerStatus.Running)
                {
                    control.Stop();
                }
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)//当用户点击窗体右上角X按钮或(Alt + F4)时 发生          
            {
                e.Cancel = true;
                this.ShowInTaskbar = false;
                this.myIcon.Icon = this.Icon;
                this.Hide();
            }
        }

        private void myIcon_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                myMenu.Show(MousePosition.X, MousePosition.Y);
            }

            if (e.Button == MouseButtons.Left)
            {
                this.Visible = true;
                this.WindowState = FormWindowState.Normal;
            }
        }



        private void toolMenuCancel_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}

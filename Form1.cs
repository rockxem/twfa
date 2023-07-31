using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading; 
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestWindowsFormsApp
{
    public partial class Form1 : Form
    {
        private readonly SynchronizationContext synchronizationContext;
        public string message1;
        public string message2;
        Stopwatch sw = new Stopwatch();
        public Form1()
        {
            InitializeComponent();
            synchronizationContext = SynchronizationContext.Current;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        public async Task DoTheTest(int Minutes)
        {
            int setMinutesToRunFor = Minutes; // the code will run for these many Minutes
            int waitTime = 25000;
            int iterationCounter = 1;
            sw.Start();
            
            DateTime start = DateTime.Now;
            for (int i = 1; i < 100000; i++)
            {
                if ((DateTime.Now - start).TotalMinutes >= setMinutesToRunFor)
                {
                    if (Debugger.IsAttached)
                    {
                        Application.Exit();
                    }
                    break;
                }                
                Thread.Sleep(waitTime); // Loop time to repeat task
                Process p = Process.GetProcessesByName("Teams").FirstOrDefault();
                if (p != null)
                {
                    IntPtr h = p.MainWindowHandle;

                    p.WaitForInputIdle();
                    SendKeys.SendWait("%{TAB}");
                    Thread.Sleep(500);

                    Random r = new Random();
                    int rInt = r.Next(1, 6); //for ints                    

                    if (rInt == 1)
                    {
                        p.WaitForInputIdle();
                        SendKeys.SendWait("^{1}");
                        Thread.Sleep(500);
                    }
                    if (rInt == 2)
                    {
                        p.WaitForInputIdle();
                        SendKeys.SendWait("^{2}");
                        Thread.Sleep(500);

                    }
                    if (rInt == 3)
                    {
                        p.WaitForInputIdle();
                        SendKeys.SendWait("^{3}");
                        Thread.Sleep(500);

                    }
                    if (rInt == 4)
                    {
                        p.WaitForInputIdle();
                        SendKeys.SendWait("^{4}");
                        Thread.Sleep(500);
                    }
                    if (rInt == 5)
                    {
                        p.WaitForInputIdle();
                        SendKeys.SendWait("{PGUP}");
                        Thread.Sleep(500);
                    }
                    if (rInt == 6)
                    {
                        p.WaitForInputIdle();
                        SendKeys.SendWait("{PGDN}");
                        Thread.Sleep(500);
                    }

                    p.WaitForInputIdle();
                    SendKeys.SendWait("{UP}");
                    Thread.Sleep(500);

                    p.WaitForInputIdle();
                    SendKeys.SendWait("{DOWN}");
                    Thread.Sleep(500);

                    string msg11 = $"{decimal.Round(((decimal)sw.Elapsed.TotalMinutes),2)} minutes";
                    iterationCounter++;
                    SetText(msg11);

                }
            }
        }
        private async void button1_Click(object sender, EventArgs e)
        {
            int val;
            int.TryParse(textBox1.Text, out val);
            if ( val > 0)
            {
                pictureBox1.Visible = true;
                label4.Visible = true;
                label5.Visible = true;
                DateTime dt = DateTime.Now;
                label2.Text = $"{dt.ToString()}";
                label1.ForeColor = Color.Black;
                label1.Text = "0.1 minutes";
                await Task.Run(() => DoTheTest(Convert.ToInt32(textBox1.Text)));
            }
            else
            {

                label1.Text = "Invalid value! Please enter correct value for minutes";
                label1.ForeColor = Color.Red;
                
            }
          
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
        delegate void SetTextCallback(string text);

        private void SetText(string text)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (this.label1.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetText);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                this.label1.Text = text;
            }
        }
    }
}

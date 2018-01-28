using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;


namespace HeavyOperation
{
    public partial class Form1 : Form
    {

        // Declare a delegate used to communicate with the UI thread
        private delegate void UpdateStatusDelegate(string message);
        private UpdateStatusDelegate updateStatusDelegate = null;

        public Form1()
        {
            InitializeComponent();

            // Initialise the delegate
            this.updateStatusDelegate = new UpdateStatusDelegate(this.UpdateStatus);
        }

        // Declare our worker thread
        private Thread workerThread = null;

        // Boolean flag used to stop the 
        private bool stopProcess = false;


        private void UpdateStatus(string message)
        {
        //    this.txtProgress.Text += "*";
        //    Thread.Sleep(50);
            this.txtProgress.Text += message + Environment.NewLine;
        }

        Process process = new Process();

        private void HeavyOperation()
        {
            //// Example heavy operation
            //for (int i = 0; i <= 999999; i++)
            //{
            //    // Check if Stop button was clicked
            //    if (!this.stopProcess)
            //    {
            //        // Show progress
            //        if ((i % 1000) == 0)
            //        {
            //            this.Invoke(this.updateStatusDelegate);
            //        }
            //    }
            //    else
            //    {
            //        // Stop heavy operation
            //        this.workerThread.Abort();
            //    }
            //}

            process.EnableRaisingEvents = true;
            process.OutputDataReceived += new System.Diagnostics.DataReceivedEventHandler(process_OutputDataReceived);
            process.ErrorDataReceived += new System.Diagnostics.DataReceivedEventHandler(process_ErrorDataReceived);
            process.Exited += new System.EventHandler(process_Exited);

            process.StartInfo.FileName = "ls";
            process.StartInfo.Arguments = "-l";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.RedirectStandardOutput = true;

            process.Start();
            process.BeginErrorReadLine();
            process.BeginOutputReadLine();

            //below line is optional if we want a blocking call
            //process.WaitForExit();

        }

        void process_Exited(object sender, EventArgs e)
        {
            string message = string.Format("process exited with code {0}\n", process.ExitCode.ToString());

            this.Invoke(this.updateStatusDelegate, message);
        }


        void process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            Console.WriteLine(e.Data + "\n");
        }

        void process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            //Console.WriteLine(e.Data);
            this.Invoke(this.updateStatusDelegate, e.Data);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.stopProcess = false;

            // Initialise and start worker thread
            this.workerThread = new Thread(new ThreadStart(this.HeavyOperation));
            this.workerThread.Start();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.stopProcess = true;
        }
    }
}

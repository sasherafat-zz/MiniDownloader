using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using Timer = System.Threading.Timer;
using System.Timers;

namespace Downloader
{
    public partial class Form1 : Form
    {
        protected WebClient webClient = new WebClient();
        protected Thread[] workerThread = new Thread[4];
        public int ThreadNumber = 0;

        static System.Windows.Forms.Timer[] aTimer = new System.Windows.Forms.Timer[4];

        public string[] linkAddress = new string[4], typeName = new string[4];
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
        private void Button_Click(object sender, EventArgs e)
        {
            aTimer[0] = new System.Windows.Forms.Timer();
            ThManager(0);
        }
        private void Button1_Click(object sender, EventArgs e)
        {
            aTimer[1] = new System.Windows.Forms.Timer();
            ThManager(1);
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            aTimer[2] = new System.Windows.Forms.Timer();
            ThManager(2);
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            aTimer[3] = new System.Windows.Forms.Timer();
            ThManager(3);
        }
        private void ThManager(int thnumber)
        {
            linkAddress[thnumber] = textBox1.Text;
            typeName[thnumber] = textBox2.Text;
            textBox1.Text = String.Empty;
            textBox2.Text = String.Empty;
            try
            {
                //SSL Exeption Handeler
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                ThreadNumber = thnumber;
                workerThread[thnumber] = new Thread(new ThreadStart(wcDownloader));
                workerThread[thnumber].Start();
                listBox.Items.Add("Download "+ (thnumber + 1) +" Started!");

                listBox.Items.Add("Size To Download : " + getFileSize(linkAddress[thnumber]) + " B");

                aTimer[thnumber].Tick += OnTimer;
                aTimer[thnumber].Interval = 1000;
                aTimer[thnumber].Start();

            }
            catch (Exception ex)
            {
                listBox.Items.Add("Error : " + ex.Message);
                workerThread[thnumber].Abort();
            }
        }
        private void wcDownloader()
        {
            webClient.DownloadFile(linkAddress[ThreadNumber], @".\" + "DlFile_" + generateName(5) + "." + typeName[ThreadNumber]);
            workerThread[ThreadNumber].Join();
        }

        private void OnTimer(Object sender, EventArgs e)
        {
            if (workerThread != null)
            {
                if (workerThread[ThreadNumber].ThreadState == ThreadState.WaitSleepJoin)
                {
                    listBox.Items.Add("Download " + (ThreadNumber + 1) + " Finished!");
                    aTimer[ThreadNumber].Stop();
                }
            }
        }
        public long getFileSize(string url)
        {
            long result = -1;

            System.Net.WebRequest req = System.Net.WebRequest.Create(url);
            req.Method = "HEAD";
            using (System.Net.WebResponse resp = req.GetResponse())
            {
                if (long.TryParse(resp.Headers.Get("Content-Length"), out long ContentLength))
                {
                    result = ContentLength;
                }
            }

            return result;
        }


        public static string generateName(int len)
        {
            Random r = new Random();
            string[] consonants = { "b", "c", "d", "f", "g", "h", "j", "k", "l", "m", "l", "n", "p", "q", "r", "s", "sh", "zh", "t", "v", "w", "x" };
            string[] vowels = { "a", "e", "i", "o", "u", "y" };
            string Name = "";
            Name += consonants[r.Next(consonants.Length)].ToUpper();
            Name += vowels[r.Next(vowels.Length)];
            int b = 2; //b tells how many times a new letter has been added. It's 2 right now because the first two letters are already in the name.
            while (b < len)
            {
                Name += consonants[r.Next(consonants.Length)];
                b++;
                Name += vowels[r.Next(vowels.Length)];
                b++;
            }

            return Name;
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SendDataBetweenProcesses
{
    public partial class TinhToan : Form
    {
        public TinhToan()
        {
            InitializeComponent();
        }
        public TinhToan(int length) : this()
        {
            this.Length = length;
            x = new Task(ReadDataFromMemmory);
            x.Start(); 
        }
        string s = "";
        private Task x;

        delegate void SetTextCallback(string text);
        private void SetText(string text)
        {
            if (this.txtText.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetText);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                this.txtText.Text = text;
            }
        }
        void Tinh()
        {
            var xs = S.Split('\n');
            foreach (var item in xs)
            {
                var t = item.Split(' ');

                if (t.Length > 2)
                {
                    var a1 = int.Parse(t[0]);
                    var b2 = int.Parse(t[2]);
                    if (t[1] == "+") s += item.ToString() + " = " + (a1 + b2).ToString() + Environment.NewLine;
                    else if (t[1] == "-") s += item.ToString() + " = " + (a1 - b2).ToString() + Environment.NewLine;
                    else if (t[1] == "*") s += item.ToString() + " = " + (a1 * b2).ToString() + Environment.NewLine;
                    else if (b2 > 0) s += item.ToString() + " = " + (a1 / b2).ToString() + Environment.NewLine;
                }
                else MessageBox.Show("Chỉ tính được phép toán 2 ngôi");
            }
            SetText(s);

        }
        void ReadDataFromMemmory()
        {
            try
            {
                using (MemoryMappedFile mmf = MemoryMappedFile.OpenExisting("testmap"))
                {
                    Mutex mutex = Mutex.OpenExisting("testmapmutex");
                    mutex.WaitOne();
                    using (MemoryMappedViewStream stream = mmf.CreateViewStream(1, 0))
                    {
                        byte[] buffer = new byte[Length];
                        if (stream.CanRead)
                        {
                            stream.Read(buffer, 0, Length);
                            using (var strxeam = new MemoryStream(buffer))
                            {
                                using (var streamReader = new StreamReader(strxeam))
                                {
                                    this.S = streamReader.ReadToEnd();
                                    Tinh();
                                }
                            }
                        }
                    }
                }
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("Memory-mapped file does not exist. Run Process A first, then B.");
            }
        }
        public int Length { get; private set; }
        public string S { get; private set; }

        private void button1_Click(object sender, EventArgs e)
        {
            x.Dispose();
            this.Close();
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SendDataBetweenProcesses
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        string s = "";
        void process1()
        {
            using (MemoryMappedFile mmf = MemoryMappedFile.CreateOrOpen("testmap", s.Length))
            {
                bool mutexCreated;
                Mutex mutex = new Mutex(true, "testmapmutex", out mutexCreated);
                using (MemoryMappedViewStream stream = mmf.CreateViewStream())
                {
                    BinaryWriter writer = new BinaryWriter(stream);
                    writer.Write(s);
                }
                mutex.ReleaseMutex();
                mutex.WaitOne();
                using (MemoryMappedViewStream stream = mmf.CreateViewStream())
                {
                    BinaryReader reader = new BinaryReader(stream);
                    Thread.Sleep(1000);
                    MessageBox.Show("đọc file thành công");
                }
                mutex.ReleaseMutex();
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            var op = new OpenFileDialog();
            if (op.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    var sr = new StreamReader(op.FileName);
                    s = sr.ReadToEnd().ToString();
                    if (s.Length > 0)
                    {
                        txtText.Text = s;
                        var ts = new Task(process1);
                        ts.Start();
                        var p2 = new TinhToan(s.Length);
                        p2.ShowDialog();
                        if (p2.S != null)
                        {
                            txtText.Text = p2.S;
                            if (MessageBox.Show("Lưu kết quả vào KQ.txt", "Thông báo", MessageBoxButtons.YesNo) == DialogResult.Yes)
                            {
                                ts.Dispose();
                                File.AppendAllText("KQ.txt", p2.S);
                            }
                            
                        }
                            
                    }
                    else MessageBox.Show("file rỗng");
                }
                catch (SecurityException ex)
                {
                    MessageBox.Show($"Security error.\n\nError message: {ex.Message}\n\n" +
                    $"Details:\n\n{ex.StackTrace}");
                }

            }
        }

    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Proc1
{
    class Program
    {
        static void Main(string[] args)
        {
            string s = "";
            foreach (var item in args)
                s += item;
            using (MemoryMappedFile mmf = MemoryMappedFile.CreateOrOpen("testmap", s.Length))
            {
                bool mutexCreated;
                Mutex mutex = new Mutex(true, "testmapmutex", out mutexCreated);

                using (MemoryMappedViewStream stream = mmf.CreateViewStream())
                {
                    BinaryWriter writer = new BinaryWriter(stream);
                    writer.Write(s.ToString());
                }
                mutex.ReleaseMutex();
                Console.WriteLine("Start Process B and press ENTER to continue.");
                Console.ReadLine();
                mutex.WaitOne();
                using (MemoryMappedViewStream stream = mmf.CreateViewStream())
                {
                    BinaryReader reader = new BinaryReader(stream);

                    Console.WriteLine("Process B says: {0}", reader.ReadBoolean());
                }
                mutex.ReleaseMutex();
            }
        }
    }
}

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

namespace Proc2
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                using (MemoryMappedFile mmf = MemoryMappedFile.OpenExisting("testmap"))
                {
                    Mutex mutex = Mutex.OpenExisting("testmapmutex");
                    mutex.WaitOne();
                    using (MemoryMappedViewStream stream = mmf.CreateViewStream(1, 0))
                    {
                        byte[] buffer = new byte[1024];
                        while (stream.CanRead)
                        {
                            stream.Read(buffer, 0, 1024);
                            using (var strxeam = new MemoryStream(buffer))
                            {
                                using (var streamReader = new StreamReader(strxeam))
                                {
                                    Console.WriteLine(streamReader.ReadToEnd()); ;
                                }
                            }
                        }
                    }
                    //mutex.ReleaseMutex();
                }
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("Memory-mapped file does not exist. Run Process A first, then B.");
            }
        }
    }

}

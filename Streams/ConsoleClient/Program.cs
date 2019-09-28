using System;
using System.Configuration;
using System.IO;
using System.Text;
using static StreamsDemo.StreamsExtension;

namespace ConsoleClient
{
    class Program
    {
        static void Main(string[] args)
        {
            //var source = ConfigurationManager.AppSettings[@"C:\Epam_Step2\Days20-21 02.05.2019\SourceText.txt"];

            //var destination = ConfigurationManager.AppSettings[@"C:\Epam_Step2\Days20-21 02.05.2019\fileTo.txt"];

            string source = @"C:\Epam_Step2\NET1.S.2019.Sokolova.20\SourceText.txt";
            string destination = @"C:\Epam_Step2\NET1.S.2019.Sokolova.20\fileTo.txt";

            if (File.Exists(destination))
            {
                File.Delete(destination);
            }

            using (FileStream fs = File.Create(destination)) { }
            Console.WriteLine($"BybyteCopy() done. Total bytes: {ByByteCopy(source, destination)}");
            Console.WriteLine($"IsContentEquals() done: {IsContentEquals(source, destination)}");
            Console.WriteLine();

            if (File.Exists(destination))
            {
                File.Delete(destination);
            }

            using (FileStream fs = File.Create(destination)) { }
            Console.WriteLine($"InMemoryByByteCopy() done. Total bytes: {InMemoryByByteCopy(source, destination)}");
            Console.WriteLine($"IsContentEquals() done: {IsContentEquals(source, destination)}");
            Console.WriteLine();

            if (File.Exists(destination))
            {
                File.Delete(destination);
            }

            using (FileStream fs = File.Create(destination)) { }
            Console.WriteLine($"ByBockCopy() done. Total bytes: {ByBlockCopy(source, destination)}");
            Console.WriteLine($"IsContentEquals() done: {IsContentEquals(source, destination)}");
            Console.WriteLine();

            if (File.Exists(destination))
            {
                File.Delete(destination);
            }

            using (FileStream fs = File.Create(destination)) { }
            Console.WriteLine($"BufferedCopy() done. Total bytes: {BufferedCopy(source, destination)}");
            Console.WriteLine($"IsContentEquals() done: {IsContentEquals(source, destination)}");
            Console.WriteLine();

            if (File.Exists(destination))
            {
                File.Delete(destination);
            }

            using (FileStream fs = File.Create(destination)) { }
            Console.WriteLine($"InMemoryByBlockCopy() done. Total bytes: {InMemoryByBlockCopy(source, destination)}");
            Console.WriteLine($"IsContentEquals() done: {IsContentEquals(source, destination)}");
            Console.WriteLine();

            if (File.Exists(destination))
            {
                File.Delete(destination);
            }

            using (FileStream fs = File.Create(destination)) { }
            Console.WriteLine($"ByLineCopy() done. Total bytes: {ByLineCopy(source, destination)}");
            Console.WriteLine($"IsContentEquals() done: {IsContentEquals(source, destination)}");
        }
    }
}

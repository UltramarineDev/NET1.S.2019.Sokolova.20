using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace StreamsDemo
{
    // C# 6.0 in a Nutshell. Joseph Albahari, Ben Albahari. O'Reilly Media. 2015
    // Chapter 15: Streams and I/O
    // Chapter 6: Framework Fundamentals - Text Encodings and Unicode
    // https://msdn.microsoft.com/ru-ru/library/system.text.encoding(v=vs.110).aspx

    public static class StreamsExtension
    {

        #region Public members

        #region Task 1 TODO: Implement by byte copy logic using class FileStream as a backing store stream .

        public static int ByByteCopy(string sourcePath, string destinationPath)
        {
            InputValidation(sourcePath, destinationPath);
            using (FileStream outputStream = File.OpenWrite(destinationPath))
            using (FileStream inputStream = File.OpenRead(sourcePath))
            {
                inputStream.CopyTo(outputStream);
                return (int)outputStream.Length;
            }
        }

        #endregion

        #region Task 2 TODO: Implement by byte copy logic using class MemoryStream as a backing store stream.

        public static int InMemoryByByteCopy(string sourcePath, string destinationPath)
        {
            InputValidation(sourcePath, destinationPath);
            // TODO: step 1. Use StreamReader to read entire file in string
            var content = new StringBuilder();
            using (StreamReader sr = new StreamReader(sourcePath))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    content.Append(line);
                }
            }

            // TODO: step 2. Create byte array on base string content - use  System.Text.Encoding class
            Encoding unicode = Encoding.Unicode;
            byte[] unicodeBytes = unicode.GetBytes(content.ToString());

            // TODO: step 3. Use MemoryStream instance to read from byte array (from step 2)
            using (var memStream = new MemoryStream())
            {
                memStream.Write(unicodeBytes, 0, unicodeBytes.Length);
                memStream.Seek(0, SeekOrigin.Begin);
                // TODO: step 4. Use MemoryStream instance (from step 3) to write it content in new byte array
                int count = 0;
                byte[] byteArray = new byte[memStream.Length];

                count = memStream.Read(byteArray, 0, (int)memStream.Length);
                while (count < memStream.Length)
                {
                    byteArray[count++] = Convert.ToByte(memStream.ReadByte());
                }
            }

            // TODO: step 5. Use Encoding class instance (from step 2) to create char array on byte array content

            char[] unicodeChars = new char[unicode.GetCharCount(unicodeBytes, 0, unicodeBytes.Length)];
            unicode.GetChars(unicodeBytes, 0, unicodeBytes.Length, unicodeChars, 0);

            // TODO: step 6. Use StreamWriter here to write char array content in new file
            using (var writer = new StreamWriter(destinationPath))
            {
                foreach (var unicodeChar in unicodeChars)
                {
                    writer.Write(unicodeChar);
                }
            }

            return File.ReadAllBytes(destinationPath).Length;
        }

        #endregion

        #region Task 3 TODO: Implement by block copy logic using FileStream buffer.

        public static int ByBlockCopy(string sourcePath, string destinationPath)
        {
            InputValidation(sourcePath, destinationPath);
            using (FileStream outputStream = File.OpenWrite(destinationPath))
            using (FileStream inputStream = File.OpenRead(sourcePath))
            {
                const int BufferSize = 1024;
                byte[] buffer = new byte[BufferSize];
                int readed = inputStream.Read(buffer, 0, BufferSize);
                while (readed > 0)
                {
                    outputStream.Write(buffer, 0, readed);
                    readed = inputStream.Read(buffer, 0, BufferSize);
                }
            }

            return File.ReadAllBytes(destinationPath).Length;
        }

        #endregion

        #region Task 5 TODO: Implement by block copy logic using MemoryStream.

        public static int InMemoryByBlockCopy(string sourcePath, string destinationPath)
        {
            // TODO: Use InMemoryByByteCopy method's approach
            InputValidation(sourcePath, destinationPath);
            var content = new StringBuilder();
            using (StreamReader sr = new StreamReader(sourcePath))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    content.Append(line);
                }
            }

            byte[] buffer = Encoding.ASCII.GetBytes(content.ToString());

            using (FileStream outputStream = File.OpenWrite(destinationPath))
            using (var memStream = new MemoryStream(buffer.Length))
            {
                memStream.Write(buffer, 0, buffer.Length);
                memStream.WriteTo(outputStream);
            }

            return File.ReadAllBytes(destinationPath).Length;
        }

        #endregion

        #region Task 4 TODO: Implement by block copy logic using class-decorator BufferedStream.

        public static int BufferedCopy(string sourcePath, string destinationPath)
        {
            InputValidation(sourcePath, destinationPath);

            const int streamBufferSize = 50;
            const int dataArraySize = 50;

            byte[] data = new byte[dataArraySize];

            List<byte[]> byteArrays = new List<byte[]>();
            using (FileStream inputStream = File.Open(sourcePath, FileMode.Open))
            using (Stream bufStream = new BufferedStream(inputStream))
            {
                if (bufStream.CanRead)
                {
                    int readed = bufStream.Read(data, 0, streamBufferSize);
                    byteArrays.Add(data);

                    while (readed > 0)
                    {
                        var currentData = new byte[dataArraySize];
                        readed = bufStream.Read(currentData, 0, streamBufferSize);
                        byteArrays.Add(currentData);
                        if (readed < streamBufferSize)
                        {
                            break;
                        }
                    }
                }
            }

            using (FileStream outputStream = File.Open(destinationPath, FileMode.Open))
            using (Stream bufStream = new BufferedStream(outputStream))
            {
                if (bufStream.CanWrite)
                {
                    for (var index = 0; index < byteArrays.Count; index++)
                    {
                        int lenght = byteArrays[index].Length;
                        if (index == byteArrays.Count - 1)
                        {
                            lenght = CountLastBufferBlockLenght(byteArrays[index]);
                        }

                        bufStream.Write(byteArrays[index], 0, lenght);
                    }
                }
            }

            return File.ReadAllBytes(destinationPath).Length;
        }

        private static int CountLastBufferBlockLenght(byte[] array)
        {
            int lenght = 0;
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] != 0)
                {
                    lenght++;
                }
                else
                {
                    break;
                }
            }

            return lenght;
        }

        #endregion

        #region Task 6 TODO: Implement by line copy logic using FileStream and classes text-adapters StreamReader/StreamWriter

        public static int ByLineCopy(string sourcePath, string destinationPath)
        {
            InputValidation(sourcePath, destinationPath);

            using (StreamWriter writer = new StreamWriter(destinationPath))
            using (StreamReader reader = new StreamReader(sourcePath))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    writer.WriteLine(line);
                }
            }

            return File.ReadAllBytes(destinationPath).Length;
        }

        #endregion

        #region Task 7 TODO: Implement content comparison logic of two files 

        public static bool IsContentEquals(string sourcePath, string destinationPath)
        {
            InputValidation(sourcePath, destinationPath);
            bool result = true;
            var contentSource = new StringBuilder();
            var contentDestination = new StringBuilder();

            using (StreamReader dest = new StreamReader(destinationPath))
            using (StreamReader sr = new StreamReader(sourcePath))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    contentSource.Append(line);
                }

                line = null;
                while ((line = dest.ReadLine()) != null)
                {
                    contentDestination.Append(line);
                }

                if (contentSource.Length != contentDestination.Length)
                {
                    result = false;
                }
                else
                {
                    byte[] bufferSource = Encoding.ASCII.GetBytes(contentSource.ToString());
                    byte[] bufferDestination = Encoding.ASCII.GetBytes(contentDestination.ToString());
                    for (int i = 0; i < bufferSource.Length; i++)
                    {
                        if (!bufferSource[i].Equals(bufferDestination[i]))
                        {
                            result = false;
                        }
                    }
                }
            }

            return result;
        }

        #endregion

        #endregion

        #region Private members

        #region TODO: Implement validation logic

        private static void InputValidation(string sourcePath, string destinationPath)
        {
            if (!File.Exists(sourcePath))
            {
                throw new ArgumentException("Source file not found.", nameof(sourcePath));
            }

            if (!File.Exists(destinationPath))
            {
                throw new ArgumentException("Target file not found.", nameof(destinationPath));
            }
        }

        #endregion

        #endregion

    }
}

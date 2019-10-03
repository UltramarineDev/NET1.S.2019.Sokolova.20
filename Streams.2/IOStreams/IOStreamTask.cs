using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.IO.Packaging;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;
using OfficeOpenXml;

namespace IOStreams
{
    public static class IOStreamTask
    {
        /// <summary>
        /// Parses Resourses\Planets.xlsx file and returns the planet data: 
        ///   Jupiter     69911.00
        ///   Saturn      58232.00
        ///   Uranus      25362.00
        ///    ...
        /// See Resourses\Planets.xlsx for details
        /// </summary>
        /// <param name="xlsxFileName">Source file name.</param>
        /// <returns>Sequence of PlanetInfo</returns>
        public static IEnumerable<PlanetInfo> ReadPlanetInfoFromXlsx(string xlsxFileName)
        {

            // TODO : Implement ReadPlanetInfoFromXlsx method using System.IO.Packaging + Linq-2-Xml

            // HINT : Please be as simple & clear as possible.
            //        No complex and common use cases, just this specified file.
            //        Required data are stored in Planets.xlsx archive in 2 files:
            //         /xl/sharedStrings.xml      - dictionary of all string values
            //         /xl/worksheets/sheet1.xml  - main worksheet            
            List<PlanetInfo> list = new List<PlanetInfo>();
            using (ExcelPackage xlPackage = new ExcelPackage(new FileInfo(xlsxFileName)))
            {
                var sheet1 = xlPackage.Workbook.Worksheets.First();
                var totalRows = sheet1.Dimension.End.Row;
                var totalColumns = sheet1.Dimension.End.Column;


                for (int rowNum = 2; rowNum <= totalRows; rowNum++)
                {
                    var row = sheet1.Cells[rowNum, 1, rowNum, totalColumns].Select(c => c.Value == null ? string.Empty : c.Value.ToString());
                    row = row.Where(x => x != string.Empty);
                    bool isRadious = double.TryParse(row.Last(), out double radious);
                    if (!isRadious)
                    {
                        throw new ArgumentException("Invalid radious", nameof(xlsxFileName));
                    }

                    list.Add(new PlanetInfo() { Name = row.First(), MeanRadius = radious });
                }
            }

            return list;
        }

        /// <summary>
        /// Calculates hash of stream using specified algorithm.
        /// </summary>
        /// <param name="stream">Source stream</param>
        /// <param name="hashAlgorithmName">
        ///     Hash algorithm ("MD5","SHA1","SHA256" and other supported by .NET).
        /// </param>
        /// <returns></returns>
        public static string CalculateHash(this Stream stream, string hashAlgorithmName)
        {
            // TODO : Implement CalculateHash method
            if (string.IsNullOrEmpty(hashAlgorithmName))
            {
                throw new ArgumentException("Invalid algorithm name", nameof(hashAlgorithmName));
            }

            var hashAlg = HashAlgorithm.Create(hashAlgorithmName) ?? throw new ArgumentException("Invalid algorithm", nameof(hashAlgorithmName));
            byte[] data = hashAlg.ComputeHash(stream);
            var sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            return sBuilder.ToString().ToUpper();

            // HashAlgorithm hash = hashAlg.BaseType.Name.Equals(typeof(HashAlgorithm)) as HashAlgorithm ?? throw new ArgumentException();
            //HashAlgorithm hash = (HashAlgorithm)Convert.ChangeType(hashAlg.BaseType.Name, typeof(HashAlgorithm));

        }

        /// <summary>
        /// Returns decompressed stream from file. 
        /// </summary>
        /// <param name="fileName">Source file.</param>
        /// <param name="method">Method used for compression (none, deflate, gzip).</param>
        /// <returns>output stream</returns>
        public static Stream DecompressStream(string fileName, DecompressionMethods method)
        {
            string newFileName = "";
            // TODO : Implement DecompressStream method
            using (FileStream originalFileStream = File.OpenRead(fileName))
            {
                var info = new FileInfo(fileName);
                string currentFileName = info.Name;
                if (method == DecompressionMethods.None)
                    newFileName = currentFileName;
                else
                    newFileName = currentFileName.Remove(currentFileName.Length - info.Extension.Length);

                var directory = Path.GetDirectoryName(@"Decompress\Planet.xlsx");
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                string pathToCreate = Path.Combine(directory, newFileName);
                using (FileStream decompressedFileStream = File.Create(pathToCreate))
                {
                    switch (method)
                    {
                        case DecompressionMethods.Deflate:
                            using (DeflateStream decompressionStream = new DeflateStream(originalFileStream, CompressionMode.Decompress, false))
                            {
                                decompressionStream.CopyTo(decompressedFileStream);
                                break;
                            }

                        case DecompressionMethods.None:
                            {
                                originalFileStream.CopyTo(decompressedFileStream);
                                break;
                            }
                           

                        case DecompressionMethods.GZip:
                            using (GZipStream decompressionStream = new GZipStream(originalFileStream, CompressionMode.Decompress, false))
                            {
                                decompressionStream.CopyTo(decompressedFileStream);
                                break;
                            }
                        default:
                            break;
                    }
                }

                return File.OpenRead(pathToCreate);
            }
        }


        /// <summary>
        /// Reads file content encoded with non Unicode encoding
        /// </summary>
        /// <param name="fileName">Source file name</param>
        /// <param name="encoding">Encoding name</param>
        /// <returns>Unicoded file content</returns>
        public static string ReadEncodedText(string fileName, string encoding)
        {
            // TODO : Implement ReadEncodedText method
            byte[] buff = null;
            using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                BinaryReader br = new BinaryReader(fs);
                long numBytes = new FileInfo(fileName).Length;
                buff = br.ReadBytes((int)numBytes);

                Encoding encod = Encoding.GetEncoding(encoding);

                char[] charArray = encod.GetChars(buff);
                return new string(charArray);
            }
        }
    }
}
using System;
using System.IO;
using System.Text;

namespace Lab3
{
    class Program
    {
        enum FileType
        {
            png,
            bmp,
            unknown
        }

        static void Main(string[] args)
        {
            FileType fileType;
            Byte[] data;
            int fileSize;

            if (args.Length > 0) 
            {
                try
                {
                    FileStream fs;
                    using (fs = new FileStream(args[0], FileMode.Open))
                    {
                        fileSize = (int)fs.Length;
                        data = new Byte[fileSize];
                        fs.Read(data, 0, fileSize);
                        fs.Close();
                    }

                    fileType = GetFileType(data);
                    if (fileType == FileType.unknown)
                    {
                        Console.WriteLine("This is not a valid .bmp or.png file!");
                    }
                    else
                    {
                        int[] resolution = GetResolution(fileType, data);
                        Console.WriteLine($"This is a .{fileType} image. Resolution: {resolution[0]}x{resolution[1]} pixels.");

                        if (fileType == FileType.png)
                        {
                            PrintAllPngChunks(data);
                        }
                    }
                }
                catch (FileNotFoundException)
                {
                    Console.WriteLine("File not found.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occured: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("File not specified.");
            }
        }

        static FileType GetFileType(Byte[] data)
        {
            string first8Bytes = "";
            string first2Bytes;

            for (int i = 0; i < 8; i++)
            {
                first8Bytes += data[i].ToString();
            }
            first2Bytes = first8Bytes.Substring(0, 4);

            if (first8Bytes == "13780787113102610")
            {
                return FileType.png;
            }
            else if (first2Bytes == "6677") 
            {
                return FileType.bmp;
            }
            else
            {
                return FileType.unknown;
            }
        }

        static int[] GetResolution(FileType fileType, Byte[] data)
        {
            int[] resolution = new int[2];
            if (fileType == FileType.png)
            {
                Byte[] bytes = new Byte[4] { data[19], data[18], data[17], data[16] };
                resolution[0] = BitConverter.ToInt32(bytes, 0);

                bytes[0] = data[23];
                bytes[1] = data[22];
                bytes[2] = data[21];
                bytes[3] = data[20];
                resolution[1] = BitConverter.ToInt32(bytes, 0);
            }
            else if (fileType == FileType.bmp)
            {
                Byte[] bytes = new Byte[4] { data[18], data[19], data[20], data[21] };
                resolution[0] = BitConverter.ToInt32(bytes, 0);

                bytes[0] = data[22];
                bytes[1] = data[23];
                bytes[2] = data[24];
                bytes[3] = data[25];
                resolution[1] = BitConverter.ToInt32(bytes, 0);
            }

            return resolution;
        }

        static void PrintAllPngChunks(Byte[] data)
        {
            Console.WriteLine("");

            int i = 8;
            int chunkSize;
            while (i < data.Length )
            {
                Byte[] bytes = new Byte[4] { data[i+3], data[i+2], data[i+1], data[i] };
                chunkSize = BitConverter.ToInt32(bytes, 0);

                bytes[0] = data[i + 4];
                bytes[1] = data[i + 5];
                bytes[2] = data[i + 6];
                bytes[3] = data[i + 7];
                Console.WriteLine(Encoding.ASCII.GetString(bytes));

                i = i + 12 + chunkSize;
            }
            return;
        }
    }
}

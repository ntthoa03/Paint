
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Xml.Serialization;
using System.Xml;
using System;
using Microsoft.Win32;
using MyLib;

namespace Paint
{
    internal class MyFile
    {
        public const int BINARY_FILE = 1;
        public const int XML_FILE = 2;
        public const string MPXML_EXT = ".xml";
        public const string MPBIT_EXT = ".mpbin";



        public const char big_seperator = '|';
        public const char minor_seperator_1 = '!';
        public const char minor_seperator_2 = ';';
        private const int BUFFER_SIZE = 1024 * 2;

        public double ImageDpiX { get; set; }
        public double ImageDpiY { get; set; }
        public System.Windows.Media.PixelFormat ImagePixelFormat { get; set; }


        public Dictionary<string, IShape>? ReferenceAbilities { get; set; } = null;

        public string? CurrentStoredPath { get; set; } = null;

        public SaveFileDialog SaveFileDialog { get; set; }

        public OpenFileDialog OpenFileDialog { get; set; }


        public MyFile()
        {
            SaveFileDialog = new SaveFileDialog();
            SaveFileDialog.InitialDirectory = @"C:\";
            SaveFileDialog.Title = "Save your file";
            SaveFileDialog.RestoreDirectory = true;
            SaveFileDialog.DefaultExt = "mpbin";
            SaveFileDialog.CheckFileExists = false;
            SaveFileDialog.CheckPathExists = true;
            SaveFileDialog.Filter = "Binary file (*.mpbin)|*.mpbin";
            SaveFileDialog.FilterIndex = 1;
            SaveFileDialog.AddExtension = true;

            OpenFileDialog = new OpenFileDialog();
            OpenFileDialog.Filter = "Binary file (*.mpbin)|*.mpbin";
            OpenFileDialog.Title = "Open file";
            OpenFileDialog.FilterIndex = 1;
            OpenFileDialog.AddExtension = true;
            OpenFileDialog.CheckFileExists = false;
            OpenFileDialog.CheckPathExists = true;
            OpenFileDialog.DefaultExt = "xml";

            ImageDpiX = 1 / 96;
            ImageDpiY = 1 / 96;
            ImagePixelFormat = PixelFormats.Pbgra32;
        }

        public MyFile(string OpenPath)
        {
            SaveFileDialog = new SaveFileDialog();
            SaveFileDialog.InitialDirectory = @"C:\";
            SaveFileDialog.Title = "Save your file";
            SaveFileDialog.RestoreDirectory = true;
            SaveFileDialog.CheckFileExists = true;
            SaveFileDialog.CheckPathExists = true;
            SaveFileDialog.AddExtension = true;
            SaveFileDialog.DefaultExt = "mpbin";
            SaveFileDialog.Filter = "Binary file (*.mpbin)|*.mpbin";
            CurrentStoredPath = OpenPath;

            OpenFileDialog = new OpenFileDialog();
            OpenFileDialog.Filter = "Binary file (*.mpbin)|*.mpbin";
            OpenFileDialog.Title = "Open file";
            OpenFileDialog.FilterIndex = 1;
            OpenFileDialog.AddExtension = true;
            OpenFileDialog.CheckFileExists = false;
            OpenFileDialog.CheckPathExists = true;
            OpenFileDialog.DefaultExt = "mpbin";


            ImageDpiX = 1 / 96;
            ImageDpiY = 1 / 96;
            ImagePixelFormat = PixelFormats.Pbgra32;
        }

        public bool isExist(string path)
        {
            FileInfo fileInfo = new FileInfo(path);
            return fileInfo.Exists;
        }

        public bool isNewFile()
        {
            if (CurrentStoredPath == null)
            {
                return true;
            }
            else
            { return false; }
        }

        public bool WriteTo(string filepath, List<IShape> shapes, int write_mode)
        {
            bool result = true;
            try
            {
                switch (write_mode)
                {
                    case XML_FILE:
                        {

                            StreamWriter writer = new StreamWriter(filepath);
                            //clear old data
                            writer.BaseStream.Seek(0, SeekOrigin.Begin);
                            writer.Flush();

                            for (int i = 0; i < shapes.Count; i++)
                            {
                                XmlSerializer s = new XmlSerializer(shapes[i].GetType());

                                s.Serialize(writer, shapes[i]);
                            }

                            writer.Close();
                            result = true;
                            break;
                        }
                    case BINARY_FILE:
                        {
                            string construct_string = "";
                            for (int i = 0; i < shapes.Count; i++)
                            {
                                string string_of_shape = new StringBuilder().Append(big_seperator).Append(shapes[i].FromShapeToString()).ToString();
                                construct_string = construct_string + string_of_shape;
                            }


                            if (construct_string.Length <= 0) { break; }
                            
                            byte[] buffer = Encoding.Unicode.GetBytes(construct_string);
                            FileStream file = File.Open(filepath, FileMode.Create);
                            file.Position = 0;
                            BinaryWriter writer = new BinaryWriter(file, Encoding.UTF8, false);
                            writer.Flush();
                            writer.Seek(0, SeekOrigin.Begin);

                            writer.Write(buffer);
                            writer.Close();

                            break;
                        }
                }
            }
            catch (Exception ex)
            {
                result = false;
            }

            return result;
        }

        public List<IShape> ReadFrom(string filepath, int read_mode)
        {
            if (ReferenceAbilities == null)
            {
                throw new ArgumentNullException("Set ReferenceAbilities variable.");
            }

            List<IShape> shapes = new List<IShape>();
            try
            {
                switch (read_mode)
                {
                    case XML_FILE:
                        {

                            XmlDocument xmlDocument = new XmlDocument();
                            xmlDocument.Load(filepath);
                            if (xmlDocument.DocumentElement == null)
                            {
                                shapes = null;
                                break;
                            }
                            break;
                        }
                    case BINARY_FILE:
                        {
                            FileStream file = File.Open(filepath, FileMode.Open);
                            BinaryReader reader = new BinaryReader(file, Encoding.Unicode);
                            reader.BaseStream.Position = 0;
                            List<byte> buffer = new List<byte>();

                            long file_size = file.Length;
                            do
                            {
                                byte[] task = reader.ReadBytes(BUFFER_SIZE);
                                buffer.AddRange(task);
                            }
                            while (buffer.Count < file_size);

                            string convertedString = Encoding.Unicode.GetString(buffer.ToArray());
                            
                            reader.Close();

                            if (convertedString != null)
                            {
                                string[] items = convertedString.Split(big_seperator);
                                
                                for (int i = 1; i < items.Length; i++)
                                {
      
                                    string[] details = items[i].Split(minor_seperator_1);

                                    IShape controller = (IShape)ReferenceAbilities[details[0]].deepCopy();
                                    IShape shape = controller.FromStringToShape(items[i]);
                                    shapes.Add(shape);

                                }
                            }
                            break;
                        }
                }
            }
            catch (Exception ex)
            {
            }
            return shapes;
        }
    }
}
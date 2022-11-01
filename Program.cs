using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml;
using System.IO.Compression;

namespace FileSystem
{
    class Program
    {

        static void InfoDrive()
        {
            DriveInfo[] drives = DriveInfo.GetDrives();

            foreach (DriveInfo drive in drives)
            {
                Console.WriteLine($"Название: {drive.Name}");
                Console.WriteLine($"Тип: {drive.DriveType}");
                //IsReady - Возвращает значение, указывающее, готов ли диск.
                if (drive.IsReady)
                {
                    Console.WriteLine($"Файловая система: {drive.DriveFormat}");
                    Console.WriteLine($"Объем диска: {drive.TotalSize}");
                    Console.WriteLine($"Свободное пространство: {drive.TotalFreeSpace}");
                    Console.WriteLine($"Метка: {drive.VolumeLabel}");
                }
                Console.WriteLine();
            }
        }

        static void WorkingWithFiles()
        {
            Console.Write("Название нового файла:");
            string path = Console.ReadLine();
            Console.WriteLine("Введите строку для записи в файл:");
            string text = Console.ReadLine();
            // Создание и запись в файл
            using (FileStream fstream = new FileStream(path, FileMode.OpenOrCreate))
            {
                // Преобразуем строку в байты
                byte[] array = System.Text.Encoding.Default.GetBytes(text);
                // Запись массива байтов в файл
                fstream.Write(array, 0, array.Length);
                Console.WriteLine("Текст записан в файл");
            }
            // Чтение файла
            using (FileStream fstream = File.OpenRead(path))
            {
                // Преобразуем строку в байты
                byte[] array = new byte[fstream.Length];
                // Считываем данные
                fstream.Read(array, 0, array.Length);
                // Декодируем байты в строку
                string textFromFile = System.Text.Encoding.Default.GetString(array);
                Console.WriteLine($"Текст из файла: {textFromFile}");
            }
            // Удаление файла
            FileInfo fileInf = new FileInfo(path);
            if (fileInf.Exists)
            {
                File.Delete(path);
                Console.WriteLine("Файл удален");
            }
        }
        class UBI
        {
            public int Id { get; set; }
            public string Title { get; set; }
        }

        static async Task WorkingWithJson()
        {
            string path = @"bdu.json";
            Console.Write("Номер УИБ: ");
            int id = Convert.ToInt32(Console.ReadLine());
            Console.Write("Название УИБ: ");
            string title = Console.ReadLine();
            // Сохранение данных
            using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate))
            {
                UBI newUBI = new UBI() { Id = id, Title = title };
                await JsonSerializer.SerializeAsync<UBI>(fs,newUBI);
                Console.WriteLine("Данные записанны");
            }
            // Чтение данных
            Console.WriteLine("Чтение файла:");
            using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate))
            {
                UBI DesUBI = await JsonSerializer.DeserializeAsync<UBI>(fs);
                Console.WriteLine($"Id: {DesUBI.Id} Title: {DesUBI.Title}");
            }
            FileInfo fileInf = new FileInfo(path);
            if (fileInf.Exists)
            {
                File.Delete(path);
                Console.WriteLine("Файл удален");
            }
        }

        static void WorkingWithXML()
        {
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load("bdu.xml");
            // получим корневой элемент
            XmlElement xRoot = xDoc.DocumentElement;
            // обход всех узлов в корневом элементе
            XmlElement bduElem = xDoc.CreateElement("uib");
            // создаем атрибут id
            XmlAttribute idAttr = xDoc.CreateAttribute("id");
            // создаем атрибут title
            XmlElement titleAttr = xDoc.CreateElement("title");
            Console.WriteLine("Id УИБ:");
            XmlText idText = xDoc.CreateTextNode(Console.ReadLine());
            Console.WriteLine("Название УИБ:");
            XmlText titleText = xDoc.CreateTextNode(Console.ReadLine());

            idAttr.AppendChild(idText);
            titleAttr.AppendChild(titleText);
            bduElem.Attributes.Append(idAttr);
            bduElem.AppendChild(titleAttr);
            xRoot.AppendChild(bduElem);
            xDoc.Save("bdu.xml");

            foreach (XmlNode xnode in xRoot)
            {
                // получаем атрибуты
                if (xnode.Attributes.Count > 0)
                {
                    XmlNode attr = xnode.Attributes.GetNamedItem("id");
                    if (attr != null)
                        Console.WriteLine($"Id: {attr.Value}");
                }
                // обходим все дочерние узлы элемента
                foreach (XmlNode childnode in xnode.ChildNodes)
                {
                    if (childnode.Name == "title")
                    {
                        Console.WriteLine($"Название: {childnode.InnerText}");
                    }
                }
            }
            FileInfo fileInf = new FileInfo("bdu.xml");
            if (fileInf.Exists)
            {
                File.Delete("bdu.xml");
                Console.WriteLine("Файл удален");
            }
        }
        public static void Compress(string sourceFile, string compressedFile)
        {
            // поток для чтения исходного файла
            using (FileStream sourceStream = new FileStream(sourceFile, FileMode.OpenOrCreate))
            {
                // поток для записи сжатого файла
                using (FileStream targetStream = File.Create(compressedFile))
                {
                    // поток архивации
                    using (GZipStream compressionStream = new GZipStream(targetStream, CompressionMode.Compress))
                    {
                        sourceStream.CopyTo(compressionStream); // копируем байты из одного потока в другой
                        Console.WriteLine("Сжатие файла {0} завершено. Исходный размер: {1}  сжатый размер: {2}.",
                            sourceFile, sourceStream.Length.ToString(), targetStream.Length.ToString());
                    }
                }
            }
        }
        public static void Decompress(string compressedFile, string targetFile)
        {
            // поток для чтения из сжатого файла
            using (FileStream sourceStream = new FileStream(compressedFile, FileMode.OpenOrCreate))
            {
                // поток для записи восстановленного файла
                using (FileStream targetStream = File.Create(targetFile))
                {
                    // поток разархивации
                    using (GZipStream decompressionStream = new GZipStream(sourceStream, CompressionMode.Decompress))
                    {
                        decompressionStream.CopyTo(targetStream);
                        Console.WriteLine("Восстановлен файл: {0}", targetFile);
                    }
                }
            }
        }
        static void WorkingWithZip()
        {
            Compress("tbdu.xml", "myzip.zip");


            Console.WriteLine("Имя файла для сжатия:");
            var path = Console.ReadLine();
            FileInfo fileInf = new FileInfo(path);
            if (!fileInf.Exists)
                return;
            // Создать архив в формате zip
            // Добавить файл, выбранный пользователем, в архив
            Compress(path, "myzip2.zip");

            Console.WriteLine("Разархивирование");
            Decompress("myzip.zip", $"Decompress_{path}");

            // Удалить файл и архив
            Console.WriteLine("Удаление");
            File.Delete($"Decompress_{path}.zip");
            File.Delete("myzip2.zip");
            File.Delete("myzip.zip");
        }
        static async Task Main(string[] args)
        {
            Console.WriteLine("|---Практическое задание. Файловая система---|");
            switch (Console.ReadLine())
            {
                case "help":
                    Console.WriteLine("   info - Вывести информацию в консоль о логических дисках, именах, метке тома, размере и типе файловой системы.\n" +
                        "   file - Работа с файлами\n" +
                        "   json - Работа с форматом JSON\n" +
                        "   xml - Работа с форматом XML\n" +
                        "   zip - Работа с zip архивом");
                    break;

                case "info":
                    InfoDrive();
                    break;

                case "file":
                    WorkingWithFiles();
                    break;

                case "json":
                    await WorkingWithJson();
                    break;

                case "xml":
                    WorkingWithXML();
                    break;

                case "zip":
                    WorkingWithZip();
                    break;

            }
                
        }
    }
}

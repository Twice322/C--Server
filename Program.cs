using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Drawing;
using System.IO;


// Данная программа реализует передачу данных через стэк протоколов TCP/IP 

/// <Принцип>
/// 
/// Клиент подключается к серверу и отправляет на сервер изображение, полученное
/// с камеры графического движка Unity 3D, на стороне сервера происходит принятие байтов,
/// полученого изображения, и конвертация их в изображение формата .png
/// 
/// </Принцип>


namespace Server
{

    static class Program
    {
        static void Main()
        {
            // Иницилизурем класс Server 
            Server server = new Server();

            // Используя метод класса Server запускаем локальный сервер
            server.StartServer();

            // Используя метод класса Server подключаем к локальному серверу пользователя
            server.ConnectUserToServer();

            // Используя метод класса Server получаем данные от пользователя
            server.GetDataFromClient();


        }

    }

    class Server
    {
        // Серверные переменные
        private readonly string _IP = "127.0.0.1";
        private readonly int _PORT = 3000;
        private NetworkStream stream;
        private TcpClient client;
        private TcpListener _SERVER;

        // Переменные, которые используются для формирования пути папки, в которой будут храниться изображения
        private static readonly string _FILEPATH = @"D:\data";
        private static readonly string _FILENAME = @"\Screenshot";
        private static readonly string _FILESCOUNT = (new DirectoryInfo(_FILEPATH).GetFiles().Length + 1).ToString();
        private static readonly string _FILETYPE = @".png";

        // С помощью функции GenerateFileNameAndPath получаем абсолютный путь нашего изображения
        public string _FULLFILEPATH = GenerateFileNameAndPath(_FILEPATH, _FILENAME, _FILETYPE, _FILESCOUNT);
        
        
        
        
        // Метод класса Server, который реализует запуск локального сервера 
        public void StartServer()
        {
            _SERVER = new TcpListener(IPAddress.Parse(_IP), _PORT);
            Console.WriteLine("Запускаем сервер на " + _IP);
            _SERVER.Start();
            Console.WriteLine("Сервер успешно запущен на IP: " + _IP);
        }


        // Метод класса Server, который реализует подключение пользователя к локальному серверу
        public void ConnectUserToServer()
        {
            client = _SERVER.AcceptTcpClient();
            stream = client.GetStream();
            Console.WriteLine("Пользователь подключился");
        }


        // Метод класса Server, который реализует полученние данных от пользователя

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:Проверка совместимости платформы", Justification = "<Ожидание>")]
        public void GetDataFromClient()
        {
            Console.WriteLine("Получаем данные от пользователя");

            // Создаем массив байтов для хранения полученной информации
            byte[] data = new byte[2000000];

            // Создаем лист байтов для хранения отдельно взятого байта
            List<byte> newData = new List<byte>();
            
            // Считываем количество байтов, полученных от клиента
            Int32 bytesAmount = stream.Read(data, 0, data.Length);

            // Заполняем список байтам до тех пора пока они есть
            if (bytesAmount > 0)
            {
                for (int i = 0; i < data.Length; i++)
                {
                    newData.Add(data[i]);
                }
            }

            // Из байтов формируем изображение
            using Bitmap temp = new Bitmap(ByteArrayToImage(newData.ToArray()));

            // Сохраняем изображение по указанному пути
            temp.Save(_FULLFILEPATH);

            Console.WriteLine("Данные от пользователя получены");
        }

        // Метод класса Server, преобразующий байты в изображение
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:Проверка совместимости платформы", Justification = "<Ожидание>")]
        private static Image ByteArrayToImage(byte[] bytesArr)
        {
            using MemoryStream memstr = new(bytesArr);
            Bitmap img = (Bitmap)Image.FromStream(memstr);
            return img;
        }


        // Метод класса Server, который возвращает относительный путь до изображения
        public static string GenerateFileNameAndPath(string filePath, string fileName, string fileExtension, string filesCountInDir)
        {
            return filePath + fileName + filesCountInDir + fileExtension;
        }
    }


}

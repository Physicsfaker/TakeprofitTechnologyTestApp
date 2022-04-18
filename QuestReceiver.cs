namespace TakeprofitTechnologyTestApp;

using System;
using System.Text;
using System.Net;
using System.Net.Sockets;

public sealed class QuestReceiver
{
    public static async void GetQuest(string serverAdrs, int serverPort)
    {
        try
        {
            // адрес и порт сервера, к которому будем подключаться
            int port = serverPort; // порт сервера
            string address = serverAdrs; // адрес сервера

            Console.OutputEncoding = Encoding.UTF8;

            IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse(address), port);
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            // подключаемся к удаленному хосту
            socket.Connect(ipPoint);
            string message = "Greetings\n";
            byte[] data = Encoding.UTF8.GetBytes(message);
            socket.Send(data);

            // получаем ответ
            data = new byte[1024 * 1024 * 3]; // буфер для ответа
            StringBuilder builder = new StringBuilder();
            int bytes = 0; // количество полученных байт

            do
            {
                bytes = socket.Receive(data, data.Length, 0);
                builder.Append(CodePagesEncodingProvider.Instance.GetEncoding("koi8r")?.GetString(data, 0, bytes));
            } while (socket.Available > 0);

            Console.WriteLine("ответ сервера: " + builder.ToString());

            using (StreamWriter str = new StreamWriter("zadanie.txt", false))
            {
                await str.WriteLineAsync(builder.ToString());
            }

            // закрываем сокет
            socket.Shutdown(SocketShutdown.Both);
            socket.Close();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

        Console.Read();
    }
}
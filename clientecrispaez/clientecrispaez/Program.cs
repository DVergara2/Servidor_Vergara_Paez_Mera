


using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;

class Client
{
    private static TcpClient client;
    private static NetworkStream stream;
    private static string username;

    static void Main(string[] args)
    {
        try
        {
            Console.Write("Ingresar nombre de usuario: ");
            username = Console.ReadLine();

            client = new TcpClient("172.20.11.5", 8888);
            stream = client.GetStream();

            Thread receiveThread = new Thread(new ThreadStart(ReceiveMessages));
            receiveThread.Start();

            Console.WriteLine("Cliente conectado. Puede empezar a enviar mensajes...");

            while (true)
            {
                string message = Console.ReadLine();
                SendMessage($"{DateTime.Now:G} {username}: {message}");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Error: " + e.Message);
        }
        finally
        {
            stream.Close();
            client.Close();
        }
    }

    private static void SendMessage(string message)
    {
        byte[] buffer = Encoding.ASCII.GetBytes(message);
        stream.Write(buffer, 0, buffer.Length);
        Console.WriteLine($"[{DateTime.Now:G}] Mensaje enviado: {message}");
    }

    private static void ReceiveMessages()
    {
        byte[] buffer = new byte[1024];
        int bytesRead;

        try
        {
            while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) != 0)
            {
                string message = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                Console.WriteLine($"[{DateTime.Now:G}] Mensaje recibido: {message}");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Error: " + e.Message);
        }
    }
}
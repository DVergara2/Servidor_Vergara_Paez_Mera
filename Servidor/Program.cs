using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

class Program
{
    static void Main(string[] args)
    {
        TcpListener server = null;
        try
        {
            // Establece el servidor para escuchar en el puerto 13000
            Int32 port = 13004;
            IPAddress localAddr = IPAddress.Parse("172.20.11.5");
            server = new TcpListener(localAddr, port);

            // Inicia el servidor
            server.Start();

            // Buffer para leer datos
            Byte[] bytes = new Byte[256];
            String data = null;

            // Loop infinito para aceptar conexiones
            while (true)
            {
                Console.WriteLine("Esperando una conexión... ");

                // Acepta una conexión de cliente
                TcpClient client = server.AcceptTcpClient();
                Console.WriteLine("Conectado!");

                // Crea un nuevo thread para manejar al cliente
                Thread clientThread = new Thread(() => HandleClient(client));
                clientThread.Start();
            }
        }
        catch (SocketException e)
        {
            Console.WriteLine("SocketException: {0}", e);
        }
        finally
        {
            // Detiene el servidor
            server.Stop();
        }

        Console.WriteLine("\nPresiona ENTER para continuar...");
        Console.Read();
    }

    static void HandleClient(TcpClient client)
    {
        byte[] bytes = new byte[256];
        string data = null;

        NetworkStream stream = client.GetStream();

        int i;

        // Loop para recibir todos los datos enviados por el cliente
        while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
        {
            // Traduce los datos recibidos a una cadena de texto
            data = Encoding.ASCII.GetString(bytes, 0, i);
            Console.WriteLine("Recibido: {0}", data);

            string message = "JAJAJA si soy";
            data = message;
            // Procesa los datos enviados por el cliente
            data = data.ToUpper();

            byte[] msg = Encoding.ASCII.GetBytes(data);

            // Envía los datos de vuelta al cliente
            stream.Write(msg, 0, msg.Length);
            Console.WriteLine("Enviado: {0}", data);
        }

        // Cierra la conexión con el cliente
        client.Close();
    }
}

/*using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

class Program
{
    static List<TcpClient> clients = new List<TcpClient>();

    static void Main(string[] args)
    {
        TcpListener server = null;
        try
        {
            // Establece el servidor para escuchar en el puerto 13004
            Int32 port = 13004;
            IPAddress localAddr = IPAddress.Parse("172.20.11.5");
            server = new TcpListener(localAddr, port);

            // Inicia el servidor
            server.Start();

            Console.WriteLine("Servidor iniciado. Esperando conexiones...");

            while (true)
            {
                // Acepta una conexión de cliente
                TcpClient client = server.AcceptTcpClient();
                Console.WriteLine("Cliente conectado!");

                // Agrega el cliente a la lista de clientes
                clients.Add(client);

                // Utiliza Task.Run para manejar al cliente de manera asíncrona
                Task.Run(() => HandleClient(client));
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

        Console.WriteLine("\nPresiona ENTER para salir...");
        Console.Read();
    }

    static void HandleClient(TcpClient client)
    {
        try
        {
            // Buffer para leer datos
            byte[] bytes = new byte[256];
            string data;

            using (NetworkStream stream = client.GetStream())
            {
                int bytesRead;

                // Loop infinito para recibir y enviar datos al cliente
                while (true)
                {
                    bytesRead = stream.Read(bytes, 0, bytes.Length);

                    // Si no se reciben bytes, el cliente se desconectó
                    if (bytesRead == 0)
                        break;

                    // Traduce los datos recibidos a una cadena de texto
                    data = Encoding.ASCII.GetString(bytes, 0, bytesRead);
                    Console.WriteLine("Recibido de cliente {0}: {1}", client.Client.RemoteEndPoint, data);

                    // Envía el mensaje a todos los otros clientes
                    SendMessageToAllClients(data, client);
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Excepción en HandleClient: {0}", e);
        }
        finally
        {
            // Remueve al cliente de la lista
            clients.Remove(client);

            // Cierra la conexión con el cliente
            client.Close();
            Console.WriteLine("Cliente desconectado: {0}", client.Client.RemoteEndPoint);
        }
    }

    static void SendMessageToAllClients(string message, TcpClient sender)
    {
        // Convierte el mensaje en bytes
        byte[] bytes = Encoding.ASCII.GetBytes(message);

        // Itera sobre todos los clientes y envía el mensaje, excepto al remitente
        foreach (TcpClient client in clients)
        {
            if (client != sender)
            {
                try
                {
                    NetworkStream stream = client.GetStream();
                    stream.Write(bytes, 0, bytes.Length);
                    Console.WriteLine("Enviado a cliente {0}: {1}", client.Client.RemoteEndPoint, message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error al enviar mensaje a cliente {0}: {1}", client.Client.RemoteEndPoint, ex.Message);
                }
            }
        }
    }
}*/
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

class Server
{
    private static List<TcpClient> clients = new List<TcpClient>();

    static void Main(string[] args)
    {
        TcpListener server = new TcpListener(IPAddress.Any, 8888);
        server.Start();
        Console.WriteLine("Servidor iniciado...");

        while (true)
        {
            TcpClient client = server.AcceptTcpClient();
            clients.Add(client);
            Console.WriteLine("Cliente conectado...");

            Thread clientThread = new Thread(() => HandleClient(client));
            clientThread.Start();
        }
    }

    private static void HandleClient(TcpClient client)
    {
        NetworkStream stream = client.GetStream();
        byte[] buffer = new byte[1024];
        int bytesRead;

        try
        {
            while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) != 0)
            {
                string message = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                Console.WriteLine("Mensaje recibido: " + message);
                BroadcastMessage(message, client);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Error: " + e.Message);
        }
        finally
        {
            clients.Remove(client);
            client.Close();
        }
    }

    private static void BroadcastMessage(string message, TcpClient senderClient)
    {
        byte[] buffer = Encoding.ASCII.GetBytes(message);

        foreach (var client in clients)
        {
            if (client != senderClient)
            {
                NetworkStream stream = client.GetStream();
                stream.Write(buffer, 0, buffer.Length);
            }
        }
    }
}
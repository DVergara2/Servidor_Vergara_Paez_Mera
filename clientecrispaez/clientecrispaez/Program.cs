



using System;
using System.Net.Sockets;
using System.Text;

class Client
{
    static void Main(string[] args)
    {
        try
        {
            // Conecta al servidor en el puerto 13004
            Int32 port = 13004;
            TcpClient client = new TcpClient("172.20.11.5", port);

            // Obtiene el stream para leer y escribir datos
            NetworkStream stream = client.GetStream();

            // Mensaje para enviar inicialmente
            string initialMessage = "eres puto";
            byte[] initialData = Encoding.ASCII.GetBytes(initialMessage);

            // Envía el mensaje inicial al servidor
            stream.Write(initialData, 0, initialData.Length);
            Console.WriteLine("Enviado: {0}", initialMessage);

            // Buffer para leer la respuesta del servidor
            byte[] data = new byte[256];
            String responseData = String.Empty;

            // Lee la respuesta inicial del servidor
            Int32 initialBytes = stream.Read(data, 0, data.Length);
            responseData = Encoding.ASCII.GetString(data, 0, initialBytes);
            Console.WriteLine("Recibido inicialmente: {0}", responseData);

            // Bucle para enviar mensajes desde la consola al servidor
            while (true)
            {
                // Leer mensaje desde la consola
                string input = Console.ReadLine();

                if (input.ToLower() == "exit")
                    break;

                // Convertir mensaje a bytes
                data = Encoding.ASCII.GetBytes(input);

                // Enviar mensaje al servidor
                stream.Write(data, 0, data.Length);
                Console.WriteLine("Enviado: {0}", input);
            }

            // Cierra todo
            stream.Close();
            client.Close();
        }
        catch (ArgumentNullException e)
        {
            Console.WriteLine("ArgumentNullException: {0}", e);
        }
        catch (SocketException e)
        {
            Console.WriteLine("SocketException: {0}", e);
        }

        Console.WriteLine("\nPresiona ENTER para continuar...");
        Console.Read();
    }
}
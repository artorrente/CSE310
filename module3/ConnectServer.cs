using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

public class ServerJoin
{
    private TcpClient client;
    private NetworkStream stream;

    public async Task JoinServer(string ip, int port)
    {
        try
        {
            client = new TcpClient();
            Console.WriteLine($"connecting to {ip}:{port}...");
            await client.ConnectAsync(ip, port);
            Console.WriteLine($"Connected to server {ip}:{port}");

            stream = client.GetStream();

            // Start a task to continuously listen for messages from the server
            _ = Task.Run(() => ListenForServerMessages()); // This runs a task that makes it so that the client will continuously recieve messages from the server, instead of only getting one or two messages.

            while (true) // loop for inputting stuff
            {
                await Task.Delay(100); // without this, the program crashes because the code gets more and more resource heavy, until the computer cannot handle to program anymore, and stops it
            }
        }
        catch (SocketException e) //this whole thing closes the stream if there is a socket exception
        {
            Console.WriteLine($"SocketException: {e.Message}");
        }
        catch (Exception e)
        {
            Console.WriteLine($"General Exception: {e.Message}");
        }
        finally
        {
            stream.Close();
            client.Close();
            Console.WriteLine("Disconnected from server.");
        }
    }

    private async Task ListenForServerMessages()
    {
        byte[] buffer = new byte[2048];
        try
        {
            while (client.Connected)
            {
                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                if (bytesRead == 0) // Server disconnected
                {
                    Console.WriteLine("Server disconnected.");
                    break;
                }

                string receivedMessage = Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim();
                Console.WriteLine($"   {receivedMessage}"); // Print received message

                if (receivedMessage.Contains("it's your turn. Please pick a number"))
                {
                    Console.Write("Your move: ");
                    string moveInput = Console.ReadLine();
                    await SendMessageToServer(moveInput);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error listening for messages: {ex.Message}");
        }
    }

    private async Task SendMessageToServer(string message)
    {
        try
        {
            byte[] data = Encoding.UTF8.GetBytes(message + "\n"); 
            await stream.WriteAsync(data, 0, data.Length);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending message to server: {ex.Message}");
        }
    }
}

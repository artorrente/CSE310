using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks; // For asynchronous operations

public class ServerHost
{
    private TcpListener listener;
    private Dictionary<int, TcpClient> connectedClients = new Dictionary<int, TcpClient>(); // having this as a dictionary makes it easier to make a TcpClient have a player number
    private Game ticTacToeGame;

    private int currentPlayerTurn = 1;
    private readonly object gameLock = new object();
    public ServerHost(int port)
    {
        ticTacToeGame = new Game();
        IPAddress localAddr = IPAddress.Parse("127.0.0.1");
        listener = new TcpListener(localAddr, port);
    }

    public async Task Start()
    {
        listener.Start();
        Console.WriteLine("Server Created! Waiting for players to join...");

        // Loop to accept client connections
        while (connectedClients.Count < 2) 
        {
            Console.WriteLine($"Current players: {connectedClients.Count}/2. Waiting for new client...");
            TcpClient client = await listener.AcceptTcpClientAsync(); //this line keeps the listener open with the await command, and accepts clients as they come in
            int playerNumber = connectedClients.Count + 1;
            connectedClients.Add(playerNumber, client);

            Console.WriteLine($"Player {playerNumber} has joined the game!");

            SendClientMessage(client, $"You are Player {playerNumber}.");
            SendClientMessage(client, $"Current players: {connectedClients.Count}/2");

            if (connectedClients.Count < 2)
            {
                Console.WriteLine("Still waiting for another player...");
                SendClientMessage(client, "Waiting for another player to join...");
            }
            else
            {
                Console.WriteLine("Both players connected. Starting game...");
                _ = Task.Run(() => GameLoop());
            }
        }
    }

    private async Task GameLoop()
    {
        Console.WriteLine("\n--- Game Started ---");
        BroadcastGameInfo(connectedClients.Values.ToList());// this piece of code 

        bool win = false;
        bool draw = false;

        while (!win && !draw)
        {
            // Prompt current player for input
            TcpClient currentPlayerClient = connectedClients[currentPlayerTurn];
            await SendClientMessage(currentPlayerClient, $"Player {currentPlayerTurn}, it's your turn. Please pick a number (1-9):");

            // Broadcast turn info to the other player as well
            foreach (var clientEntry in connectedClients)
            {
                if (clientEntry.Key != currentPlayerTurn)
                {
                    await SendClientMessage(clientEntry.Value, $"Waiting for Player {currentPlayerTurn}'s move...");
                }
            }

            // Get move from current player
            string rawInput = await ReceiveClientInput(currentPlayerClient);
            int chosenNumber;

            bool isValidMove = false;
            int opponentPlayer = (currentPlayerTurn == 1) ? 2 : 1; // this line decides who is against who, according to who! meaning who can move when!

            lock (gameLock)
            {
                if (int.TryParse(rawInput, out chosenNumber))
                {
                    if (ticTacToeGame.TryMakeMove(currentPlayerTurn, chosenNumber))
                    {
                        isValidMove = true;
                        Console.WriteLine($"[Server] Player {currentPlayerTurn} chose: {chosenNumber}");

                        // Check for win/draw after valid move
                        win = ticTacToeGame.CheckWin(currentPlayerTurn);
                        if (!win)
                        {
                            draw = ticTacToeGame.CheckDraw();
                        }
                    }
                    else
                    {
                        SendClientMessage(currentPlayerClient, "Invalid choice. That spot is already taken or out of range. Try again.");
                    }
                }
                else
                {
                    SendClientMessage(currentPlayerClient, "That was not a number. Please input a number from 1 to 9.");
                }
            } // End lock

            if (isValidMove)
            {
                // this line broadcasts the players moves, and porevious moves they have made
                BroadcastGameInfo(connectedClients.Values.ToList());

                if (win) //if they win, it sends out this message
                {
                    string winMessage = $"Player {currentPlayerTurn} Wins!"; 
                    Console.WriteLine($"[Server] {winMessage}"); // Server's console
                    BroadcastToAllClients(winMessage);  //prints to players consoles
                }
                else if (draw)
                {
                    string drawMessage = "It's a Draw!";
                    Console.WriteLine($"[Server] {drawMessage}"); // Server's console
                    BroadcastToAllClients(drawMessage);
                }
                else
                {
                    // Switch turn to the other player
                    currentPlayerTurn = (currentPlayerTurn == 1) ? 2 : 1;
                }
            }
        }

        Console.WriteLine("\n--- Game Over ---");
        // Close client connections after game ends
        CloseAllClientConnections();
    }

    private async Task SendClientMessage(TcpClient client, string message)
    {
        try
        {
            NetworkStream stream = client.GetStream(); //gets information sent from client, in this case 
            byte[] data = Encoding.UTF8.GetBytes(message + "\n"); // Add newline for client parsing
            await stream.WriteAsync(data, 0, data.Length);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending message to client: {ex.Message}");
            RemoveClient(client); // Remove disconnected client
        }
    }

    private async Task<string> ReceiveClientInput(TcpClient client) //probably the most important part, recieves user input
    {
        try
        {
            NetworkStream stream = client.GetStream();
            byte[] buffer = new byte[1024];
            int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);

            if (bytesRead == 0) // Client disconnected
            {
                Console.WriteLine($"Client {((IPEndPoint)client.Client.RemoteEndPoint).Address} disconnected.");
                RemoveClient(client);
                return null;
            }

            string receivedData = Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim();
            return receivedData;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error receiving from client: {ex.Message}");
            RemoveClient(client); // Remove disconnected client
            return null;
        }
    }

    // Sends the current game state (board, choices, etc.) to a list of clients
    private async Task BroadcastGameInfo(List<TcpClient> clientsToSendTo)
    {
        lock (gameLock) // prevents data corruption within the server.
        {
            StringBuilder messageBuilder = new StringBuilder();
            messageBuilder.AppendLine(ticTacToeGame.GetBoardDisplay());
            messageBuilder.AppendLine($"Player One's choices (X): {string.Join(", ", ticTacToeGame.PlayerOneChoices)}");
            messageBuilder.AppendLine($"Player Two's choices (O): {string.Join(", ", ticTacToeGame.PlayerTwoChoices)}");
            messageBuilder.AppendLine($"Remaining choices: {string.Join(", ", ticTacToeGame.GameList)}");

            string gameInfoMessage = messageBuilder.ToString();

            // Display on server's own console
            Console.WriteLine("\n" + gameInfoMessage);

            // Sends stuff to clients
            foreach (var client in clientsToSendTo)
            {
                _ = SendClientMessage(client, gameInfoMessage);
            }
        }
    }

    // Helper to send a message to all connected clients
    private async Task BroadcastToAllClients(string message)
    {
        Console.WriteLine($"[Server] Broadcasting: {message}"); // Server's own console
        foreach (var clientEntry in connectedClients)
        {
            _ = SendClientMessage(clientEntry.Value, message);
        }
    }

    private void RemoveClient(TcpClient client)
    {
        lock (connectedClients) // Protect list modification
        {
            var entry = connectedClients.FirstOrDefault(x => x.Value == client);
            if (entry.Value != null)
            {
                connectedClients.Remove(entry.Key);
                client.Close();
                Console.WriteLine($"Player {entry.Key} disconnected.");
            }
        }
    }

    private void CloseAllClientConnections()
    {
        foreach (var clientEntry in connectedClients.Values.ToList()) // ToList for safe iteration
        {
            clientEntry.Close();
        }
        connectedClients.Clear();
        listener.Stop();
    }
}
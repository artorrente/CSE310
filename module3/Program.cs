using System;
using System.Threading.Tasks;

public class Program
{
    public static async Task Main(string[] args)
    {
        Console.WriteLine("Do you want to run as (S)erver or (C)lient?");
        string choice = Console.ReadLine().ToUpper();

        if (choice == "S")
        {
            Console.WriteLine("Starting Server...");
            ServerHost server = new ServerHost(19880);
            await server.Start();
        }
        else if (choice == "C")
        {
            Console.WriteLine("Starting Client...");
            ServerJoin client = new ServerJoin();
            await client.JoinServer("127.0.0.1", 19880);
        }
        else
        {
            Console.WriteLine("Invalid choice. Exiting.");
        }
        Console.WriteLine("Press any key to exit.");
        Console.ReadKey();
    }
}
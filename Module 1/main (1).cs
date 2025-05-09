using System;
using System.Threading;
using System.Collections.Generic;

class Menu {
	static void Main() {
		bool MenuOp = true;
		while (MenuOp)  {
			Console.WriteLine("Menu:");
			Console.WriteLine("1). Play Game");
			Console.WriteLine("2). Quit");
			Console.WriteLine("3). Credits");

			string userInput = Console.ReadLine();
			int userNum = Convert.ToInt32(userInput);

			if (userNum == 1)
			{
				Console.Clear();
				Console.WriteLine("You Have Chosen to Play the Game!");
				Game ticTacToeGame = new Game(); // Create Game object here
				ticTacToeGame.playGame();

				MenuOp = false;
			}
			else if (userNum == 2)
			{
				Console.WriteLine("You Have Chosen to Quit the Game!");
				MenuOp = false;
			}
			else if (userNum == 3)
			{
				Console.Clear();
				Console.WriteLine("Everything: Alexander Torrente");
				Thread.Sleep(2000);
				Console.Clear();
			}
			else
			{
				Console.WriteLine("Invalid input, please select one of the options");
			}
		}
	}
}
class Game {
	List<int> gameList = new List<int> {1, 2, 3, 4, 5, 6, 7, 8, 9};
	Random random = new Random();
	List<int> userList = new List<int>();
	List<int> CompList = new List<int>();
	char[] board = {'1', '2', '3', '4', '5', '6', '7', '8', '9'};
	
	public void drawBoard()
    {
        Console.WriteLine($"{board[0]}|{board[1]}|{board[2]}");
        Console.WriteLine("-----");
        Console.WriteLine($"{board[3]}|{board[4]}|{board[5]}");
        Console.WriteLine("-----");
        Console.WriteLine($"{board[6]}|{board[7]}|{board[8]}");
    }
    
	public void playGame()
	{
		bool win = false;
		bool loose = false;
		
		while (!win && !loose)
		{
		    drawBoard();
			Console.WriteLine("Please pick a number");
			string UserIn = Console.ReadLine();
			int number;
			bool success = int.TryParse(UserIn, out number);

			if (success)
			{
				if (gameList.Contains(number))
				{
				    
					userList.Add(number);
					gameList.Remove(number);
					board[number - 1] = 'X';
					Console.WriteLine($"You entered: {number}");
                    
                    int randomIndex = random.Next(gameList.Count); // Get a random index from the list
                    int computerChoice = gameList[randomIndex]; // Select the number at that index
                    
                    Console.WriteLine($"Computer picked: {computerChoice}");
                    CompList.Add(computerChoice);
                    gameList.Remove(computerChoice);
                    board[computerChoice - 1] = 'O';
                    Console.WriteLine($"Your choices: {string.Join(", ", userList)}");
                    Console.WriteLine($"Computer choices: {string.Join(", ", CompList)}");
                    Console.WriteLine($"Remaining choices: {string.Join(", ", gameList)}");
                    
                    
				}
				else
				{
					Console.WriteLine("Please input a number between 1 and 9");
				}
			}
			else
			{
				Console.WriteLine("That was not a number, Please input a number from 1 to 9");
			}
			if ((userList.Contains(1) && userList.Contains(2) && userList.Contains(3)) || 
                (userList.Contains(4) && userList.Contains(5) && userList.Contains(6)) || 
                (userList.Contains(7) && userList.Contains(8) && userList.Contains(9)) || 
                (userList.Contains(1) && userList.Contains(4) && userList.Contains(7)) || 
                (userList.Contains(2) && userList.Contains(5) && userList.Contains(8)) || 
                (userList.Contains(3) && userList.Contains(6) && userList.Contains(9)) || 
                (userList.Contains(3) && userList.Contains(5) && userList.Contains(7)) || 
                (userList.Contains(1) && userList.Contains(5) && userList.Contains(9)))
                {
                    Console.WriteLine("You win!!");
                    win = true;
                }
            if ((CompList.Contains(1) && CompList.Contains(2) && CompList.Contains(3)) || 
                (CompList.Contains(4) && CompList.Contains(5) && CompList.Contains(6)) || 
                (CompList.Contains(7) && CompList.Contains(8) && CompList.Contains(9)) || 
                (CompList.Contains(1) && CompList.Contains(4) && CompList.Contains(7)) || 
                (CompList.Contains(2) && CompList.Contains(5) && CompList.Contains(8)) || 
                (CompList.Contains(3) && CompList.Contains(6) && CompList.Contains(9)) || 
                (CompList.Contains(3) && CompList.Contains(5) && CompList.Contains(7)) || 
                (CompList.Contains(1) && CompList.Contains(5) && CompList.Contains(9)))
                {
                    Console.WriteLine("Computer wins! You lose.");
                    loose = true;
                }
		}
	}
}
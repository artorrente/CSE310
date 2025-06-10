using System;
using System.Collections.Generic;
using System.Linq; // For .Contains() and .SequenceEqual() later

public class Game
{
    public char[] Board { get; private set; }
    public List<int> GameList { get; private set; } // Numbers not yet picked
    public List<int> PlayerOneChoices { get; private set; } // Player 1's picked numbers
    public List<int> PlayerTwoChoices { get; private set; } // Player 2's picked numbers

    public Game()
    {
        Board = new char[] { '1', '2', '3', '4', '5', '6', '7', '8', '9' };
        GameList = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        PlayerOneChoices = new List<int>();
        PlayerTwoChoices = new List<int>();
    }

    // Returns a string representation of the board for display
    public string GetBoardDisplay()
    {
        return $@"
   {Board[0]} | {Board[1]} | {Board[2]}
  ---+---+---
   {Board[3]} | {Board[4]} | {Board[5]}
  ---+---+---
   {Board[6]} | {Board[7]} | {Board[8]}
";
    }

    // Attempts to make a move. Returns true if successful, false otherwise (e.g., invalid choice)
    public bool TryMakeMove(int playerNumber, int choice)
    {
        if (!GameList.Contains(choice) || choice < 1 || choice > 9)
        {
            return false; // Invalid choice (already taken or out of range)
        }

        char playerMark = (playerNumber == 1) ? 'X' : 'O';
        List<int> currentPlayerChoices = (playerNumber == 1) ? PlayerOneChoices : PlayerTwoChoices;

        Board[choice - 1] = playerMark;
        currentPlayerChoices.Add(choice);
        GameList.Remove(choice);

        // Sort choices for consistent display, though not strictly necessary for game logic
        currentPlayerChoices.Sort();

        return true;
    }

    // Checks if a player has won
    public bool CheckWin(int playerNumber)
    {
        List<int> choices = (playerNumber == 1) ? PlayerOneChoices : PlayerTwoChoices;

        // Define all winning combinations
        int[][] winCombinations = new int[][]
        {
            new int[] {1, 2, 3}, new int[] {4, 5, 6}, new int[] {7, 8, 9}, // Rows
            new int[] {1, 4, 7}, new int[] {2, 5, 8}, new int[] {3, 6, 9}, // Columns
            new int[] {1, 5, 9}, new int[] {3, 5, 7}                      // Diagonals
        };

        foreach (var combo in winCombinations)
        {
            if (combo.All(c => choices.Contains(c)))
            {
                return true; // This player has a winning combination
            }
        }
        return false;
    }

    // Checks if the game is a draw
    public bool CheckDraw()
    {
        return GameList.Count == 0 && !CheckWin(1) && !CheckWin(2);
    }
}
using System;
using GameEngine;
namespace ConsoleUI
{
    public static class GameUI
    {
        private static readonly string _vertiacalSeparator = "|";
        private static readonly string _horizontalSeparator = "-";
        private static readonly string _centralSeparator = "+";
        public static void PrintBoard(Game game)
        {
            var board = game.GetBoard();
            var line = "";
            var _horizontalBorder = new String('-', game.BoardWidth*4+1);
            line += _horizontalBorder;
            Console.WriteLine(line);
            for (int y = 0; y < game.BoardHeight; y++)
            {
                line = "";
                line += _vertiacalSeparator;
                    for (int x = 0; x < game.BoardWidth; x++)
                    {
                        line += " " + GetSingleState(board[y, x]) + " ";
                        line += _vertiacalSeparator;
                    }
                    Console.WriteLine(line);
                    if (y < game.BoardHeight - 1)
                    {
                        line = "";
                        line += _vertiacalSeparator;
                        for (int x = 0; x < game.BoardWidth - 1; x++)
                        {
                        line += _horizontalSeparator + _horizontalSeparator + _horizontalSeparator;
                        line += _centralSeparator;
                        }
                        line += _horizontalSeparator + _horizontalSeparator + _horizontalSeparator;
                        line += _vertiacalSeparator;
                        Console.WriteLine(line);
                    }   
            }
            line = "";
            line += _horizontalBorder;
            Console.WriteLine(line);
        }
       
        public static string GetSingleState(CellState state)
        {
            switch (state)
            {
                case CellState.Empty:
                    return " ";
                case CellState.O:
                    return "0";
                case CellState.X:
                    return "X";
            }
            return "";
        }
    }
}

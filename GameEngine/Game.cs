using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
namespace GameEngine
{
    public class Game
    {
        private static GameSettings _settings;
        private static int[,] directions = new int[,] { { -1, -1 }, { 1, 1 }, { -1, 1 }, { 1, -1 }, { 0, 1 }, { 0, -1 }, { 1, 0 }, { -1, 0 } };
        private CellState[,] Board { get; set; }
        private bool _moveTurn;
        public int BoardWidth { get; }
        public int BoardHeight { get; }
        private readonly List<CheckLine> _playerZeroMoves;
        private readonly List<CheckLine> _playerOneMoves;
        public string Title { get; set; }
        private readonly int _elementsInRow;
        public Game(GameSettings settings)
        {
            _settings = settings;
            _playerOneMoves = settings.PlayerOneMoves;
            _playerZeroMoves = settings.PlayerZeroMoves;
            _moveTurn = false;
            BoardHeight = settings.BoardHeight;
            BoardWidth = settings.BoardWidth;
            _elementsInRow = BoardWidth > 7 ? (BoardWidth - 3 - (BoardWidth/4)) : 3;  // It's stupid. We need specific math function, but it's just 
            if (BoardHeight < 4 || BoardWidth < 4)                                      // to present how computer vs computer works on somewhat 
            {                                                                           // 10-12 width, 15+ will be full every time
                throw new ArgumentException(message: "Board size has to be at least 3 by 3");
            }
            Board = settings.Board;
        }

        static (int getX, int getY) GetNextElementByCurrDirection(List<CheckLine> item, int CurrDirection, int IndexOfElem)
        {
            int x = 0, y = 0;
            x = item[IndexOfElem].X + directions[CurrDirection, 0];     // determining next and previous element of 2d array for every direction
            y = item[IndexOfElem].Y + directions[CurrDirection, 1];
            return (x, y);
        }

        public CellState[,] GetBoard()
        {
            var result = new CellState[BoardHeight, BoardWidth];
            Array.Copy(Board, result, Board.Length);
            return result;
        }


        private static void RecursiveAddition(List <CheckLine> item, int indexOfElem, int permanentDir)
        {
            int x = 0, y = 0;                                                               //recursive line checking
            (x, y) = GetNextElementByCurrDirection(item, permanentDir, indexOfElem);
            for(var i = 0; i < item.Count; i++)
            {
                if (item[i].X == x && item[i].Y == y)
                {
                    RecursiveAddition(item, i, permanentDir);
                }
            }
            item[indexOfElem].Surroundings[permanentDir / 2] = item[item.Count - 1].Surroundings[permanentDir / 2];
        }



        private bool GetSurrounding(List<CheckLine> item)
        {
            GameConfigHandler.SaveConfig(_settings, "Continue.json");
            int x = 0, y = 0;
            for (var j = 0; j < item.Count; j++)
            {
                for (var i = 0; i < directions.Length / 2; i++)   // there'is overall 8 directions but every direction is the same line 
                {                                                           // for opposite direction, so the object has only 4.
                    (x, y) = GetNextElementByCurrDirection(item, i, item.Count - 1);
                    item[item.Count - 1].Surroundings[i / 2] +=             // if there is such element arrond then we will add it to 
                        (item[j].X == x && item[j].Y == y) ? item[j].Surroundings[i / 2] : 0;           // the array of directions' power
                    //    element.surroundings[i / 2] += (element.X == x && element.Y == y) ? 1 : 0;
                    if (item[item.Count - 1].Surroundings[i / 2] > _elementsInRow)      // if there are more than 3 elements in a row than 1st/2nd player won
                    {
                        Console.Clear();
                        Console.WriteLine(item[item.Count - 1].Title + " player won!");
                        File.Delete(_settings.FileName);
                        return true;
                    }
                }
                for (var i = 0; i < directions.Length / 2; i++)
                {
                    (x, y) = GetNextElementByCurrDirection(item, i, item.Count - 1);
                    if (item[j].X == x && item[j].Y == y)
                    {
                        RecursiveAddition(item, item.Count - 1, i);
                    }
                }

            }

            return false;
        }


        private int MoveGenerating(List<CheckLine> item)
        {
            Random rnd = new Random();
            var x = 0; var outOfBoard = 0; var area = 0;

            do                                          // getting random x that not exceed board space
            {
                area++;
                outOfBoard = rnd.Next(0,BoardWidth);
            } while (Board[0, outOfBoard] != CellState.Empty && area < BoardWidth*BoardHeight);

            foreach (var element in item) {                 // define biggest number of elements in any directions
                x = element.Surroundings.Max() > x ? element.Surroundings.Max() : x;
            }
            for (var j = item.Count - 1; j >= 0; j--)
            {
                for (var i = 0; i < directions.Length / 2; i++)
                {
                    if (item[j].Surroundings[i/2] == x)         // moving through list of elements and their numbers of surrounding elements
                    {                                           // if we find those biggest we will chouse this X and add -1...1 to it to avoid 
                        do                                          // situations when there is already computer's element covering user elements
                        {
                            x = item[j].X + rnd.Next(-1, 1);       
                        } while (x < 0 || x > BoardWidth - 1);           //checking if it holds on the board (0 + rand(-1..1) can be -1 which is out of the board
                        return Board[0, x] == CellState.Empty ? x : outOfBoard;
                    }
                }
            }
            return outOfBoard;           // if it's first move or some another crazy case, we will use just random
        }

        public bool Move(int posX, bool pcMove = false, bool secondpcMove = false)
        {
            posX = pcMove ? MoveGenerating(_playerOneMoves) : posX;    // this is for move generating
            posX = secondpcMove ? MoveGenerating(_playerZeroMoves) : posX;
            for (var y = BoardHeight - 1; y >= 0; y--)
            {
                if (Board[y, posX] == CellState.Empty)
                {
                    _moveTurn = !_moveTurn;
                    Board[y, posX] = _moveTurn ? CellState.X : CellState.O;    // it's for deciding which player will make a move
                    if (_moveTurn)
                    {
                        _playerOneMoves.Add(new CheckLine(y, posX, "1st"));    //adding element to array of 1st player'/computer' elements
                        return GetSurrounding(_playerOneMoves);
                    }
                    _playerZeroMoves.Add(new CheckLine(y, posX, "2nd"));
                        return GetSurrounding(_playerZeroMoves);
                }
            }
           
            Console.Clear();
            Console.WriteLine(_moveTurn ? _playerOneMoves[0].Title : _playerZeroMoves[0].Title + " player won!");
            File.Delete(_settings.FileName);
            return true;            //if someone exseed the board he's gonna lose
        }
    }
}
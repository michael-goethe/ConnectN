using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GameEngine;
using ConsoleUI;

namespace MenuSystem
{
    public class Menu
    {
        private static GameSettings _settings;
        private readonly int _menuLevel;
        private const string TempName = "Continue.Json";
        private const string MenuCommandExit = "X";
        private const string MenuCommandReturnToMain = "M";
        public string FileName { get; set; }
        private Dictionary<string, MenuItem> _menuItemsDictionary = new Dictionary<string, MenuItem>();

        public Menu(int menuLevel = 0)
        {
            _menuLevel = menuLevel;
        }

        public string Title { get; set; }

        public Dictionary<string, MenuItem> MenuItemsDictionary
        {
            get => _menuItemsDictionary;
            set
            {
                _menuItemsDictionary = value;
                if (_menuLevel >= 1)
                {
                    _menuItemsDictionary.Add(MenuCommandReturnToMain,
                        new MenuItem() { Title = "Return to Main Menu" });
                }
                _menuItemsDictionary.Add(MenuCommandExit,
                    new MenuItem() { Title = "Exit" });
            }
        }
        


        public string Run()
        {
            var command = "";
            do
            {
                Console.Clear();
                Console.WriteLine(Title);
                Console.WriteLine("========================");

                foreach (var menuItem in MenuItemsDictionary)
                {
                    Console.Write(menuItem.Key);
                    Console.Write(" ");
                    Console.WriteLine(menuItem.Value);
                }

                Console.WriteLine("----------");
                Console.Write(">");

                command = Console.ReadLine()?.Trim().ToUpper() ?? "";
                
                var returnCommand = "";
                
                if (MenuItemsDictionary.ContainsKey(command))
                {
                    var menuItem = MenuItemsDictionary[command];

                    returnCommand = menuItem.CommandToExecute != null ? menuItem.CommandToExecute() : returnCommand;
                    
                
                if (returnCommand == MenuCommandExit)
                {
                    command = MenuCommandExit;
                }

                if (returnCommand == MenuCommandReturnToMain && _menuLevel != 0)
                {
                    command = MenuCommandReturnToMain;
                }
                if (menuItem.Title != null && command != MenuCommandReturnToMain &&
                    command != MenuCommandExit)
                {
                    switch (_menuLevel)
                    {
                        case 1 :
                            TestGame(menuItem.Title, TempName);
                            break;
                        case 2 :
                            return "";
                        case 3 :
                            NewGameCustom(command);
                            return "";
                        case 4 :
                            FileOpen(command);
                            return "";
                    }
                }
                }



            } while (command != MenuCommandExit &&
                     command != MenuCommandReturnToMain);
            
            return command;
        }
        static void NewGameCustom(string command)
        {
            switch (command)
            {
                case "1" :
                    var settings = new GameSettings();
                    GameConfigHandler.SaveConfig(settings, TempName);
                    break;
                case "2" :
                    var customSettings = new GameSettings();
                    BoardSize(customSettings);
                    break;
                default: 
                    var defSettings = new GameSettings();
                    GameConfigHandler.SaveConfig(defSettings, TempName);
                    break;
            }
        }
        

        static void FileOpen(string command)
        {
            var userInputint = 0;
            var userInputstr = "";
            var usercanceled = false;
            var fileName = "";
            DirectoryInfo d = new DirectoryInfo(Directory.GetCurrentDirectory());
            FileInfo[] Files = d.GetFiles("*.json");
            
            for(var i = 0; i < Files.Length; i++)
            {
                Console.WriteLine(i+1 + ": " + Files[i].Name);
            }
            Console.WriteLine("Input game save");
            
            //It'd be better to rewrite switch below somehow
            
            switch (command)
            {
                case "1" :
                    (fileName, usercanceled) = FileNameValidation();
                    var openedFile = GameConfigHandler.LoadConfig(fileName);
                    GameConfigHandler.SaveConfig(openedFile, TempName);
                    break;
                case "2" :
                    (userInputint, usercanceled) = GetUserIntInput("Pick the save by index", 0, Files.Length, 0);
                    var pickFile = userInputint >= 0 && userInputint < Files.Length?
                        GameConfigHandler.LoadConfig(Files[userInputint-1].Name) : new GameSettings();
                    GameConfigHandler.SaveConfig(pickFile, TempName);
                    break;
            }
        }


        static void SaveGame(GameSettings currentState)
        {
            var newFileName = "";
            var userCanceled = false;
            var uniqFile = true;
            
            DirectoryInfo d = new DirectoryInfo(Directory.GetCurrentDirectory());//Assuming Test is your Folder
            FileInfo[] Files = d.GetFiles("*.json"); //Getting Text files
            do
            {
                (newFileName, userCanceled) = FileNameValidation();
                foreach (var file in Files)
                {
                    if (file.Name == newFileName)
                    {
                        uniqFile = false;
                        Console.WriteLine("File already exists, please try again");
                    }
                }
            } while (!uniqFile);

            if (!userCanceled)
            {
                GameConfigHandler.SaveConfig(currentState, newFileName);
            }
        }

        static (string nameOfTheFile, bool canceled)  FileNameValidation()
        {
            var nameOfTheFile = "";
            Console.WriteLine("Input file name (only characters and numbers are allowed");
            Console.WriteLine("To cancel press 'C'");
            do
            {
                if (nameOfTheFile != "")
                    Console.WriteLine("Please try again");
                nameOfTheFile = Console.ReadLine();
            } while (!nameOfTheFile.All(c=>Char.IsLetterOrDigit(c) || c=='_') && nameOfTheFile != "");

            return (nameOfTheFile != "c") ? (nameOfTheFile + ".json", false) : ("", true);
        }

        static void  BoardSize(GameSettings settings)
        {
            var cancel = false;
            var n = 0; var m = 0;
            (n, cancel) = GetUserIntInput("Enter width size", 4, default, 0);
            (m, cancel) = GetUserIntInput("Enter height size", 4, default, 0);
            settings.BoardHeight = m;
            settings.BoardWidth = n;
            settings.Board = new CellState[settings.BoardHeight, settings.BoardWidth];
            GameConfigHandler.SaveConfig(settings, TempName);
        }
        
        


        static string TestGame(string playAgainst, string filename)
        {
            _settings = GameConfigHandler.LoadConfig(filename);
            var game = new Game (_settings);
            var done = false;
            do
            {
                Console.Clear();
                GameUI.PrintBoard(game);
                done = StartGame(playAgainst, game, _settings);
                GameUI.PrintBoard(game);
                // GameConfigHandler.SaveConfig(_settings);
            } while (!done);
            Console.WriteLine("Press any key to continue");
            Console.ReadLine();
            return "";
        }

        



        static bool StartGame(string gameOption, Game game, GameSettings setSettings = null)
        {
            var userXint = 0;
            var userCanceled = false;
            var done = false;

            if (gameOption == "2 Players mode" || gameOption == "Play against Computer")
            {
                Console.WriteLine("To save game press 'S'");
                (userXint, userCanceled) = GetUserIntInput("Enter X coordinate", 1, 7, 0, default, setSettings);
                
                if (userCanceled)
                {
                    Console.Clear();
                    return true;
                }
            }

            // it's bad as well. Should've created better logic for move switch 
            switch (gameOption)
            {
                case "2 Players mode":
                    done = game.Move(userXint - 1);
                    break;
                case "Play against Computer":
                    done = !done ? game.Move(userXint - 1) : done;
                    done = !done ? game.Move(0, true) : done;
                    break;
                case "Computer vs Computer (for testing)":
                    done = !done ? game.Move(0, false, true) : done;
                    done = !done ? game.Move(0, true) : done;
                    break;
                default:
                    return true;
            }
            return done;
        }
        static (int result, bool wasCanceled) GetUserIntInput(string prompt, int min, int max = 1000, int? cancelIntValue = null, string cancelStrValue = "", GameSettings save = null)
        {
            var userXint = 0;
            var userCanceled = false;
            do
            {
                Console.WriteLine(prompt);
                if (cancelIntValue.HasValue || !string.IsNullOrWhiteSpace(cancelStrValue))
                {
                    Console.WriteLine($"To cancel input enter: {cancelIntValue}" +
                                      $"{ (cancelIntValue.HasValue && !string.IsNullOrWhiteSpace(cancelStrValue) ? " or " : "") }" +
                                      $"{cancelStrValue}");
                }

                Console.Write(">");
                var consoleLine = Console.ReadLine();
                if (consoleLine == "s")
                {
                    SaveGame(save);
                    (userXint, userCanceled) = GetUserIntInput("Enter X coordinate", 1, 7, 0);
                    return (userXint, userCanceled);
                }

                if (consoleLine == cancelStrValue) return (0, true);

                if (int.TryParse(consoleLine, out var userInt))
                {
                    return userInt == cancelIntValue ? (userInt, true) : (userInt, false);
                }

                Console.WriteLine($"'{consoleLine}' cant be converted to int value!");
            } while (true);

            return (0, true);
        }

    }
}
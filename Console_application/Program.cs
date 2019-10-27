using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using ConsoleUI;
using GameEngine;
using MenuSystem;

namespace Console_application
{
    class Program
    {

        static void Main(string[] args)
        {
            Game();
        }
        
        
        
        
        

        static void Game()
        {
            
            Console.Clear();
            var openFile = new Menu(4)
            {
                Title = "Preset Board",
                MenuItemsDictionary = new Dictionary<string, MenuItem>()
                {
                    {
                        "1", new MenuItem()
                        {
                            Title = "Search by name",
                            CommandToExecute = null,
                        }
                    },
                    {
                        "2", new MenuItem()
                        {
                            Title = "Pick from the list",
                            CommandToExecute = null,
                        }
                    },
                }
            };
            var preSet = new Menu(3)
            {
                Title = "Preset Board",
                MenuItemsDictionary = new Dictionary<string, MenuItem>()
                {
                    {
                        "1", new MenuItem()
                        {
                            Title = "Default Board",
                            CommandToExecute = null,
                        }
                    },
                    {
                        "2", new MenuItem()
                        {
                            Title = "Custom Board",
                            CommandToExecute = null,
                        }
                    },
                }
            };
            var setMenu = new Menu(2)
            {
                Title = "Open game",
                FileName = "",
                MenuItemsDictionary = new Dictionary<string, MenuItem>()
                {
                    {
                        "1", new MenuItem()
                        {
                            Title = "New Game",
                            CommandToExecute = preSet.Run,
                        }
                    },
                    {
                        "2", new MenuItem()
                        {
                            Title = "Open File",
                            CommandToExecute = openFile.Run
                        }
                    },
                    {    
                        "3", new MenuItem()
                        {
                            Title = "Continue",
                            CommandToExecute = null
                        }
                    },
                },
            };
            
            var gameMenu = new Menu(1)
            {
                Title = "Start a new game of connect 4",
                MenuItemsDictionary = new Dictionary<string, MenuItem>()
                {
                    {
                        "1", new MenuItem()
                        {
                            Title = "2 Players mode",
                            CommandToExecute = setMenu.Run
                        }
                    },
                    {
                        "2", new MenuItem()
                        {
                            Title = "Play against Computer",
                            CommandToExecute = setMenu.Run
                        }
                    },
                    {    
                        "3", new MenuItem()
                        {
                            Title = "Computer vs Computer (for testing)",
                            CommandToExecute = preSet.Run
                        }
                    },
                    {    
                        "4", new MenuItem()
                        {
                            Title = "Open saved game",
                            CommandToExecute = openFile.Run
                        }
                    },


                }
            };


            var menu0 = new Menu(0)
            { 
                Title = "Connect4",
                MenuItemsDictionary = new Dictionary<string, MenuItem>()
                {
                    {
                        "S", new MenuItem()
                        {
                            Title = "Start game",
                            CommandToExecute = gameMenu.Run
                        }
                    },
                    {
                        "J", new MenuItem()
                        {
                            Title = "Set defaults for game (save to JSON)",
                            CommandToExecute = null
                        }
                    },

                }
            };

            menu0.Run();
        }

    }
}

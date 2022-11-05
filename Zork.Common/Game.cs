using System;
using System.Linq;

namespace Zork.Common
{
    public class Game
    {
        public World World { get; }

        public Player Player { get; }

        public IOutputService Output { get; private set; }

        public Game(World world, string startingLocation)
        {
            World = world;
            Player = new Player(World, startingLocation);
        }

        public void Run(IOutputService output)
        {
            Output = output;

            Room previousRoom = null;
            bool isRunning = true;
            while (isRunning)
            {
                Output.WriteLine(Player.CurrentRoom);
                if (previousRoom != Player.CurrentRoom)
                {
                    Output.WriteLine(Player.CurrentRoom.Description);
                    foreach(Item item in Player.CurrentRoom.Inventory) //if the roomInventory is empty it never enters this loop 
                        //for (int i=0; i < Player.CurrentRoom.Inventory.Count; i++)
                        {
                        Output.WriteLine(item.Description);
                        //Output.WriteLine(Player.CurrentRoom.Inventory[1]);
                    }
                    previousRoom = Player.CurrentRoom;
                }

                Output.Write("\n> ");

                string inputString = Console.ReadLine().Trim();
                // might look like:  "LOOK", "TAKE MAT", "QUIT"
                char  separator = ' ';
                string[] commandTokens = inputString.Split(separator);
                
                string verb = null;
                string subject = null;
                if (commandTokens.Length == 0)
                {
                    continue;
                }
                else if (commandTokens.Length == 1)
                {
                    verb = commandTokens[0];

                }
                else
                {
                    verb = commandTokens[0];
                    subject = commandTokens[1];
                }

                Commands command = ToCommand(verb);
                string outputString;
                switch (command)
                {
                    case Commands.Quit:
                        isRunning = false;
                        outputString = "Thank you for playing!";
                        break;

                    case Commands.Look:
                        outputString = Player.CurrentRoom.Description;
                        Output.WriteLine(outputString);
                        foreach (Item item in Player.CurrentRoom.Inventory)
                        {
                            Output.WriteLine(item.Description);
                            
                        }
                       
                        outputString = Player.CurrentRoom.Description;
                        break;

                    case Commands.North:
                    case Commands.South:
                    case Commands.East:
                    case Commands.West:
                        Directions direction = (Directions)command;
                        if (Player.Move(direction))
                        {
                            outputString = $"You moved {direction}.";
                        }
                        else
                        {
                            outputString = "The way is shut!";
                        }
                        break;

                    case Commands.Take:
                        Item itemToTake = null;  
 
                        if (subject == null)
                        {
                            Output.WriteLine("This command requires a subject");
                            break;
                        }

                        //counts all items in world and counts them as item to take
                        foreach (Item item in World.Items)
                        {
                            if (string.Compare(item.Name, subject, ignoreCase: true) == 0)
                            {
                                itemToTake = item;
                                break;
                            }
                        }
                        if (itemToTake == null)
                        {
                            Output.WriteLine("You can't see any such thing");
                            break;
                        }

                        foreach (Item item in Player.CurrentRoom.Inventory)
                        {
                            if (item == itemToTake)
                            {
                                Player.Inventory.Add(item);
                                Player.CurrentRoom.Inventory.Remove(item);
                                Output.WriteLine("Taken.");
                                break;
                            }

                        }
                        
                        outputString = null;
                        break;

                    case Commands.Drop:
                        Item itemToDrop = null;

                        if (subject == null)
                        {
                            Output.WriteLine("This command requires a subject");
                            break;
                        }

                        foreach (Item item in Player.Inventory)
                        {
                            if (string.Compare(item.Name, subject, ignoreCase: true) == 0)
                            {
                                itemToDrop = item;
                                break;
                            }
                        }


                        foreach (Item item in Player.Inventory)
                        {
                            if (item == itemToDrop)
                            {
                                Player.Inventory.Remove(item);
                                Player.CurrentRoom.Inventory.Add(item);
                                Output.WriteLine("Dropped.");
                                break;
                            }

                        }



                        outputString = null;
                        break;

                    case Commands.Inventory:
                        //Checks if player has a inventory count of more then 0 - if not the player is empty handed 
                        if (Player.Inventory.Count < 1)
                        {
                            Console.WriteLine("You are Empty Handed");
                        }
                        else
                        {
                            foreach(Item Item in Player.Inventory)
                            {
                                Console.WriteLine(Item.Description);
                            }
                                
                        }
                        outputString = null;
                        break;

                    default:
                        outputString = "Unknown command.";
                        break;
                }

                //Output.WriteLine(outputString);
            }
        }

        private static Commands ToCommand(string commandString) => Enum.TryParse(commandString, true, out Commands result) ? result : Commands.Unknown;
    }
}

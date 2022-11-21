namespace WFC_Sudoku_Solver
{
    public static class Draw
    {
        public static void Board(Board board)
        {

            for (int x = 0; x < 9; x++)
            {
                if (x % 3 == 0)
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine("-------------------------------");
                }
                for (int y = 0; y < 9; y++)
                {
                    if (y % 3 == 0)
                    {
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write("|");
                    }
                    var tile = board.GetTile(x, y);
                    var entropy = tile.GetEntropy();

                    if (entropy == 0)
                    {
                        if (tile.IsSolved())
                        {
                            Console.ForegroundColor = ConsoleColor.Gray;
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Magenta;
                        }
                    }
                    else if (entropy == 1)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                    }
                    else if (entropy < 3)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                    }
                    else if (entropy < 6)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                    }

                    Console.Write(" " + tile.ToString() + " ");
                }
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("|");
                Console.WriteLine();
            }
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("-------------------------------");
        }
    }
}
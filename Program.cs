using System.Diagnostics;
using WFC_Sudoku_Solver;

// see https://www.youtube.com/watch?v=rI_y2GAlQFM for the general theory on the WFC algo

var input = File.ReadAllText("input.txt");

// writing to console is fairly slow actually so if you want to measure performance set this to false:
var demoMode = false;

foreach (var puzzle in input.Split('-'))
{

    var sw = Stopwatch.StartNew();

    Rand.Init();
    var board = new Board(puzzle);

    if (!board.IsValid())
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("Invalid board.");
        Console.ReadKey();
    }

    var snapshot = board.Clone();

    var i = 0;
    while (!board.IsSolved())
    {
        i++;

        if (!board.CalculateEntropy())
        {
            // todo: this the greatest potential place for improvement
            // when we fail to calculate entropy it means that the board is in a 
            // state where it has cells that can't change to anything
            // we then revert to the original board snapshot and redo the whole process
            // and hope the collapse is more favorable, having some kind of intelligent
            // 'last known good' state would make this much more efficient
            board = snapshot.Clone();
            continue;
        }

        board.Collapse();

        if (demoMode)
        {
            Console.Clear();
            Console.WriteLine(i);
            Draw.Board(board);
            Thread.Sleep(100);
        }
    }

    sw.Stop();
    Console.Clear();

    Console.WriteLine();
    Draw.Board(snapshot);
    Console.WriteLine();
    Console.WriteLine("              vvv              ");
    Console.WriteLine();
    Draw.Board(board);

    if (!board.IsValid())
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("Board Invalid!");
    }
    else
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"Done! Board solved in {i} iterations in {sw.ElapsedMilliseconds}ms. Press any key to go to next.");
    }
    Console.ReadKey();
}

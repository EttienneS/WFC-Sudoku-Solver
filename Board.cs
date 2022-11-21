namespace WFC_Sudoku_Solver
{
    public class Board
    {
        private readonly List<Tile> _board;

        private List<List<Tile>> _columns;
        private List<List<Tile>> _rows;
        private List<List<Tile>> _squares;

        public Board(string input)
        {
            _board = new List<Tile>();

            var lines = input.Trim().Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            for (int x = 0; x < lines.Length; x++)
            {
                var line = lines[x].Trim();
                for (int y = 0; y < line.Length; y++)
                {
                    _board.Add(new Tile(x, y, int.Parse(line[y].ToString())));
                }
            }

            CalculateRowsAndColumns();
        }

        private Board(List<Tile> board)
        {
            // only used to clone a new board
            _board = new List<Tile>();
            foreach (var tile in board)
            {
                _board.Add(new Tile(tile.X, tile.Y, tile.Value));
            }

            CalculateRowsAndColumns();
        }

        public bool CalculateEntropy()
        {
            foreach (var tile in _board)
            {
                var open = Constants.AllValues.ToList();

                var closedRow = GetClosed(GetRow(tile));
                var closedColumn = GetClosed(GetColumn(tile));
                var closedSquare = GetClosed(GetSquare(tile));

                foreach (var value in closedRow.Concat(closedColumn).Concat(closedSquare))
                {
                    open.Remove(value);
                }

                tile.SetOpen(open.ToArray());

                if (!tile.IsSolved() && tile.GetEntropy() == 0)
                {
                    // messages are just here to help debugging when unsolvable state is hit
                    // Console.WriteLine($"Unsolvable state: {tile.X}:{tile.Y}");
                    // Console.WriteLine("Row " + string.Join("", GetRow(tile)));
                    // Console.WriteLine("Column " + string.Join("", GetColumn(tile)));
                    // Console.WriteLine("Square " + string.Join("", GetSquare(tile)));
                    return false;
                }
            }

            return true;
        }

        public Board Clone()
        {
            return new Board(_board);
        }

        public void Collapse()
        {
            var entropyGroupings = _board.Where(t => !t.IsSolved())
                                         .GroupBy(t => t.GetEntropy())
                                         .OrderBy(t => t.Key);

            var lowestEntropy = entropyGroupings.First().ToList();

            var tile = lowestEntropy[Rand.Get(0, lowestEntropy.Count)];

            tile.Solve(tile.GetRandomPossibleValue());
        }

        public bool IsValid()
        {
            return ValidateSet(_rows) && ValidateSet(_columns) && ValidateSet(_squares);
        }

        private static bool ValidateSet(List<List<Tile>> sets)
        {
            foreach (var set in sets)
            {
                var hash = new HashSet<int>();
                foreach (var tile in set)
                {
                    if (tile.Value == 0) continue;

                    if (!hash.Add(tile.Value))
                    {
                        Console.WriteLine($"Invalid value: {tile.X}: {tile.Y} = {tile.Value}");
                        return false;
                    }
                }
            }
            return true;
        }

        public Tile GetTile(int x, int y)
        {
            // todo: this can be sped up by using a lookup with the X/Y coord as the key
            return _board.First(t => t.X == x && t.Y == y);
        }

        public bool IsSolved()
        {
            return _board.All(t => t.IsSolved());
        }

        private void CalculateRowsAndColumns()
        {
            _columns = _board.GroupBy(g => g.Y).Select(r => r.ToList()).ToList();
            _rows = _board.GroupBy(g => g.X).Select(r => r.ToList()).ToList();
            _squares = new List<List<Tile>>();

            for (var i = 0; i < 3; i++)
            {
                for (var j = 0; j < 3; j++)
                {
                    var square = new List<Tile>();
                    for (var x = 0; x < 3; x++)
                    {
                        for (var y = 0; y < 3; y++)
                        {
                            var tile = GetTile(x + (i * 3), y + (j * 3));
                            square.Add(tile);
                        }
                    }

                    _squares.Add(square);
                }
            }
        }

        private IEnumerable<int> GetClosed(IEnumerable<Tile> tiles)
        {
            return tiles.Where(s => s.IsSolved()).Select(s => s.Value);
        }

        private List<Tile> GetColumn(Tile tile)
        {
            return _columns.First(c => c.Contains(tile));
        }

        private List<Tile> GetRow(Tile tile)
        {
            return _rows.First(r => r.Contains(tile));
        }

        private List<Tile> GetSquare(Tile tile)
        {
            return _squares.First(s => s.Contains(tile));
        }
    }
}
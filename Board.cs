namespace WFC_Sudoku_Solver
{
    public class Board
    {
        private readonly List<Tile> _board;

        private List<List<Tile>> _columns;
        private List<List<Tile>> _rows;
        private List<List<Tile>> _squares;

        private Dictionary<Tile, (List<Tile> row, List<Tile> column, List<Tile> square)> _tileLookup;

        public Board(string input)
        {
            _board = new List<Tile>();

            var lines = input.Trim().Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).ToList();

            Name = lines[0];
            lines.RemoveAt(0);

            for (int x = 0; x < lines.Count; x++)
            {
                // change . to 0 to allow support for importing from https://qqwing.com/generate.html easily
                var line = lines[x].Replace('.', '0').Trim();
                for (int y = 0; y < line.Length; y++)
                {
                    _board.Add(new Tile(x, y, int.Parse(line[y].ToString())));
                }
            }

            CalculateRowsAndColumns();
        }

        private Board(List<Tile> board, string name)
        {
            // only used to clone a new board
            Name = name;
            _board = new List<Tile>();
            foreach (var tile in board)
            {
                _board.Add(new Tile(tile.X, tile.Y, tile.Value));
            }

            CalculateRowsAndColumns();
        }

        public string Name { get; }

        public bool CalculateEntropy()
        {
            foreach (var tile in _board)
            {
                var open = Constants.AllValues.ToList();

                var lookup = _tileLookup[tile];
                var closedRow = GetClosed(lookup.row);
                var closedColumn = GetClosed(lookup.column);
                var closedSquare = GetClosed(lookup.square);

                foreach (var value in closedRow.Concat(closedColumn).Concat(closedSquare))
                {
                    open.Remove(value);
                }

                tile.SetOpen(open.ToArray());

                if (!tile.IsSolved() && tile.GetEntropy() == 0)
                {
                    return false;
                }
            }

            return true;
        }

        public Board Clone()
        {
            return new Board(_board, Name);
        }

        public void Collapse()
        {
            var entropyGroupings = _board.Where(t => !t.IsSolved())
                                         .GroupBy(t => t.GetEntropy())
                                         .OrderBy(t => t.Key);

            var lowestEntropy = entropyGroupings.First().ToList();

            if (lowestEntropy[0].GetEntropy() == 1)
            {
                // solve all tiles that only have one possible value
                // greatly speeds up process and helps fail quicker when
                // we are on the wrong track (this needs a validation check to happen)
                // right after the collapse step or we could go down an incorrect tangent
                foreach (var tile in lowestEntropy)
                {
                    tile.Solve(tile.GetRandomPossibleValue());
                }
            }
            else
            {
                var tile = lowestEntropy[Rand.Get(0, lowestEntropy.Count)];
                tile.Solve(tile.GetRandomPossibleValue());
            }
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
                        return false;
                    }
                }
            }
            return true;
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

            _tileLookup = new Dictionary<Tile, (List<Tile> row, List<Tile> column, List<Tile> square)>();
            foreach (var tile in _board)
            {
                var column = _columns.First(c => c.Contains(tile));
                var row = _rows.First(r => r.Contains(tile));
                var square = _squares.First(s => s.Contains(tile));

                _tileLookup.Add(tile, (row, column, square));
            }
        }

        private IEnumerable<int> GetClosed(IEnumerable<Tile> tiles)
        {
            return tiles.Where(s => s.IsSolved()).Select(s => s.Value);
        }
    }
}
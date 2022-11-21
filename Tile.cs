namespace WFC_Sudoku_Solver
{
    public class Tile
    {
        private int[] _open;

        public Tile(int x, int y, int value)
        {
            X = x;
            Y = y;

            Solve(value);
        }

        public int Value { get; internal set; }
        public int X { get; }
        public int Y { get; }

        public int GetEntropy() => _open.Length;

        public bool IsSolved() => Value > 0;

        public void SetOpen(int[] open)
        {
            _open = open;
        }

        public override string ToString()
        {
            return IsSolved() ? Value.ToString() : "X";
        }

        internal int GetRandomPossibleValue()
        {
            return _open[Rand.Get(0, _open.Length)];
        }

        internal void Solve(int value)
        {
            Value = value;

            if (Value <= 0)
            {
                SetOpen(Constants.AllValues);
            }
            else
            {
                SetOpen(Array.Empty<int>());
            }
        }
    }
}
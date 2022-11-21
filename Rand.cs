namespace WFC_Sudoku_Solver
{
    public static class Rand
    {
        // use singleton Rand object to allow us to use same seed for all random in a run
        private static Random _random = new Random();

        public static void Init(int seed = -1)
        {
            if (seed > 0)
            {
                _random = new Random(seed);
            }
            else
            {
                _random = new Random();
            }
        }

        public static int Get(int min, int max)
        {
            return _random.Next(min, max);
        }
    }
}
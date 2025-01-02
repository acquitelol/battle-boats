public enum GridKind : int {
    Player = 0,
    Computer = 1,
    Tracker = 2,
}

public class Grid {
    private char[,] grid = new char[Config.GRID_SIZE, Config.GRID_SIZE].Fill('-');
    public GridKind kind { get; }

    public Grid(GridKind kind) {
        this.kind = kind;
    }

    public void Print() {
        if (grid.GetLength(1).ToString().Length > 1) {
            Console.Write("  ");
            for (int col = 1; col <= grid.GetLength(1); ++col) {
                var digit = col / 10;
                Console.Write((digit > 0 ? digit.ToString() : " ") + " ");
            }
            Console.WriteLine();
        }

        Console.Write("  ");
        for (int col = 1; col <= grid.GetLength(1); ++col) {
            var digit = col % 10;
            Console.Write(digit + " ");
        }
        Console.WriteLine();
        bool isComputer = kind == GridKind.Computer;

        for (int i = 0; i < grid.GetLength(0); ++i) {
            Console.Write($"{(char)('A' + i)} ");

            for (int j = 0; j < grid.GetLength(1); ++j) {
                if (grid[i, j] == 'H')
                    Console.ForegroundColor = isComputer ? ConsoleColor.Green : ConsoleColor.Red;
                else if (grid[i, j] == 'M')
                    Console.ForegroundColor = isComputer ? ConsoleColor.Red : ConsoleColor.Green;
                else if (grid[i, j] == 'B')
                    Console.ForegroundColor = ConsoleColor.Blue;
                else
                    Console.ResetColor();

                Console.Write($"{grid[i, j]} ");
                Console.ResetColor();
            }

            Console.WriteLine();
        }
    }

    public char this[int row, int col] {
        get => grid[row, col];
        set => grid[row, col] = value;
    }

    public void Clear() => grid.Fill('-');
    public bool CheckWinner() => grid.Cast<char>().Count(x => x == 'H') == Config.BOAT_COUNT;
}

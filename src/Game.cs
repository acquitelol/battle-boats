namespace BattleBoats;

class Game {
    private static GameState state = new();
    private static SaveManager manager = new();
    private static Random random = new();

    static void Main() {
        for (;;) {
            switch (DisplayMenu()) {
                case "1": PlayGame(false); break;
                case "2": ResumeGame(); break;
                case "3": return;
                default: Console.WriteLine("Invalid choice"); break;
            }
        }
    }

    private static string DisplayMenu() {
        Console.WriteLine("\nBattle Boats");
        Console.WriteLine("1. Play new game");
        Console.WriteLine("2. Resume game");
        Console.WriteLine("3. Quit game");
        Console.Write("-> ");
        return Console.ReadLine() ?? "<>";
    }

    private static void PlayGame(bool resuming) {
        if (!resuming) {
            state.Clear();
            PlacePlayerBoats();
            PlaceComputerBoats();
        }

        for (;;) {
            Console.WriteLine("\nYour target tracker:");
            state.targetTracker.Print();
            Console.WriteLine("\nYour fleet:");
            state.playerFleet.Print();

            PlayerTurn();
            if (state.targetTracker.CheckWinner()) {
                Console.WriteLine("\nCongratulations! You won!");
                break;
            }

            ComputerTurn();
            if (state.playerFleet.CheckWinner()) {
                Console.WriteLine("\nGame Over! Computer wins!");
                state.computerFleet.Print();
                break;
            }

            manager.Save(state);
        }
    }

    private static void ResumeGame() {
        if (!File.Exists(SaveManager.Name)) {
            Console.WriteLine("No existing game state found, starting a new game.");
            PlayGame(false);
        }

        state = manager.Load();
        PlayGame(true);
    }

    private static void PlacePlayerBoats() {
        Console.WriteLine("\nPlace your fleet:");

        for (int i = 0; i < Config.BOAT_COUNT; ++i) {
            state.playerFleet.Print();

            for (;;) {
                Console.Write($"\nEnter coordinates for boat {i + 1} (e.g. A1) -> ");

                if (!ValidateCoordinates(Console.ReadLine() ?? "<>", out int row, out int col)) {
                    Console.WriteLine($"Invalid coordinates. Use letter A-{(char)('A' + Config.GRID_SIZE - 1)} followed by number 1-8");
                    continue;
                }

                if (state.playerFleet[row, col] == 'B') {
                    Console.WriteLine("Location already occupied");
                    continue;
                }

                state.playerFleet[row, col] = 'B';
                break;
            }
        }
    }

    private static void PlaceComputerBoats() {
        int placed = 0;

        while (placed < Config.BOAT_COUNT) {
            int row = random.Next(Config.GRID_SIZE);
            int col = random.Next(Config.GRID_SIZE);

            if (state.computerFleet[row, col] == '-') {
                state.computerFleet[row, col] = 'B';
                ++placed;
            }
        }
    }

    private static void PlayerTurn() {
        for (;;) {
            Console.Write("\nEnter target coordinates (e.g. A1) -> ");

            if (!ValidateCoordinates(Console.ReadLine() ?? "<>", out int row, out int col)) {
                Console.WriteLine($"Invalid coordinates. Use letter A-{(char)('A' + Config.GRID_SIZE - 1)} followed by number 1-{Config.GRID_SIZE}");
                continue;
            }

            if (state.targetTracker[row, col] != '-') {
                Console.WriteLine("Location already targeted");
                continue;
            }

            if (state.computerFleet[row, col] == 'B') {
                state.targetTracker[row, col] = 'H';
                Console.WriteLine("You hit the computer's boat!");
            } else {
                state.targetTracker[row, col] = 'M';
                Console.WriteLine("You missed!");
            }

            break;
        }
    }

    private static void ComputerTurn() {
        for (;;) {
            int row = random.Next(Config.GRID_SIZE);
            int col = random.Next(Config.GRID_SIZE);

            if (state.playerFleet[row, col] is '-' or 'B') {
                string coord = $"{(char)('A' + row)}{col + 1}";
                Console.WriteLine($"\nComputer fires at {coord}");

                if (state.playerFleet[row, col] == 'B') {
                    state.playerFleet[row, col] = 'H';
                    Console.WriteLine("Computer hit your boat!");
                } else {
                    state.playerFleet[row, col] = 'M';
                    Console.WriteLine("Computer missed!");
                }

                break;
            }
        }
    }

    private static bool ValidateCoordinates(string coord, out int row, out int col) {
        row = col = -1;

        if (string.IsNullOrEmpty(coord) || coord.Length != 2)
            return false;

        char rowChar = char.ToUpper(coord[0]);
        if (!char.IsLetter(rowChar) || rowChar < 'A' || rowChar > ('A' + Config.GRID_SIZE - 1))
            return false;

        if (!int.TryParse(coord[1].ToString(), out int colNum) || colNum < 1 || colNum > 8)
            return false;

        row = rowChar - 'A';
        col = colNum - 1;
        return true;
    }
}

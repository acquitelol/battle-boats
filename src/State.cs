using System.Reflection;

class GameState {
    public Grid playerFleet = new(GridKind.Player);
    public Grid computerFleet = new(GridKind.Computer);
    public Grid targetTracker = new(GridKind.Tracker);

    public void Clear() {
        playerFleet.Clear();
        computerFleet.Clear();
        targetTracker.Clear();
    }
}

class SaveManager {
    public const string Name = "state.bin";

    public void Save(GameState state) {
        using (BinaryWriter writer = new(File.Open(Name, FileMode.Create))) {
            WriteObject(writer, state);
        }
    }

    public GameState Load() {
        using (BinaryReader reader = new(File.Open(Name, FileMode.Open))) {
            return (GameState)ReadObject(reader, typeof(GameState));
        }
    }

    private void WriteObject(BinaryWriter writer, object obj) {
        Type type = obj.GetType();

        foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.Instance)) {
            object value = field.GetValue(obj)!;
            if (value is Grid grid) WriteGrid(writer, grid);
        }
    }

    private object ReadObject(BinaryReader reader, Type type) {
        object obj = Activator.CreateInstance(type)!;

        foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.Instance)) {
            field.SetValue(obj, ReadGrid(reader));
        }

        return obj;
    }

    private void WriteGrid(BinaryWriter writer, Grid grid) {
        writer.Write((Byte)grid.kind);

        for (int row = 0; row < Config.GRID_SIZE; ++row)
            for (int col = 0; col < Config.GRID_SIZE; ++col)
                writer.Write(grid[row, col]);
    }

    private Grid ReadGrid(BinaryReader reader) {
        Grid grid = new((GridKind)reader.ReadByte());

        for (int row = 0; row < Config.GRID_SIZE; ++row)
            for (int col = 0; col < Config.GRID_SIZE; ++col)
                grid[row, col] = reader.ReadChar();

        return grid;
    }
}

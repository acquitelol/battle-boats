public static class Config {
    public static int GRID_SIZE = 8;
    public static int BOAT_COUNT = 5;
}

public static class ArrayExtensions {
    public static char[,] Fill(this char[,] array, char filler) {
        for (int i = 0; i < array.GetLength(0); ++i)
            for (int j = 0; j < array.GetLength(1); ++j)
                array[i, j] = filler;
        return array;
    }
}

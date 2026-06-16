public static class GameData
{
    public static int TotalThrown = 0;
    public static int CorrectCount = 0;
    public static int WrongCount = 0;
    public static int MaxCombo = 0;

    public static void Reset()
    {
        TotalThrown = 0;
        CorrectCount = 0;
        WrongCount = 0;
        MaxCombo = 0;
    }
}
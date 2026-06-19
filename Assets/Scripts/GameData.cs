using System.Collections.Generic;

public static class GameData
{
    public static int TotalThrown = 0;
    public static int CorrectCount = 0;
    public static int WrongCount = 0;

    // 現在持っているごみのデータ（実行時の状態管理用）
    public static TrashData CurrentTrash;
    public static bool IsHoldingTrash = false;

    public static void Reset()
    {
        TotalThrown = 0;
        CorrectCount = 0;
        WrongCount = 0;
        IsHoldingTrash = false;
        CurrentTrash = default;
    }
}
using UnityEngine;
using UnityEngine.UI;

public class ResultManager : MonoBehaviour
{
    [System.Serializable]
    public struct RankCondition
    {
        public string rankName; // "S", "A", "B", "C"
        public int minTotal;
        public int minCorrect;
        public int maxWrong;
    }

    [Header("UI Elements")]
    public Text totalText;
    public Text correctText;
    public Text wrongText;
    public Text rankText;

    [Header("Rank Settings (S, A, B, Cの順で4つ要素を作る)")]
    public RankCondition[] rankConditions;

    void Start()
    {
        // スコア表示
        totalText.text = GameData.TotalThrown.ToString();
        correctText.text = GameData.CorrectCount.ToString();
        wrongText.text = GameData.WrongCount.ToString();

        EvaluateRank();
    }

    void EvaluateRank()
    {
        string finalRank = "C"; // 条件を満たさなかった場合のデフォルト

        // インスペクタで設定した条件を上から順にチェック
        foreach (var condition in rankConditions)
        {
            if (GameData.TotalThrown >= condition.minTotal &&
                GameData.CorrectCount >= condition.minCorrect &&
                GameData.WrongCount <= condition.maxWrong)
            {
                finalRank = condition.rankName;
                break; // 満たした時点で決定（Sから順に設定しておくこと）
            }
        }

        rankText.text = finalRank;
    }
}
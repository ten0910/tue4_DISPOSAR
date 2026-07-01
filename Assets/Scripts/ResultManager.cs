using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ResultManager : MonoBehaviour
{
    private const string TitleSceneName = "TitleScene";

    [Header("Result UI")]
    [SerializeField] private TextMeshProUGUI correctCountValueText;
    [SerializeField] private TextMeshProUGUI totalCountValueText;
    [SerializeField] private GameObject perfectResultGroup;
    [SerializeField] private TextMeshProUGUI elapsedTimeValueText;
    [SerializeField] private Button returnButton;

    private void Start()
    {
        ShowResult();

        if (returnButton != null)
        {
            returnButton.onClick.AddListener(ReturnToTitle);
        }
    }

    private void OnDestroy()
    {
        if (returnButton != null)
        {
            returnButton.onClick.RemoveListener(ReturnToTitle);
        }
    }

    private void ShowResult()
    {
        if (correctCountValueText != null)
        {
            correctCountValueText.text = $"{GameData.CorrectCount}個";
        }

        if (totalCountValueText != null)
        {
            totalCountValueText.text = $"{GameData.TotalThrown}個";
        }

        bool isPerfect = GameData.TotalThrown > 0 &&
                         GameData.CorrectCount == GameData.TotalThrown;

        if (perfectResultGroup != null)
        {
            perfectResultGroup.SetActive(isPerfect);
        }

        if (isPerfect && elapsedTimeValueText != null)
        {
            elapsedTimeValueText.text = FormatTime(GameData.ElapsedTime);
        }
    }

    public void ReturnToTitle()
    {
        FadeManager.Instance.TransitionToScene(TitleSceneName, 1f, Color.black);
    }

    private static string FormatTime(float seconds)
    {
        int totalSeconds = Mathf.Max(0, Mathf.RoundToInt(seconds));
        return $"{totalSeconds / 60:00}:{totalSeconds % 60:00}";
    }
}

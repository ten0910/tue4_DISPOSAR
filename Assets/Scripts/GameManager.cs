using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro; // TextMeshProを使用

public class GameManager : MonoBehaviour
{
    public enum GameState { Tutorial, Countdown, Playing, TimeUp }
    public GameState currentState = GameState.Tutorial;

    [Header("UI Elements")]
    public GameObject tutorialUI;
    public TextMeshProUGUI countdownText;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI comboText;
    public GameObject timeUpUI;
    public GameObject startPopSprite; // 0.6秒ポップするスプライト

    [Header("Settings")]
    public float gameDuration = 300f; // 5分
    private float timeRemaining;
    private int currentCombo = 0;

    private TrashSpawner spawner;

    void Start()
    {
        GameData.Reset();
        spawner = FindFirstObjectByType<TrashSpawner>();

        tutorialUI.SetActive(true);
        if (countdownText != null) countdownText.gameObject.SetActive(false);
        timeUpUI.SetActive(false);
        if (startPopSprite) startPopSprite.SetActive(false);
        UpdateComboUI();
    }

    void Update()
    {
        switch (currentState)
        {
            case GameState.Tutorial:
                if (Input.GetKeyDown(KeyCode.Return)) // Enterキー
                {
                    tutorialUI.SetActive(false);
                    StartCoroutine(StartCountdown());
                }
                break;

            case GameState.Playing:
                timeRemaining -= Time.deltaTime;
                if (timeRemaining <= 0)
                {
                    timeRemaining = 0;
                    StartCoroutine(EndGame());
                }
                UpdateTimerUI();
                break;
        }
    }

    IEnumerator StartCountdown()
    {
        currentState = GameState.Countdown;
        if (countdownText != null) countdownText.gameObject.SetActive(true);

        // 10秒カウントダウン
        for (int i = 10; i > 0; i--)
        {
            if (countdownText != null) countdownText.text = i.ToString();
            yield return new WaitForSeconds(1f);
        }

        if (countdownText != null) countdownText.text = "START!";
        
        // スタート時のポップ演出 (0.6秒)
        if (startPopSprite) startPopSprite.SetActive(true);
        yield return new WaitForSeconds(0.6f);
        if (startPopSprite) startPopSprite.SetActive(false);

        yield return new WaitForSeconds(0.4f); // 合計1秒程度待機
        if (countdownText != null) countdownText.gameObject.SetActive(false);

        timeRemaining = gameDuration;
        currentState = GameState.Playing;
    }

    public void OnSortResult(bool isCorrect)
    {
        GameData.TotalThrown++;
        if (isCorrect)
        {
            GameData.CorrectCount++;
            currentCombo++;
            if (currentCombo > GameData.MaxCombo)
            {
                GameData.MaxCombo = currentCombo;
            }
        }
        else
        {
            if (currentCombo > GameData.MaxCombo) GameData.MaxCombo = currentCombo;
            currentCombo = 0;
        }
        UpdateComboUI();
    }

    void UpdateTimerUI()
    {
        if (timerText == null) return;
        int minutes = Mathf.FloorToInt(timeRemaining / 60f);
        int seconds = Mathf.FloorToInt(timeRemaining % 60f);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    void UpdateComboUI()
    {
        if (comboText == null) return;
        comboText.text = currentCombo > 0 ? $"{currentCombo} Combo!" : "";
    }


    IEnumerator EndGame()
    {
        currentState = GameState.TimeUp;
        if (currentCombo > GameData.MaxCombo) GameData.MaxCombo = currentCombo;

        timeUpUI.SetActive(true);
        yield return new WaitForSeconds(2f);
        timeUpUI.SetActive(false);

        FadeManager.Instance.TransitionToScene("ResultScene", 1.0f, Color.black);
    }
}
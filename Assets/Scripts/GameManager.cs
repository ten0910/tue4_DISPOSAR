using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public enum GameState { Tutorial, Countdown, Playing, TimeUp }
    public GameState currentState = GameState.Tutorial;

    [Header("UI Elements")]
    public GameObject tutorialUI;
    public Text countdownText;
    public Text timerText;
    public Text comboText;
    public GameObject timeUpUI;

    [Header("Settings")]
    public float gameDuration = 180f; // 3分
    private float timeRemaining;
    private int currentCombo = 0;

    private TrashSpawner spawner;

    void Start()
    {
        GameData.Reset();
        spawner = FindFirstObjectByType<TrashSpawner>();


        tutorialUI.SetActive(true);
        countdownText.gameObject.SetActive(false);
        timeUpUI.SetActive(false);
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
        countdownText.gameObject.SetActive(true);

        for (int i = 3; i > 0; i--)
        {
            countdownText.text = i.ToString();
            yield return new WaitForSeconds(1f);
        }

        countdownText.text = "START!";
        spawner.SpawnNextTrash(); // 最初のごみを生成
        yield return new WaitForSeconds(1f);
        countdownText.gameObject.SetActive(false);

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
        int minutes = Mathf.FloorToInt(timeRemaining / 60f);
        int seconds = Mathf.FloorToInt(timeRemaining % 60f);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    void UpdateComboUI()
    {
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
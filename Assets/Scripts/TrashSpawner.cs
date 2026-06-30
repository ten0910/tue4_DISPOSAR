using UnityEngine;
using TMPro; // TextMeshProを使用
using UnityEngine.InputSystem;
using System.Collections; // コルーチンを使用するために追加

public class TrashSpawner : MonoBehaviour
{
    public GameObject trashPrefab;    // 全てのごみで共通のプレハブ
    public Transform spawnPoint;      // 手元の位置
    public TrashDatabase database;    // インスペクタからアセットを割り当てる
    public TextMeshProUGUI trashNameText; // 持っているごみの名前表示用
    public GameObject explanationUI;      // 解説UIの親
    public TextMeshProUGUI explanationText; // 解説文表示用

    [Header("ゴミ箱の演出・UI設定")]
    public ParticleSystem correctEffect;
    public ParticleSystem wrongEffect;
    public GameObject correctUI;
    public GameObject wrongUI;

    private GameObject currentTrashObject;
    private GameManager gameManager;
    private Coroutine uiHideCoroutine; // 実行中のコルーチンを管理する変数

    void Start()
    {
        gameManager = Object.FindFirstObjectByType<GameManager>();
        if (explanationUI) explanationUI.SetActive(false);
        if (trashNameText) trashNameText.text = "";

        // 初期状態では演出用UIを非表示にしておく
        if (correctUI) correctUI.SetActive(false);
        if (wrongUI) wrongUI.SetActive(false);
    }

    void Update()
    {
        if (gameManager == null || gameManager.currentState != GameManager.GameState.Playing) return;

        // RFID読み取りのプロトタイプ（キーボード操作）
        // 1: バナナの皮(101), 2: ペットボトル(102), 3: お菓子の袋(103)
        if (Keyboard.current.digit1Key.wasPressedThisFrame) OnRFIDRead("101");
        if (Keyboard.current.digit2Key.wasPressedThisFrame) OnRFIDRead("102");
        if (Keyboard.current.digit3Key.wasPressedThisFrame) OnRFIDRead("103");
    }

    public void OnRFIDRead(string id)
    {
        if (database == null)
        {
            Debug.LogError("TrashDatabase asset is not assigned to TrashSpawner!");
            return;
        }

        // 次の読み取り時にエフェクトや解説を非表示にする
        if (explanationUI) explanationUI.SetActive(false);

        // 新しく読み込んだ際、前回のUIタイマーが動いていたら強制停止して非表示にする
        if (uiHideCoroutine != null)
        {
            StopCoroutine(uiHideCoroutine);
            if (correctUI) correctUI.SetActive(false);
            if (wrongUI) wrongUI.SetActive(false);
        }

        TrashData data = database.GetById(id);
        if (string.IsNullOrEmpty(data.id))
        {
            Debug.LogWarning($"Trash with ID {id} not found in database.");
            return;
        }

        if (currentTrashObject != null) Destroy(currentTrashObject);

        GameData.CurrentTrash = data;
        GameData.IsHoldingTrash = true;

        if (trashNameText) trashNameText.text = "所持中: " + data.name;

        SpawnTrash(data);
    }

    void SpawnTrash(TrashData data)
    {
        if (trashPrefab == null)
        {
            Debug.LogError("TrashPrefab is not assigned to TrashSpawner!");
            return;
        }

        currentTrashObject = Instantiate(trashPrefab, spawnPoint.position, Quaternion.identity);

        Rigidbody rb = currentTrashObject.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        TrashProjectile projectile = currentTrashObject.GetComponent<TrashProjectile>();
        if (projectile != null)
        {
            projectile.data = data;
        }
    }

    public void ShowExplanation(bool isCorrect)
    {
        if (explanationUI && explanationText)
        {
            explanationUI.SetActive(true);
            string result = isCorrect ? "<color=blue>正解！</color>\n" : "<color=red>不正解...</color>\n";

            // --- ここから追加・統合した処理 ---
            if (isCorrect)
            {
                if (correctEffect) correctEffect.Play();
                if (correctUI) correctUI.SetActive(true);
            }
            else
            {
                if (wrongEffect) wrongEffect.Play();
                if (wrongUI) wrongUI.SetActive(true);
            }

            // すでにタイマーが動いていた場合は一度止める（連続で呼ばれた際のバグ防止）
            if (uiHideCoroutine != null) StopCoroutine(uiHideCoroutine);
            // 1秒後にUIを消すコルーチンを開始
            uiHideCoroutine = StartCoroutine(HideUIDelayed(1.0f));
            // --- ここまで ---

            explanationText.text = result + GameData.CurrentTrash.description;
        }

        if (trashNameText) trashNameText.text = "";
        GameData.IsHoldingTrash = false;
        currentTrashObject = null;
    }

    // UIを遅延して非表示にするコルーチン
    private IEnumerator HideUIDelayed(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (correctUI) correctUI.SetActive(false);
        if (wrongUI) wrongUI.SetActive(false);

        uiHideCoroutine = null;
    }
}
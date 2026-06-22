using UnityEngine;
using TMPro; // TextMeshProを使用
using UnityEngine.InputSystem;

public class TrashSpawner : MonoBehaviour
{
    public GameObject trashPrefab;    // 全てのごみで共通のプレハブ
    public Transform spawnPoint;      // 手元の位置
    public TrashDatabase database;    // インスペクタからアセットを割り当てる
    public TextMeshProUGUI trashNameText; // 持っているごみの名前表示用
    public GameObject explanationUI;      // 解説UIの親
    public TextMeshProUGUI explanationText; // 解説文表示用

    private GameObject currentTrashObject;
    private GameManager gameManager;

    void Start()
    {
        gameManager = Object.FindFirstObjectByType<GameManager>();
        if (explanationUI) explanationUI.SetActive(false);
        if (trashNameText) trashNameText.text = "";
    }

    void Update()
    {
        if (gameManager == null || gameManager.currentState != GameManager.GameState.Playing) return;

        // RFID読み取りのプロトタイプ（キーボード操作）
        // 1: バナナの皮(101), 2: ペットボトル(102), 3: お菓子の袋(103)
        if (Keyboard.current.digit1Key.wasPressedThisFrame) OnRFIDRead("101");
        if (Keyboard.current.digit2Key.wasPressedThisFrame) OnRFIDRead("102");
        if (Keyboard.current.digit3Key.wasPressedThisFrame) OnRFIDRead("103");

        /* Arduino/RFIDセンサーがある場合の処理 (コメントアウト)
        string rfidId = ArduinoManager.Instance.ReadRFID(); 
        if (!string.IsNullOrEmpty(rfidId)) OnRFIDRead(rfidId);
        */
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

        TrashData data = database.GetById(id);
        if (string.IsNullOrEmpty(data.id))
        {
            Debug.LogWarning($"Trash with ID {id} not found in database.");
            return;
        }

        // すでに持っている場合は生成しない（または古いものを消す）
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

        TrashProjectile projectile = currentTrashObject.GetComponent<TrashProjectile>();
        if (projectile != null)
        {
            // intではなくDataそのものを渡すように修正
            projectile.data = data;
        }
    }

    public void ShowExplanation(bool isCorrect)
    {
        if (explanationUI && explanationText)
        {
            explanationUI.SetActive(true);
            string result = isCorrect ? "<color=blue>正解！</color>\n" : "<color=red>不正解...</color>\n";
            explanationText.text = result + GameData.CurrentTrash.description;
        }
        
        // 投げ終わったので所持表示をクリア
        if (trashNameText) trashNameText.text = "";
        GameData.IsHoldingTrash = false;
        currentTrashObject = null;
    }
}
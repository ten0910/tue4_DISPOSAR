using UnityEngine;
using System.Collections;

public class TrashProjectile : MonoBehaviour
{
    [HideInInspector] public TrashData data; // データベースから渡される情報
    public float flyDuration = 0.8f; 
    public float arcHeight = 2.0f;    

    private bool isFlying = false;
    private GameManager gameManager;
    private TrashSpawner trashSpawner;

    void Start()
    {
        gameManager = Object.FindFirstObjectByType<GameManager>();
        trashSpawner = Object.FindFirstObjectByType<TrashSpawner>();
    }

    void Update()
    {
        // 飛行中、またはゲーム中でない場合は入力を無視
        if (isFlying || gameManager == null || gameManager.currentState != GameManager.GameState.Playing) return;

        // --- プロトタイプ用：キーボード操作でごみ箱を指定 ---
        if (Input.GetKeyDown(KeyCode.Q)) TryFlyToBin(0); // 可燃
        if (Input.GetKeyDown(KeyCode.W)) TryFlyToBin(1); // プラ
        if (Input.GetKeyDown(KeyCode.E)) TryFlyToBin(2); // 缶・ペットボトル

        /* Arduino/圧力センサーがある場合の処理 (将来用)
        int binIndex = ArduinoManager.Instance.GetPressureSensorIndex();
        if (binIndex != -1) TryFlyToBin(binIndex);
        */

        // マウス操作（デバッグ用）
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                TrashBin bin = hit.collider.GetComponent<TrashBin>();
                if (bin != null) StartCoroutine(FlyToBin(bin));
            }
        }
    }

    void TryFlyToBin(int binType)
    {
        // シーン内のごみ箱から、指定されたタイプのものを見つけて飛ばす
        TrashBin[] bins = Object.FindObjectsByType<TrashBin>(FindObjectsSortMode.None);
        foreach (var bin in bins)
        {
            if (bin.targetBinType == binType)
            {
                StartCoroutine(FlyToBin(bin));
                break;
            }
        }
    }

    IEnumerator FlyToBin(TrashBin bin)
    {
        isFlying = true;
        Vector3 startPos = transform.position;
        Vector3 endPos = bin.transform.position;

        // 判定（DatabaseのtargetBinTypeと、入れた箱のタイプを比較）
        bool isCorrect = (data.targetBinType == bin.targetBinType);
        
        if (gameManager != null)
        {
            gameManager.OnSortResult(isCorrect);
        }

        // 放物線移動
        float elapsed = 0f;
        while (elapsed < flyDuration)
        {
            elapsed += Time.deltaTime;
            float percent = elapsed / flyDuration;
            Vector3 currentPos = Vector3.Lerp(startPos, endPos, percent);
            currentPos.y += Mathf.Sin(percent * Mathf.PI) * arcHeight;
            transform.position = currentPos;
            yield return null;
        }

        // 到着時の演出
        bin.PlayEffect(isCorrect);

        // 解説を表示（Spawner側のUI制御を呼ぶ）
        if (trashSpawner != null)
        {
            trashSpawner.ShowExplanation(isCorrect);
        }

        Destroy(gameObject);
    }
}
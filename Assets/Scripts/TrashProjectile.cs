using UnityEngine;
using System.Collections;

public class TrashProjectile : MonoBehaviour
{
    public int trashType; // 0:可燃, 1:プラ, 2:缶・ペットボトル
    public float flyDuration = 0.8f; // 飛ぶ時間
    public float arcHeight = 2.0f;    // 放物線の高さ

    private bool isFlying = false;

    // 軽量化：参照を記憶する変数
    private GameManager gameManager;
    private TrashSpawner trashSpawner;

    void Start()
    {
        // ごみが生成された瞬間に、シーン内から1回だけ検索して記憶しておく（新形式 API）
        gameManager = Object.FindFirstObjectByType<GameManager>();
        trashSpawner = Object.FindFirstObjectByType<TrashSpawner>();
    }

    void Update()
    {
        if (isFlying) return;

        // レイキャストでクリックしたごみ箱を検知
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                TrashBin bin = hit.collider.GetComponent<TrashBin>();
                if (bin != null)
                {
                    StartCoroutine(FlyToBin(bin));
                }
            }
        }
    }

    IEnumerator FlyToBin(TrashBin bin)
    {
        isFlying = true;
        Vector3 startPos = transform.position;
        Vector3 endPos = bin.transform.position;

        // 飛んだ時点で正誤判定を行う（あらかじめ記憶した変数を使用）
        bool isCorrect = (this.trashType == bin.targetBinType);
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

            // XZ軸の直線補間
            Vector3 currentPos = Vector3.Lerp(startPos, endPos, percent);
            // Y軸に数式（放物線）を上乗せ
            currentPos.y += Mathf.Sin(percent * Mathf.PI) * arcHeight;

            transform.position = currentPos;
            yield return null;
        }

        // ごみ箱に到達した瞬間の演出
        bin.PlayEffect(isCorrect);

        // 次のごみをスポーンさせて自身を消去（あらかじめ記憶した変数を使用）
        if (trashSpawner != null)
        {
            trashSpawner.SpawnNextTrash();
        }

        Destroy(gameObject);
    }
}
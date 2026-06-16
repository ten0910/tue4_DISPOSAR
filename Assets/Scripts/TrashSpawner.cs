using UnityEngine;

public class TrashSpawner : MonoBehaviour
{
    public GameObject[] trashPrefabs; // 0:可燃, 1:プラ, 2:缶・ペットボトル
    public Transform spawnPoint;      // 手元の位置

    private GameObject currentTrash;

    // 軽量化：GameManagerへの参照を記憶する変数
    private GameManager gameManager;

    void Start()
    {
        // ゲーム開始時に1回だけ検索して記憶（キャッシュ）しておく
        gameManager = Object.FindFirstObjectByType<GameManager>();
    }

    public void SpawnNextTrash()
    {
        // 記憶した変数を使うことで、毎回検索する負荷をなくす（軽量化）
        if (gameManager != null && gameManager.currentState != GameManager.GameState.Playing) return;

        int randomIndex = Random.Range(0, trashPrefabs.Length);
        currentTrash = Instantiate(trashPrefabs[randomIndex], spawnPoint.position, Quaternion.identity);

        // ごみ自身に自分の種類をインデックスとして覚えさせる
        TrashProjectile projectile = currentTrash.GetComponent<TrashProjectile>();
        if (projectile != null)
        {
            projectile.trashType = randomIndex;
        }
    }
}
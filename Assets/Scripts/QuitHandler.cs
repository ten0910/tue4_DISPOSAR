using UnityEngine;

public class QuitHandler : MonoBehaviour
{
    /// <summary>
    /// ゲームを終了します。
    /// ボタンのOnClick()イベントなどに登録して使用してください。
    /// </summary>
    public void QuitGame()
    {
        Debug.Log("Game is quitting...");

        // 実機ビルドでの終了
        Application.Quit();

        // Unityエディタ上での動作確認用（エディタの再生を停止）
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
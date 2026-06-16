using UnityEngine;

public class FadeTrigger : MonoBehaviour
{
    [Header("Transition Settings")]
    public string targetSceneName;
    public float fadeDuration = 1.0f;
    public Color fadeColor = Color.black;

    /// <summary>
    /// ボタンのOnClickなどで呼び出すためのメソッド
    /// </summary>
    public void TriggerTransition()
    {
        FadeManager.Instance.TransitionToScene(targetSceneName, fadeDuration, fadeColor);
    }

    /// <summary>
    /// プログラムから直接呼び出す場合（引数指定可能）
    /// </summary>
    public void TriggerTransition(string sceneName)
    {
        FadeManager.Instance.TransitionToScene(sceneName, fadeDuration, fadeColor);
    }
}
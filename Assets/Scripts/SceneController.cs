using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public float defaultFadeDuration = 1.0f;
    public Color defaultFadeColor = Color.black;

    public void LoadScene(string sceneName)
    {
        FadeManager.Instance.TransitionToScene(sceneName, defaultFadeDuration, defaultFadeColor);
    }
}
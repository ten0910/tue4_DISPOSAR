using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class FadeManager : MonoBehaviour
{
    private static FadeManager instance;
    public static FadeManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject obj = new GameObject("FadeManager");
                instance = obj.AddComponent<FadeManager>();
                DontDestroyOnLoad(obj);
            }
            return instance;
        }
    }

    private Canvas fadeCanvas;
    private Image fadeImage;
    private bool isFading = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            SetupCanvas();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void SetupCanvas()
    {
        // Canvasの作成
        GameObject canvasObj = new GameObject("FadeCanvas");
        canvasObj.transform.SetParent(transform);
        fadeCanvas = canvasObj.AddComponent<Canvas>();
        fadeCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        fadeCanvas.sortingOrder = 999; // 最前面に表示

        canvasObj.AddComponent<CanvasScaler>();
        canvasObj.AddComponent<GraphicRaycaster>();

        // Imageの作成
        GameObject imageObj = new GameObject("FadeImage");
        imageObj.transform.SetParent(canvasObj.transform);
        fadeImage = imageObj.AddComponent<Image>();
        
        // 全画面に広げる
        RectTransform rect = fadeImage.rectTransform;
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        // 初期状態は透明
        fadeImage.color = new Color(0, 0, 0, 0);
        fadeImage.raycastTarget = false;
    }

    public void TransitionToScene(string sceneName, float duration, Color fadeColor)
    {
        if (isFading) return;
        StartCoroutine(FadeRoutine(sceneName, duration, fadeColor));
    }

    private IEnumerator FadeRoutine(string sceneName, float duration, Color fadeColor)
    {
        isFading = true;
        fadeImage.raycastTarget = true; // フェード中は入力をブロック

        // フェードアウト
        float timer = 0;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Clamp01(timer / duration);
            fadeImage.color = new Color(fadeColor.r, fadeColor.g, fadeColor.b, alpha);
            yield return null;
        }
        fadeImage.color = new Color(fadeColor.r, fadeColor.g, fadeColor.b, 1);

        // シーン遷移
        yield return SceneManager.LoadSceneAsync(sceneName);

        // フェードイン
        timer = 0;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Clamp01(1 - (timer / duration));
            fadeImage.color = new Color(fadeColor.r, fadeColor.g, fadeColor.b, alpha);
            yield return null;
        }
        fadeImage.color = new Color(fadeColor.r, fadeColor.g, fadeColor.b, 0);

        fadeImage.raycastTarget = false;
        isFading = false;
    }
}
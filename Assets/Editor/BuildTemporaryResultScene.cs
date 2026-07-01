using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public static class BuildTemporaryResultScene
{
    private const string ScenePath = "Assets/Scenes/ResultScene_Temp.unity";
    private const string FontAssetPath =
        "Assets/TextMesh Pro/Fonts/MochiyPopOne-OTF-ExtraBold SDF.asset";

    [MenuItem("Tools/Build Temporary Result Scene")]
    public static void Build()
    {
        var scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        TMP_FontAsset fontAsset = GetOrCreateJapaneseFont();

        foreach (ResultManager oldManager in Object.FindObjectsByType<ResultManager>(FindObjectsSortMode.None))
        {
            Object.DestroyImmediate(oldManager);
        }

        GameObject oldResultUi = GameObject.Find("Result UI");
        if (oldResultUi != null)
        {
            Object.DestroyImmediate(oldResultUi);
        }

        GameObject canvasObject = new GameObject(
            "Result UI",
            typeof(RectTransform),
            typeof(Canvas),
            typeof(CanvasScaler),
            typeof(GraphicRaycaster),
            typeof(ResultManager));

        Canvas canvas = canvasObject.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;

        CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.matchWidthOrHeight = 0.5f;

        RectTransform panel = CreateImage(
            "Result Panel",
            canvasObject.transform,
            new Vector2(1180f, 780f),
            Vector2.zero,
            new Color(0.04f, 0.12f, 0.1f, 0.96f));

        CreateText("Title Text", panel, "リザルト", 64f, new Vector2(0f, 290f),
            new Vector2(1040f, 90f), fontAsset, Color.white, TextAlignmentOptions.Center);

        TextMeshProUGUI correctValueText = CreateResultRow(
            "Correct Count Row", panel, "正しく分別できたごみ", "0個", 38f,
            new Vector2(0f, 130f), fontAsset);

        TextMeshProUGUI totalValueText = CreateResultRow(
            "Total Count Row", panel, "捨てたごみの合計", "0個", 38f,
            new Vector2(0f, 35f), fontAsset);

        GameObject perfectGroup = new GameObject("Perfect Result", typeof(RectTransform));
        perfectGroup.transform.SetParent(panel, false);
        RectTransform perfectRect = perfectGroup.GetComponent<RectTransform>();
        perfectRect.anchoredPosition = new Vector2(0f, -100f);
        perfectRect.sizeDelta = new Vector2(1000f, 150f);

        CreateText("Perfect Message Text", perfectRect, "すべて正しく分別できました！", 30f,
            new Vector2(0f, 45f), new Vector2(1000f, 50f), fontAsset,
            new Color(0.35f, 0.95f, 0.55f, 1f), TextAlignmentOptions.Left);

        TextMeshProUGUI timeValueText = CreateResultRow(
            "Elapsed Time Row", perfectRect, "かかった時間", "00:00", 32f,
            new Vector2(0f, -25f), fontAsset);

        Button returnButton = CreateButton(panel, fontAsset);

        ResultManager manager = canvasObject.GetComponent<ResultManager>();
        SerializedObject serializedManager = new SerializedObject(manager);
        serializedManager.FindProperty("correctCountValueText").objectReferenceValue = correctValueText;
        serializedManager.FindProperty("totalCountValueText").objectReferenceValue = totalValueText;
        serializedManager.FindProperty("perfectResultGroup").objectReferenceValue = perfectGroup;
        serializedManager.FindProperty("elapsedTimeValueText").objectReferenceValue = timeValueText;
        serializedManager.FindProperty("returnButton").objectReferenceValue = returnButton;
        serializedManager.ApplyModifiedPropertiesWithoutUndo();

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene, ScenePath);
        AssetDatabase.SaveAssets();
        Debug.Log("ResultScene_Temp UI was built successfully.");
    }

    private static TMP_FontAsset GetOrCreateJapaneseFont()
    {
        TMP_FontAsset fontAsset = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(FontAssetPath);
        if (fontAsset == null)
        {
            throw new System.IO.FileNotFoundException(
                "Japanese TMP font asset was not found.",
                FontAssetPath);
        }

        return fontAsset;
    }

    private static RectTransform CreateImage(
        string name, Transform parent, Vector2 size, Vector2 position, Color color)
    {
        GameObject gameObject = new GameObject(name, typeof(RectTransform), typeof(Image));
        gameObject.transform.SetParent(parent, false);
        RectTransform rect = gameObject.GetComponent<RectTransform>();
        rect.sizeDelta = size;
        rect.anchoredPosition = position;
        gameObject.GetComponent<Image>().color = color;
        return rect;
    }

    private static TextMeshProUGUI CreateText(
        string name,
        Transform parent,
        string value,
        float fontSize,
        Vector2 position,
        Vector2 size,
        TMP_FontAsset font,
        Color color,
        TextAlignmentOptions alignment)
    {
        GameObject gameObject = new GameObject(name, typeof(RectTransform), typeof(TextMeshProUGUI));
        gameObject.transform.SetParent(parent, false);
        RectTransform rect = gameObject.GetComponent<RectTransform>();
        rect.sizeDelta = size;
        rect.anchoredPosition = position;

        TextMeshProUGUI text = gameObject.GetComponent<TextMeshProUGUI>();
        text.text = value;
        text.font = font;
        text.fontSize = fontSize;
        text.color = color;
        text.alignment = alignment;
        text.enableWordWrapping = false;
        return text;
    }

    private static TextMeshProUGUI CreateResultRow(
        string name,
        Transform parent,
        string label,
        string value,
        float fontSize,
        Vector2 position,
        TMP_FontAsset font)
    {
        GameObject rowObject = new GameObject(name, typeof(RectTransform));
        rowObject.transform.SetParent(parent, false);
        RectTransform row = rowObject.GetComponent<RectTransform>();
        row.sizeDelta = new Vector2(1000f, 70f);
        row.anchoredPosition = position;

        CreateText("Label", row, label, fontSize, new Vector2(-300f, 0f),
            new Vector2(400f, 70f), font, Color.white, TextAlignmentOptions.Left);

        CreateText("Dot Leader", row, "・・・・・・・・・・・・", fontSize,
            Vector2.zero, new Vector2(360f, 70f), font,
            new Color(1f, 1f, 1f, 0.75f), TextAlignmentOptions.Center);

        return CreateText("Value", row, value, fontSize, new Vector2(390f, 0f),
            new Vector2(220f, 70f), font, Color.white, TextAlignmentOptions.Right);
    }

    private static Button CreateButton(Transform parent, TMP_FontAsset font)
    {
        RectTransform buttonRect = CreateImage(
            "Return To Title Button",
            parent,
            new Vector2(380f, 92f),
            new Vector2(0f, -285f),
            new Color(0.18f, 0.72f, 0.4f, 1f));

        Button button = buttonRect.gameObject.AddComponent<Button>();
        button.targetGraphic = buttonRect.GetComponent<Image>();

        CreateText("Button Label", buttonRect, "タイトルへ戻る", 32f, Vector2.zero,
            buttonRect.sizeDelta, font, Color.white, TextAlignmentOptions.Center);
        return button;
    }
}

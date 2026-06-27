using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EcoButtonSlide : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler,
    IPointerDownHandler,
    IPointerUpHandler
{
    [Header("Button Parts")]
    [SerializeField] private RectTransform greenPart;
    [SerializeField] private RectTransform whitePart;
    [SerializeField] private Image whiteImage;
    [SerializeField] private Image greenImage;

    [Header("Green Slide Position")]
    [SerializeField] private Vector2 normalPosition = new Vector2(60f, 0f);
    [SerializeField] private Vector2 hoverPosition = new Vector2(80f, 0f);
    [SerializeField] private float slideSpeed = 12f;

    [Header("White Scale")]
    [SerializeField] private Vector3 normalScale = Vector3.one;
    [SerializeField] private Vector3 hoverScale = new Vector3(1.04f, 1.04f, 1f);
    [SerializeField] private Vector3 pressedScale = new Vector3(0.98f, 0.98f, 1f);
    [SerializeField] private float scaleSpeed = 12f;

    [Header("Press Color")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color pressedColor = new Color(0.65f, 0.65f, 0.65f, 1f);

    private Vector2 targetPosition;
    private Vector3 targetScale;
    private bool isHovering;

    private void Awake()
    {
        targetPosition = normalPosition;
        targetScale = normalScale;

        if (greenPart != null)
        {
            greenPart.anchoredPosition = normalPosition;
        }

        if (whitePart != null)
        {
            whitePart.localScale = normalScale;
        }

        SetColor(normalColor);
    }

    private void Update()
    {
        if (greenPart != null)
        {
            greenPart.anchoredPosition = Vector2.Lerp(
                greenPart.anchoredPosition,
                targetPosition,
                Time.deltaTime * slideSpeed
            );
        }

        if (whitePart != null)
        {
            whitePart.localScale = Vector3.Lerp(
                whitePart.localScale,
                targetScale,
                Time.deltaTime * scaleSpeed
            );
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;
        targetPosition = hoverPosition;
        targetScale = hoverScale;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
        targetPosition = normalPosition;
        targetScale = normalScale;
        SetColor(normalColor);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        targetScale = pressedScale;
        SetColor(pressedColor);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        SetColor(normalColor);

        if (isHovering)
        {
            targetPosition = hoverPosition;
            targetScale = hoverScale;
        }
        else
        {
            targetPosition = normalPosition;
            targetScale = normalScale;
        }
    }

    private void SetColor(Color color)
    {
        if (whiteImage != null)
        {
            whiteImage.color = color;
        }

        if (greenImage != null)
        {
            greenImage.color = color;
        }
    }
}
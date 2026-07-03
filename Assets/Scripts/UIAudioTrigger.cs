using UnityEngine;
using UnityEngine.EventSystems;

public class UIAudioTrigger : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    [Header("Prefer using ID mapped in AudioManager. If empty, directClip will be used.")]
    public string hoverId;
    public string clickId;
    public AudioClip hoverClip;
    public AudioClip clickClip;
    public float volume = 1f;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!string.IsNullOrEmpty(hoverId)) AudioManager.Instance?.PlaySE(hoverId, volume);
        else AudioManager.Instance?.PlaySEClip(hoverClip, volume);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!string.IsNullOrEmpty(clickId)) AudioManager.Instance?.PlaySE(clickId, volume);
        else AudioManager.Instance?.PlaySEClip(clickClip, volume);
    }
}

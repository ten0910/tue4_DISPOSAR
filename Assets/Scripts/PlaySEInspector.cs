using UnityEngine;

public class PlaySEInspector : MonoBehaviour
{
    [Header("Either assign id (use AudioManager map) or direct clip")]
    public string seId;
    public AudioClip clip;
    public float volume = 1f;

    // Call from other scripts or from UnityEvent in Inspector
    public void Play()
    {
        if (!string.IsNullOrEmpty(seId)) AudioManager.Instance?.PlaySE(seId, volume);
        else AudioManager.Instance?.PlaySEClip(clip, volume);
    }
}

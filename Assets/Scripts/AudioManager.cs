using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Mixer")]
    public AudioMixer mixer; // expose "BGMVolume","SEVolume"
    public AudioMixerGroup bgmGroup;
    public AudioMixerGroup seGroup;

    [Header("BGM per Scene (assign in Inspector)")]
    public List<SceneBGM> bgmMap = new List<SceneBGM>();

    [Header("SE Map (assign ids and clips in Inspector)")]
    public List<SEEntry> seMap = new List<SEEntry>();
    public int sePoolSize = 16;

    AudioSource bgmSource;
    List<AudioSource> sePool;
    Dictionary<string, AudioClip> seDict = new Dictionary<string, AudioClip>();
    Dictionary<string, AudioClip> bgmDict = new Dictionary<string, AudioClip>();
    int poolIndex = 0;

    [System.Serializable] public class SceneBGM { public string sceneName; public AudioClip clip; }
    [System.Serializable] public class SEEntry { public string id; public AudioClip clip; }

    void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else { Destroy(gameObject); return; }

        bgmSource = gameObject.AddComponent<AudioSource>();
        bgmSource.outputAudioMixerGroup = bgmGroup;
        bgmSource.loop = true;
        bgmSource.playOnAwake = false;
        bgmSource.spatialBlend = 0f;

        sePool = new List<AudioSource>(sePoolSize);
        for (int i = 0; i < sePoolSize; i++)
        {
            var s = gameObject.AddComponent<AudioSource>();
            s.outputAudioMixerGroup = seGroup;
            s.playOnAwake = false;
            s.spatialBlend = 0f;
            sePool.Add(s);
        }

        RebuildMaps();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy() => SceneManager.sceneLoaded -= OnSceneLoaded;

    void RebuildMaps()
    {
        seDict.Clear();
        foreach (var e in seMap) if (e != null && !string.IsNullOrEmpty(e.id) && e.clip != null) seDict[e.id] = e.clip;
        bgmDict.Clear();
        foreach (var b in bgmMap) if (b != null && !string.IsNullOrEmpty(b.sceneName) && b.clip != null) bgmDict[b.sceneName] = b.clip;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode) => PlayBGMForScene(scene.name, 1f);

    public void PlayBGMForScene(string sceneName, float fadeTime = 1f)
    {
        RebuildMaps();
        if (!bgmDict.TryGetValue(sceneName, out var clip) || clip == null) { StopBGM(fadeTime); return; }
        StartCoroutine(CrossfadeToClip(clip, fadeTime));
    }

    IEnumerator CrossfadeToClip(AudioClip newClip, float duration)
    {
        float half = Mathf.Max(0.05f, duration / 2f);
        yield return StartCoroutine(FadeMixer("BGMVolume", 0f, -80f, half));
        bgmSource.clip = newClip;
        bgmSource.PlayScheduled(AudioSettings.dspTime + 0.05);
        yield return StartCoroutine(FadeMixer("BGMVolume", -80f, 0f, half));
    }

    public void StopBGM(float fadeTime = 0.5f) => StartCoroutine(FadeMixer("BGMVolume", 0f, -80f, fadeTime));

    IEnumerator FadeMixer(string param, float fromDb, float toDb, float time)
    {
        float t = 0f;
        while (t < time) { t += Time.unscaledDeltaTime; mixer.SetFloat(param, Mathf.Lerp(fromDb, toDb, t / time)); yield return null; }
        mixer.SetFloat(param, toDb);
    }

    AudioSource GetNextSESource()
    {
        for (int i = 0; i < sePool.Count; i++)
        {
            int idx = (poolIndex + i) % sePool.Count;
            if (!sePool[idx].isPlaying) { poolIndex = (idx + 1) % sePool.Count; return sePool[idx]; }
        }
        poolIndex = (poolIndex + 1) % sePool.Count;
        sePool[poolIndex].Stop();
        return sePool[poolIndex];
    }

    public void PlaySE(string id, float volume = 1f)
    {
        RebuildMaps();
        if (!seDict.TryGetValue(id, out var clip) || clip == null) return;
        PlaySEClip(clip, volume);
    }

    public void PlaySEClip(AudioClip clip, float volume = 1f)
    {
        if (clip == null) return;
        var s = GetNextSESource();
        s.PlayOneShot(clip, volume);
    }

    public void PlaySEScheduled(string id, double dspTime, float volume = 1f)
    {
        RebuildMaps();
        if (!seDict.TryGetValue(id, out var clip) || clip == null) return;
        var s = GetNextSESource();
        s.clip = clip;
        s.volume = volume;
        s.PlayScheduled(dspTime);
    }

    // Optional: expose simple setters for UI sliders (dB)
    public void SetBGMDb(float db) => mixer.SetFloat("BGMVolume", db);
    public void SetSEDb(float db) => mixer.SetFloat("SEVolume", db);
}

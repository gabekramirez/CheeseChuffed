using UnityEngine;

public class AudioBootstrap : MonoBehaviour
{
    [Header("Library to use for this scene")]
    public AudioLibrary library;

    [Header("Start ambient automatically?")]
    public bool autoStartAmbient = true;

    [Header("Start music automatically? (-1 = none, uses cycle)")]
    public int startMusicIndex = 0;

    private void Start()
    {
        if (library == null) { Debug.LogWarning("[AudioBootstrap] No AudioLibrary assigned."); return; }

        var am = AudioManager.Instance;
        if (am == null) { Debug.LogError("[AudioBootstrap] AudioManager not found."); return; }

        am.ambientTracks = library.ambientTracks;
        am.musicTracks   = library.musicTracks;   // wire the playlist

        if (autoStartAmbient && library.ambientTracks is { Length: > 0 })
            am.StartAmbient();

        if (startMusicIndex >= 0)
        {
            // Use the cycle so it auto-advances; skip to the desired index first.
            for (int i = 0; i < startMusicIndex; i++) am.SkipMusic(); // fast-forward index
            am.StartMusic();
        }
    }

    // ── Ambient controls (wire to UI buttons) ─────────────────────────────────
    public void OnPauseAmbient() => AudioManager.Instance.PauseAmbient();
    public void OnResumeAmbient() => AudioManager.Instance.ResumeAmbient();
    public void OnSkipAmbient() => AudioManager.Instance.SkipAmbient();

    // ── Music controls (wire to UI buttons) ───────────────────────────────────
    public void OnPauseMusic() => AudioManager.Instance.PauseMusic();
    public void OnResumeMusic() => AudioManager.Instance.ResumeMusic();
    public void OnSkipMusic() => AudioManager.Instance.SkipMusic();

    // ── SFX helpers ───────────────────────────────────────────────────────────
    public void OnClickButton()
    {
        AudioClip clip = library.GetSFX("click");
        AudioManager.Instance.PlayClick(clip);
    }

    public void PlayWorldSFX(string sfxName, Vector3 position)
    {
        AudioClip clip = library.GetSFX(sfxName);
        AudioManager.Instance.PlaySFX(clip, position);
    }
}
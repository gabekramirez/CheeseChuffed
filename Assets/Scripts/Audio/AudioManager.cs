using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Central audio manager. Attach to a persistent GameObject in your first scene.
/// Handles ambient tracks (sequential looping), music tracks (sequential looping),
/// a single persistent looping song (position-preserving across scenes), SFX,
/// and a single looping SFX slot for continuous sounds you start/stop on demand.
/// Supports pause/resume/skip on ambient and music.
/// </summary>
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Volume Controls (0-1)")]
    [Range(0f, 1f)] public float masterVolume  = 1f;
    [Range(0f, 1f)] public float ambientVolume = 0.6f;
    [Range(0f, 1f)] public float musicVolume   = 0.8f;
    [Range(0f, 1f)] public float sfxVolume     = 1f;

    [Header("Ambient Settings")]
    [Tooltip("Tracks play in order, then loop back to the first.")]
    public AudioClip[] ambientTracks;
    [Tooltip("Seconds to cross-fade between ambient tracks.")]
    public float ambientCrossfade = 2f;

    [Header("Music Settings")]
    [Tooltip("Tracks play in order, then loop back to the first.")]
    public AudioClip[] musicTracks;
    [Tooltip("Cross-fade time when switching music tracks.")]
    public float musicCrossfade = 1.5f;

    [Header("Loop Track (single persistent song)")]
    [Tooltip("A single song that loops continuously and remembers its playback position across scene switches. Independent of the ambient/music playlist system above.")]
    public AudioClip loopTrack;
    [Tooltip("If true, the loop track starts playing automatically in Awake.")]
    public bool autoPlayLoopTrack = true;
    [Tooltip("Fade time (seconds) used when SetLoopTrack swaps to a different clip.")]
    public float loopTrackSwapFade = 1f;

    // -- Sources ---------------------------------------------------------------
    private AudioSource _ambientA, _ambientB;
    private AudioSource _musicA,   _musicB;
    private AudioSource _sfxSource;
    private AudioSource _loopSource;
    private AudioSource _loopingSFXSource;

    // -- Ambient state ---------------------------------------------------------
    private bool      _ambientActive;
    private int       _ambientIndex = -1;
    private bool      _ambientPaused;
    private Coroutine _ambientCycleRoutine;
    private Coroutine _ambientFadeRoutine;
    private Coroutine _ambientWaitRoutine;   // handle to the WaitForSeconds leg
    private float     _ambientTimeRemaining; // seconds left on the current track
    private float     _ambientWaitStart;     // Time.time when the wait began

    // -- Music state -----------------------------------------------------------
    private bool      _musicActive;
    private int       _musicIndex = -1;
    private bool      _musicPaused;
    private Coroutine _musicCycleRoutine;
    private Coroutine _musicFadeRoutine;
    private Coroutine _musicWaitRoutine;
    private float     _musicTimeRemaining;
    private float     _musicWaitStart;

    // -- Loop track state --------------------------------------------------------
    // Because this AudioManager is DontDestroyOnLoad and _loopSource.loop = true,
    // the clip keeps playing straight through scene loads on its own — no manual
    // save/restore is needed for normal scene transitions. _savedLoopTime and
    // _loopWasPlaying exist for cases where you explicitly Pause/Stop it yourself
    // (e.g. during a cutscene or menu) and want to resume exactly where it left off.
    private float _savedLoopTime;
    private bool  _loopWasPlaying;
    private Coroutine _loopSwapFadeRoutine;

    [Header("Set UI")]
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider ambientVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider effectsVolumeSlider;

    // -- Lifecycle -------------------------------------------------------------

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        BuildSources();

        LoadVolumeSettings();

        InitSlider(masterVolumeSlider,   "masterVolumeSlider",   masterVolume, SetMasterVolume);
        InitSlider(ambientVolumeSlider,  "ambientVolumeSlider",  ambientVolume, SetAmbientVolume);
        InitSlider(musicVolumeSlider,    "musicVolumeSlider",    musicVolume, SetMusicVolume);
        InitSlider(effectsVolumeSlider,  "effectsVolumeSlider",  sfxVolume, SetSFXVolume);

        if (autoPlayLoopTrack && loopTrack != null)
            PlayLoopTrack();
    }

    private void BuildSources()
    {
        _ambientA         = AddSource("Ambient_A",   loop: false);
        _ambientB         = AddSource("Ambient_B",   loop: false);
        _musicA           = AddSource("Music_A",     loop: false); // loop disabled — cycle handles it
        _musicB           = AddSource("Music_B",     loop: false);
        _sfxSource        = AddSource("SFX",         loop: false);
        _loopSource       = AddSource("LoopTrack",   loop: true);  // this one actually loops itself
        _loopingSFXSource = AddSource("LoopingSFX",  loop: true);  // single on/off looping SFX slot
    }

    private AudioSource AddSource(string label, bool loop)
    {
        var go = new GameObject(label);
        go.transform.SetParent(transform);
        var src = go.AddComponent<AudioSource>();
        src.loop        = loop;
        src.playOnAwake = false;
        src.volume      = 0f;
        return src;
    }

    /// <summary>
    /// Safely wires up a volume slider: sets its starting value and hooks its
    /// onValueChanged callback, without throwing if the slider was never
    /// assigned in the Inspector.
    /// </summary>
    private void InitSlider(Slider slider, string fieldName, float initialValue, UnityEngine.Events.UnityAction onChanged)
    {
        if (slider == null)
        {
            Debug.LogWarning($"AudioManager: {fieldName} is not assigned — skipping its setup. " +
                              "Volume changes for that channel will still work via script, just not through this slider.");
            return;
        }

        // Remove listeners before setting the value to prevent a callback feedback loop
        slider.onValueChanged.RemoveAllListeners();
        slider.value = initialValue;
        slider.onValueChanged.AddListener(_ => onChanged());
    }

    // =========================================================================
    // Public API — Loop Track (single persistent song)
    // =========================================================================

    /// <summary>
    /// Starts (or resumes) the single persistent loop track from wherever it
    /// currently sits. Since the AudioSource itself persists across scene loads,
    /// calling this again after a scene switch does nothing destructive —
    /// it just keeps playing. Safe to call once in Awake.
    /// </summary>
    public void PlayLoopTrack()
    {
        if (loopTrack == null) return;

        if (_loopSource.clip != loopTrack)
        {
            _loopSource.clip = loopTrack;
            _loopSource.time = LoadSavedLoopPosition(loopTrack);
        }

        _loopSource.volume = musicVolume * masterVolume;
        if (!_loopSource.isPlaying) _loopSource.Play();

        // Checkpoint position periodically so a scene using SimpleLoopMusic
        // (or an abrupt quit) can pick up exactly where this left off.
        CancelInvoke(nameof(SaveLoopPosition));
        InvokeRepeating(nameof(SaveLoopPosition), 2f, 2f);
    }

    private void SaveLoopPosition()
    {
        if (loopTrack == null || !_loopSource.isPlaying) return;
        PlayerPrefs.SetString("LoopSong_Clip", loopTrack.name);
        PlayerPrefs.SetFloat("LoopSong_Position", _loopSource.time);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Returns the saved playback position for this clip, or 0 if the last
    /// saved position was for a different clip (i.e. nothing to resume).
    /// </summary>
    private static float LoadSavedLoopPosition(AudioClip clip)
    {
        if (clip == null) return 0f;
        string savedClip = PlayerPrefs.GetString("LoopSong_Clip", "");
        if (savedClip != clip.name) return 0f;
        return PlayerPrefs.GetFloat("LoopSong_Position", 0f);
    }

    private void OnDestroy()
    {
        if (Instance == this) SaveLoopPosition();
    }

    private void OnApplicationQuit()
    {
        SaveLoopPosition();
    }

    /// <summary>
    /// Pauses the loop track and remembers its position so ResumeLoopTrack()
    /// picks back up exactly there. Use this for menus/cutscenes where you
    /// want the song to stop rather than keep playing underneath.
    /// </summary>
    public void PauseLoopTrack()
    {
        if (!_loopSource.isPlaying) return;
        _savedLoopTime  = _loopSource.time;
        _loopWasPlaying = true;
        _loopSource.Pause();
    }

    /// <summary>Resumes the loop track from the position saved by PauseLoopTrack().</summary>
    public void ResumeLoopTrack()
    {
        if (!_loopWasPlaying) return;
        _loopWasPlaying = false;
        _loopSource.time = _savedLoopTime;
        _loopSource.UnPause();
    }

    /// <summary>
    /// Stops the loop track entirely and resets its saved position to the start.
    /// Use PauseLoopTrack/ResumeLoopTrack instead if you want it to pick back up
    /// where it left off.
    /// </summary>
    public void StopLoopTrack()
    {
        _loopSource.Stop();
        _savedLoopTime  = 0f;
        _loopWasPlaying = false;
    }

    /// <summary>
    /// Swaps to a different song for the loop slot. If restart is false and the
    /// new clip is the same length/type of content you can resume mid-track;
    /// most of the time you'll want restart = true for a genuinely different song.
    /// </summary>
    public void SetLoopTrack(AudioClip newClip, bool restart = true, bool crossfade = true)
    {
        if (newClip == null || newClip == loopTrack) return;
        loopTrack = newClip;

        if (!crossfade)
        {
            _loopSource.clip = newClip;
            _loopSource.time = 0f;
            _loopSource.volume = musicVolume * masterVolume;
            _loopSource.Play();
            return;
        }

        if (_loopSwapFadeRoutine != null) StopCoroutine(_loopSwapFadeRoutine);
        _loopSwapFadeRoutine = StartCoroutine(SwapLoopTrackRoutine(newClip, restart));
    }

    private IEnumerator SwapLoopTrackRoutine(AudioClip newClip, bool restart)
    {
        float target = musicVolume * masterVolume;
        float t = 0f;
        float startVol = _loopSource.volume;

        // fade out
        while (t < loopTrackSwapFade)
        {
            t += Time.deltaTime;
            _loopSource.volume = Mathf.Lerp(startVol, 0f, t / loopTrackSwapFade);
            yield return null;
        }

        _loopSource.Stop();
        _loopSource.clip = newClip;
        if (restart) _loopSource.time = 0f;
        _loopSource.Play();

        // fade in
        t = 0f;
        while (t < loopTrackSwapFade)
        {
            t += Time.deltaTime;
            _loopSource.volume = Mathf.Lerp(0f, target, t / loopTrackSwapFade);
            yield return null;
        }
        _loopSource.volume = target;
    }

    /// <summary>Current playback position of the loop track, in seconds.</summary>
    public float LoopTrackPosition => _loopSource.time;

    /// <summary>True if the loop track is currently playing.</summary>
    public bool IsLoopTrackPlaying => _loopSource.isPlaying;

    // =========================================================================
    // Public API — Ambient
    // =========================================================================

    /// <summary>Start cycling through ambient tracks from the beginning.</summary>
    public void StartAmbient()
    {
        StopAllAmbientCoroutines();
        _ambientIndex  = -1;
        _ambientPaused = false;
        _ambientCycleRoutine = StartCoroutine(AmbientCycle());
    }

    /// <summary>Stop ambient playback with an optional fade-out.</summary>
    public void StopAmbient(float fadeTime = 1f)
    {
        StopAllAmbientCoroutines();
        _ambientPaused = false;
        FadeSource(ActiveAmbient,   0f, fadeTime);
        FadeSource(InactiveAmbient, 0f, fadeTime);
    }

    /// <summary>Pause ambient playback, preserving position in the current track.</summary>
    public void PauseAmbient()
    {
        if (_ambientPaused) return;
        _ambientPaused = true;

        // Record how much time is left so we can resume the wait accurately
        if (_ambientWaitRoutine != null)
        {
            _ambientTimeRemaining = Mathf.Max(0f, _ambientTimeRemaining - (Time.time - _ambientWaitStart));
            StopCoroutine(_ambientWaitRoutine);
            _ambientWaitRoutine = null;
        }

        ActiveAmbient.Pause();
        InactiveAmbient.Pause();
    }

    /// <summary>Resume ambient playback from where it was paused.</summary>
    public void ResumeAmbient()
    {
        if (!_ambientPaused) return;
        _ambientPaused = false;

        ActiveAmbient.UnPause();
        InactiveAmbient.UnPause();

        // Re-launch the wait leg with remaining time
        _ambientWaitRoutine = StartCoroutine(AmbientWaitThenAdvance(_ambientTimeRemaining));
    }

    /// <summary>Skip the current ambient track and immediately crossfade to the next.</summary>
    public void SkipAmbient()
    {
        StopAllAmbientCoroutines();
        _ambientPaused = false;
        _ambientCycleRoutine = StartCoroutine(AmbientCycle());
    }

    // =========================================================================
    // Public API — Music
    // =========================================================================

    /// <summary>
    /// Start cycling through <see cref="musicTracks"/> from the beginning.
    /// Call this instead of PlayMusic when you want auto-advance.
    /// </summary>
    public void StartMusic()
    {
        StopAllMusicCoroutines();
        _musicIndex  = -1;
        _musicPaused = false;
        ShuffleTracks(musicTracks);
        _musicCycleRoutine = StartCoroutine(MusicCycle());
    }

    private void ShuffleTracks(AudioClip[] tracks)
    {
        for (int i = tracks.Length - 1; i > 0; i--)
        {
            int j = UnityEngine.Random.Range(0, i + 1);
            (tracks[i], tracks[j]) = (tracks[j], tracks[i]);
        }
    }

    /// <summary>
    /// Play a single music clip and crossfade to it immediately.
    /// The auto-advance cycle is stopped; use StartMusic() for sequential playback.
    /// </summary>
    public void PlayMusic(AudioClip clip)
    {
        if (clip == null) return;
        StopAllMusicCoroutines();
        _musicPaused = false;
        _musicFadeRoutine = StartCoroutine(CrossfadeMusic(clip));
    }

    /// <summary>Stop music with a fade-out.</summary>
    public void StopMusic(float fadeTime = 1f)
    {
        StopAllMusicCoroutines();
        _musicPaused = false;
        FadeSource(ActiveMusic,   0f, fadeTime);
        FadeSource(InactiveMusic, 0f, fadeTime);
    }

    /// <summary>Pause music playback.</summary>
    public void PauseMusic()
    {
        if (_musicPaused) return;
        _musicPaused = true;

        if (_musicWaitRoutine != null)
        {
            _musicTimeRemaining = Mathf.Max(0f, _musicTimeRemaining - (Time.time - _musicWaitStart));
            StopCoroutine(_musicWaitRoutine);
            _musicWaitRoutine = null;
        }

        ActiveMusic.Pause();
        InactiveMusic.Pause();
    }

    /// <summary>Resume music from where it was paused.</summary>
    public void ResumeMusic()
    {
        if (!_musicPaused) return;
        _musicPaused = false;

        ActiveMusic.UnPause();
        InactiveMusic.UnPause();

        _musicWaitRoutine = StartCoroutine(MusicWaitThenAdvance(_musicTimeRemaining));
    }

    /// <summary>Skip the current music track and immediately crossfade to the next.</summary>
    public void SkipMusic()
    {
        StopAllMusicCoroutines();
        _musicPaused = false;
        _musicCycleRoutine = StartCoroutine(MusicCycle());
    }

    // =========================================================================
    // Public API — SFX
    // =========================================================================

    /// <summary>Play a one-shot sound effect at a given world position (or 2D if null).</summary>
    public void PlaySFX(AudioClip clip, Vector3? worldPos = null)
    {
        if (clip == null) return;
        if (worldPos.HasValue)
        {
            AudioSource.PlayClipAtPoint(clip, worldPos.Value, sfxVolume * masterVolume);
        }
        else
        {
            _sfxSource.volume = sfxVolume * masterVolume;  // <-- set absolute volume first
            _sfxSource.PlayOneShot(clip);                  // no scale arg needed now
        }
    }

    /// <summary>Play a UI click or similar instant effect.</summary>
    public void PlayClick(AudioClip clip) => PlaySFX(clip);

    /// <summary>
    /// Plays a single clip on continuous loop until StopLoopingSFX is called.
    /// Independent of the ambient playlist cycle and the persistent loop track —
    /// use this for things like a toggled rain sound, a machine hum, or any
    /// single looping effect you turn on and off from UI/gameplay code.
    /// Calling this again while already looping swaps to the new clip immediately.
    /// </summary>
    public void PlayLoopingSFX(AudioClip clip, float fadeIn = 0f)
    {
        if (clip == null) return;
        _loopingSFXSource.clip = clip;
        _loopingSFXSource.volume = fadeIn > 0f ? 0f : sfxVolume * masterVolume;
        _loopingSFXSource.Play();
        if (fadeIn > 0f) FadeSource(_loopingSFXSource, sfxVolume * masterVolume, fadeIn);
    }

    /// <summary>Stops whatever clip is currently looping via PlayLoopingSFX.</summary>
    public void StopLoopingSFX(float fadeOut = 0f)
    {
        if (fadeOut > 0f) FadeSource(_loopingSFXSource, 0f, fadeOut);
        else _loopingSFXSource.Stop();
    }

    /// <summary>True if a clip is currently playing via PlayLoopingSFX.</summary>
    public bool IsLoopingSFXPlaying => _loopingSFXSource.isPlaying;

    // =========================================================================
    // Volume helpers
    // =========================================================================

    public void SetMasterVolume()
    {
        if (masterVolumeSlider != null) masterVolume = Mathf.Clamp01(masterVolumeSlider.value);
        RefreshVolumes();
        SaveVolumeSettings();
    }

    public void SetAmbientVolume()
    {
        if (ambientVolumeSlider != null) ambientVolume = Mathf.Clamp01(ambientVolumeSlider.value);
        RefreshVolumes();
        SaveVolumeSettings();
    }

    public void SetMusicVolume()
    {
        // Also drives the loop track's volume — they share this slider.
        if (musicVolumeSlider != null) musicVolume = Mathf.Clamp01(musicVolumeSlider.value);
        RefreshVolumes();
        SaveVolumeSettings();
    }

    public void SetSFXVolume()
    {
        // Also drives the looping SFX slot's volume — they share this slider.
        if (effectsVolumeSlider != null) sfxVolume = Mathf.Clamp01(effectsVolumeSlider.value);
        RefreshVolumes();
        SaveVolumeSettings();
    }

    private void RefreshVolumes()
    {
        if (_ambientA.isPlaying) _ambientA.volume = ambientVolume * masterVolume;
        if (_ambientB.isPlaying) _ambientB.volume = ambientVolume * masterVolume;
        if (_musicA.isPlaying)   _musicA.volume   = musicVolume   * masterVolume;
        if (_musicB.isPlaying)   _musicB.volume   = musicVolume   * masterVolume;
        if (_loopSource.isPlaying) _loopSource.volume = musicVolume * masterVolume;
        if (_loopingSFXSource.isPlaying) _loopingSFXSource.volume = sfxVolume * masterVolume;
    }

    // =========================================================================
    // Save / Load
    // =========================================================================

    private void SaveVolumeSettings()
    {
        PlayerPrefs.SetFloat("Vol_Master",    masterVolume);
        PlayerPrefs.SetFloat("Vol_Ambient",   ambientVolume);
        PlayerPrefs.SetFloat("Vol_Music",     musicVolume);
        PlayerPrefs.SetFloat("Vol_SFX",       sfxVolume);
        PlayerPrefs.Save();
    }

    private void LoadVolumeSettings()
    {
        masterVolume    = PlayerPrefs.GetFloat("Vol_Master",    masterVolume);
        ambientVolume   = PlayerPrefs.GetFloat("Vol_Ambient",   ambientVolume);
        musicVolume     = PlayerPrefs.GetFloat("Vol_Music",     musicVolume);
        sfxVolume       = PlayerPrefs.GetFloat("Vol_SFX",       sfxVolume);
    }

    // =========================================================================
    // Convenience properties
    // =========================================================================

    private AudioSource ActiveAmbient   => _ambientActive ? _ambientA : _ambientB;
    private AudioSource InactiveAmbient => _ambientActive ? _ambientB : _ambientA;
    private AudioSource ActiveMusic     => _musicActive   ? _musicA   : _musicB;
    private AudioSource InactiveMusic   => _musicActive   ? _musicB   : _musicA;

    // =========================================================================
    // Coroutines — Ambient
    // =========================================================================

    private IEnumerator AmbientCycle()
    {
        if (ambientTracks == null || ambientTracks.Length == 0)
        {
            Debug.LogWarning("AudioManager: ambientTracks is empty — AmbientCycle has nothing to play.");
            yield break;
        }

        while (true)
        {
            _ambientIndex = (_ambientIndex + 1) % ambientTracks.Length;
            AudioClip next = ambientTracks[_ambientIndex];

            AudioSource incoming = InactiveAmbient;
            AudioSource outgoing = ActiveAmbient;

            incoming.clip   = next;
            incoming.volume = 0f;
            incoming.Play();
            _ambientActive = !_ambientActive;

            if (_ambientFadeRoutine != null) StopCoroutine(_ambientFadeRoutine);
            _ambientFadeRoutine = StartCoroutine(CrossfadeAmbient(outgoing, incoming));

            // Wait until it's time to crossfade into the next track
            float waitTime = Mathf.Max(0f, next.length - ambientCrossfade);
            _ambientTimeRemaining = waitTime;
            _ambientWaitRoutine   = StartCoroutine(AmbientWaitThenAdvance(waitTime));
            yield return _ambientWaitRoutine;  // suspends until the wait coroutine finishes
        }
    }

    /// <summary>
    /// Waits <paramref name="seconds"/> then signals the cycle to continue.
    /// Stored as a separate coroutine so it can be stopped/restarted on pause/skip.
    /// </summary>
    private IEnumerator AmbientWaitThenAdvance(float seconds)
    {
        _ambientWaitStart = Time.time;
        yield return new WaitForSeconds(seconds);
        _ambientWaitRoutine = null;
        // AmbientCycle is yielding on this coroutine, so it resumes automatically.
    }

    private IEnumerator CrossfadeAmbient(AudioSource from, AudioSource to)
    {
        float t         = 0f;
        float fromStart = from.volume;
        float target    = ambientVolume * masterVolume;

        while (t < ambientCrossfade)
        {
            t += Time.deltaTime;
            float p = Mathf.Clamp01(t / ambientCrossfade);
            from.volume = Mathf.Lerp(fromStart, 0f,     p);
            to.volume   = Mathf.Lerp(0f,        target, p);
            yield return null;
        }

        from.Stop();
        from.volume = 0f;
        to.volume   = target;
    }

    // =========================================================================
    // Coroutines — Music
    // =========================================================================

    private IEnumerator MusicCycle()
    {
        if (musicTracks == null || musicTracks.Length == 0)
        {
            Debug.LogWarning("AudioManager: musicTracks is empty — MusicCycle has nothing to play.");
            yield break;
        }

        while (true)
        {
            _musicIndex = (_musicIndex + 1) % musicTracks.Length;
            AudioClip next = musicTracks[_musicIndex];

            yield return StartCoroutine(CrossfadeMusic(next));

            float waitTime = Mathf.Max(0f, next.length - musicCrossfade);
            _musicTimeRemaining = waitTime;
            _musicWaitRoutine   = StartCoroutine(MusicWaitThenAdvance(waitTime));
            yield return _musicWaitRoutine;
        }
    }

    private IEnumerator MusicWaitThenAdvance(float seconds)
    {
        _musicWaitStart = Time.time;
        yield return new WaitForSeconds(seconds);
        _musicWaitRoutine = null;
    }

    private IEnumerator CrossfadeMusic(AudioClip clip)
    {
        AudioSource incoming = InactiveMusic;
        AudioSource outgoing = ActiveMusic;

        incoming.clip   = clip;
        incoming.volume = 0f;
        incoming.Play();
        _musicActive = !_musicActive;

        float t         = 0f;
        float fromStart = outgoing.volume;
        float target    = musicVolume * masterVolume;

        while (t < musicCrossfade)
        {
            t += Time.deltaTime;
            float p = Mathf.Clamp01(t / musicCrossfade);
            outgoing.volume = Mathf.Lerp(fromStart, 0f,     p);
            incoming.volume = Mathf.Lerp(0f,        target, p);
            yield return null;
        }

        outgoing.Stop();
        outgoing.volume = 0f;
        incoming.volume = target;
    }

    // =========================================================================
    // Helpers
    // =========================================================================

    private void StopAllAmbientCoroutines()
    {
        if (_ambientCycleRoutine != null) { StopCoroutine(_ambientCycleRoutine); _ambientCycleRoutine = null; }
        if (_ambientFadeRoutine  != null) { StopCoroutine(_ambientFadeRoutine);  _ambientFadeRoutine  = null; }
        if (_ambientWaitRoutine  != null) { StopCoroutine(_ambientWaitRoutine);  _ambientWaitRoutine  = null; }
    }

    private void StopAllMusicCoroutines()
    {
        if (_musicCycleRoutine != null) { StopCoroutine(_musicCycleRoutine); _musicCycleRoutine = null; }
        if (_musicFadeRoutine  != null) { StopCoroutine(_musicFadeRoutine);  _musicFadeRoutine  = null; }
        if (_musicWaitRoutine  != null) { StopCoroutine(_musicWaitRoutine);  _musicWaitRoutine  = null; }
    }

    private void FadeSource(AudioSource src, float targetVol, float duration)
        => StartCoroutine(FadeRoutine(src, targetVol, duration));

    private IEnumerator FadeRoutine(AudioSource src, float targetVol, float duration)
    {
        float start = src.volume;
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            src.volume = Mathf.Lerp(start, targetVol, t / duration);
            yield return null;
        }
        src.volume = targetVol;
        if (targetVol <= 0f) src.Stop();
    }

    public void ChangeSceneMusic(AudioClip newClip)
    {
        SetLoopTrack(newClip, true, true);
    }
}
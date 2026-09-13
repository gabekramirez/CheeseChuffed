using UnityEngine;

public class AudioBootstrap : MonoBehaviour
{
    [Header("Library to use for this scene")]
    public AudioLibrary library;

    [Header("Start ambient automatically?")]
    public bool autoStartAmbient = true;

    [Header("Music")]
    public AudioClip sceneMusic;

    private void Start()
    {
        if (library == null)
        {
            Debug.LogWarning(
                "[AudioBootstrap] No AudioLibrary assigned.");
            return;
        }

        var am = AudioManager.Instance;

        if (am == null)
        {
            Debug.LogError(
                "[AudioBootstrap] AudioManager not found.");
            return;
        }

        am.ambientTracks = library.ambientTracks;

        if (autoStartAmbient &&
            library.ambientTracks is { Length: > 0 })
        {
            am.StartAmbient();
        }

        if (sceneMusic != null)
        {
            am.SetLoopTrack(sceneMusic, true, true);
        }
    }

    public void OnPauseAmbient()
    {
        AudioManager.Instance.PauseAmbient();
    }

    public void OnResumeAmbient()
    {
        AudioManager.Instance.ResumeAmbient();
    }

    public void OnSkipAmbient()
    {
        AudioManager.Instance.SkipAmbient();
    }

    public void OnClickButton()
    {
        AudioClip clip = library.GetSFX("click");
        AudioManager.Instance.PlayClick(clip);
    }

    public void PlayWorldSFX(
        string sfxName,
        Vector3 position)
    {
        AudioClip clip = library.GetSFX(sfxName);
        AudioManager.Instance.PlaySFX(clip, position);
    }
}
using UnityEngine;

public class UIAudio : MonoBehaviour
{
    public AudioLibrary library;

    public void PlayAudio(string track)
    {
        AudioManager.Instance.PlaySFX(library.GetSFX(track));
    }

    /// <summary>Starts a looping ambient/SFX clip by name. Keeps looping until StopLoopingAudio is called.</summary>
    public void PlayLoopingAudio(string track, float fadeIn = 0f)
    {
        AudioManager.Instance.PlayLoopingSFX(library.GetSFX(track), fadeIn);
    }

    /// <summary>Stops whatever clip was started with PlayLoopingAudio.</summary>
    public void StopLoopingAudio(float fadeOut = 0f)
    {
        AudioManager.Instance.StopLoopingSFX(fadeOut);
    }
}
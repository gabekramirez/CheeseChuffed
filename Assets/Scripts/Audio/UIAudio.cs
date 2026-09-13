using UnityEngine;

public class UIAudio : MonoBehaviour
{
    public AudioLibrary library;

    public void PlayAudio(string track)
    {
        AudioManager.Instance.PlaySFX(library.GetSFX(track));
    }    

}
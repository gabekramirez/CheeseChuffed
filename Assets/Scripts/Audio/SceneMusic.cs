using UnityEngine;

public class SceneMusic : MonoBehaviour
{
    public AudioClip music;

    private void Start()
    {
        AudioManager.Instance.ChangeSceneMusic(music);
    }
}
using UnityEngine;

/// <summary>
/// ScriptableObject that acts as a named catalogue of audio clips.
/// Create via: Assets → Create → Audio → AudioLibrary
/// Drag clips in from your Project window, then pass them to AudioManager.
/// </summary>
[CreateAssetMenu(menuName = "Audio/AudioLibrary", fileName = "AudioLibrary")]
public class AudioLibrary : ScriptableObject
{
    [Header("Ambient Tracks (played in order, looping)")]
    public AudioClip[] ambientTracks;

    [Header("Music Tracks")]
    public AudioClip[] musicTracks;

    [Header("Sound Effects")]
    public SFXEntry[] sfx;

    // ── Lookup helpers ─────────────────────────────────────────────────────────

    /// <summary>Returns the music clip at the given index, or null.</summary>
    public AudioClip GetMusic(int index)
    {
        if (musicTracks == null || index < 0 || index >= musicTracks.Length) return null;
        return musicTracks[index];
    }

    /// <summary>Returns the SFX clip matching name (case-insensitive), or null.</summary>
    public AudioClip GetSFX(string clipName)
    {
        if (sfx == null) return null;
        foreach (var entry in sfx)
            if (string.Equals(entry.name, clipName, System.StringComparison.OrdinalIgnoreCase))
                return entry.clip;
        return null;
    }
}

[System.Serializable]
public struct SFXEntry
{
    [Tooltip("Identifier used with AudioLibrary.GetSFX(\"...\")")]
    public string    name;
    public AudioClip clip;
}

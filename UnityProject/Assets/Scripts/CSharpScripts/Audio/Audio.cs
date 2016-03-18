using UnityEngine;
using System.Collections;

// ----------------------------------------------------------------------------------------------------
// Audio utility functions
// ----------------------------------------------------------------------------------------------------
public class Audio : MonoBehaviour
{
    // ----------------------------------------------------------------------------------------------------
    // Plays an audio clip at specified position
    // ----------------------------------------------------------------------------------------------------
    public static AudioSource PlayClip(AudioClip _clip, Vector3 _position, float _volume)
    {
        GameObject go = new GameObject("One shot audio");
        go.transform.position = _position;
        AudioSource source = go.AddComponent<AudioSource>();
        // DEPRECATED - Unity 3.1
        // source.rolloffFactor = 0;
        source.clip = _clip;
        source.volume = _volume;
        source.Play();
        Destroy(go, _clip.length);
        return source;
    }
}

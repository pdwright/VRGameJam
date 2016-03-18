using UnityEngine;
using System.Collections;

// ----------------------------------------------------------------------------------------------------
// Plays the game introduction
// ----------------------------------------------------------------------------------------------------
public class LaunchIntro : MonoBehaviour
{
    public AudioClip m_RumbleSound;
    public AudioClip m_BoomSound;
    public ParticleEmitter m_SpawnParticleEmitter;
    public Transform m_ShakeTransform;

    private AudioSource m_ThisAudio;
    private Vector3 m_InitialEulerAngles;

    // ----------------------------------------------------------------------------------------------------
    // Use this for initialization
    // ----------------------------------------------------------------------------------------------------
	void Start()
    {
        m_ThisAudio = audio;
        m_ThisAudio.PlayOneShot(m_RumbleSound, 1.0f);

        m_InitialEulerAngles = m_ShakeTransform.localEulerAngles;

        InvokeRepeating("CameraShake", 0.0f, 0.05f);
        Invoke("Launch", m_RumbleSound.length);
	}

    // ----------------------------------------------------------------------------------------------------
    // Update is called once per frame
    // ----------------------------------------------------------------------------------------------------
	void Update()
    {
	
	}

    // ----------------------------------------------------------------------------------------------------
    // Shakes the camera (moves the transform one time)
    // ----------------------------------------------------------------------------------------------------
    void CameraShake()
    {
        Vector3 eulerAngles = new Vector3(Random.Range(0, 5), Random.Range(0, 5), 0);
        m_ShakeTransform.localEulerAngles = m_InitialEulerAngles + eulerAngles;
    }

    // ----------------------------------------------------------------------------------------------------
    // Launches the particles explosion
    // ----------------------------------------------------------------------------------------------------
    void Launch()
    {
        m_SpawnParticleEmitter.emit = true;
        m_ThisAudio.PlayOneShot(m_BoomSound, 1.0f);
        Invoke("CancelInvoke", 0.5f);
    }
}

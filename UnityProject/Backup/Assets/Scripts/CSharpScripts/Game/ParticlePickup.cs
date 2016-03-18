using UnityEngine;
using System.Collections;

// ----------------------------------------------------------------------------------------------------
// Single orb to be collected by the player
// ----------------------------------------------------------------------------------------------------
public class ParticlePickup : MonoBehaviour
{
    public ParticleEmitter m_Emitter;
    public int m_Index;
    public GameObject m_CollectedParticle;

    // ----------------------------------------------------------------------------------------------------
    // Use this for initialization
    // ----------------------------------------------------------------------------------------------------
	void Start()
    {
	
	}

    // ----------------------------------------------------------------------------------------------------
    // Update is called once per frame
    // ----------------------------------------------------------------------------------------------------
	void Update()
    {
	
	}

    // ----------------------------------------------------------------------------------------------------
    // Pickup an orb
    // ----------------------------------------------------------------------------------------------------
    void OnTriggerEnter(Collider _other)
    {
        ScoreKeeper sk = _other.GetComponent<ScoreKeeper>();
	    sk.Pickup(this);
    }

    // ----------------------------------------------------------------------------------------------------
    // Player picks up the orb (he has enough room to carry it)
    // ----------------------------------------------------------------------------------------------------
    public void Collected()
    {
	    // Spawn particles where the orb was collected
	    Instantiate(m_CollectedParticle, transform.position, Quaternion.identity);
    	
	    // Scale the particle down, so it is no longer visible
 	    Particle[] particles = m_Emitter.particles;
 	    particles[m_Index].size = 0;
 	    m_Emitter.particles = particles;
    	
	    // Destroy the collider for this orb
	    Destroy(gameObject);
    }
}

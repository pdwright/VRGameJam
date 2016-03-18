using UnityEngine;
using System.Collections;

// ----------------------------------------------------------------------------------------------------
// PickupManager handles positioning the pickup particles. The PickupManager uses the children of its
// GameObject as the spawn locations for the pickups in game. It randomly selects a child, and then
// places a particle on top of it.
// ----------------------------------------------------------------------------------------------------
[RequireComponent(typeof(ParticleEmitter))]
public class PickupManager : MonoBehaviour
{
    public GameObject m_ColliderPrefab;
    public DepositTrigger m_DepositTrigger;

    // ----------------------------------------------------------------------------------------------------
    // The Start function is called at the beginning of the level, and is where the placing of the
    // particles is handedled.
    // ----------------------------------------------------------------------------------------------------
	void Start()
    {
	    ParticleEmitter emitter = particleEmitter;
        emitter.ClearParticles();
        emitter.Emit();

        Particle[] myParticles = emitter.particles;
        GameObject toDestroy = new GameObject("ObjectsToDestroy");
        GameObject colliderContainer = new GameObject("ParticleColliders");
        
        for (int i = 0; i < emitter.particleCount; i++)
        {
            if (transform.childCount <= 0)
            {
                break;
            }

            Transform child = transform.GetChild(Random.Range(0, transform.childCount));
            myParticles[i].position = child.position;
            child.parent = toDestroy.transform;

            GameObject prefab = (GameObject)Instantiate(m_ColliderPrefab, myParticles[i].position, Quaternion.identity);
            ParticlePickup pickup = prefab.GetComponent<ParticlePickup>();

            pickup.m_Emitter = emitter;
            pickup.m_Index = i;

            prefab.transform.parent = colliderContainer.transform;
        }

        Destroy(toDestroy);
        emitter.particles = myParticles;
	}

    // ----------------------------------------------------------------------------------------------------
    // Update is called once per frame
    // ----------------------------------------------------------------------------------------------------
	void Update()
    {
	
	}

    // ----------------------------------------------------------------------------------------------------
    // Activates the depository
    // ----------------------------------------------------------------------------------------------------
    void ActivateDepository()
    {
        m_DepositTrigger.ActivateDepository();
    }
}

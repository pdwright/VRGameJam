using UnityEngine;
using System.Collections;

// ----------------------------------------------------------------------------------------------------
// Trigger checking the deposit of orbs
// ----------------------------------------------------------------------------------------------------
public class DepositTrigger : MonoBehaviour
{
    public ParticleEmitter[] m_Emitters;
    public GameObject m_Depository;

    private bool m_ArrowShown = false;

    // ----------------------------------------------------------------------------------------------------
    // Use this for initialization
    // ----------------------------------------------------------------------------------------------------
	void Start()
    {
        foreach (ParticleEmitter emitter in m_Emitters)
        {
            emitter.emit = false;
        }

        DeactivateDepository();

        foreach (Transform child in transform)
        {
            child.gameObject.SetActiveRecursively(false);
        }
	}

    // ----------------------------------------------------------------------------------------------------
    // Update is called once per frame
    // ----------------------------------------------------------------------------------------------------
	void Update()
    {
	
	}

    // ----------------------------------------------------------------------------------------------------
    // Deactivates the depository object
    // ----------------------------------------------------------------------------------------------------
    void DeactivateDepository()
    {
        m_Depository.SendMessage("FadeOut");
    }

    // ----------------------------------------------------------------------------------------------------
    // Activates the depository object
    // ----------------------------------------------------------------------------------------------------
    public void ActivateDepository()
    {
        if (!m_ArrowShown)
        {
            gameObject.SetActiveRecursively(true);
        }

        m_Depository.SendMessage("FadeIn");
    }

    // ----------------------------------------------------------------------------------------------------
    // Deposit from player
    // ----------------------------------------------------------------------------------------------------
    void OnTriggerEnter(Collider _other)
    {
        ActivateDepository();

        foreach (ParticleEmitter emitter in m_Emitters)
        {
            emitter.emit = true;
        }

        _other.SendMessage("Deposit");

        if (!m_ArrowShown)
        {
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }

            m_ArrowShown = true;
        }
    }

    // ----------------------------------------------------------------------------------------------------
    // Deactivates depository effects
    // ----------------------------------------------------------------------------------------------------
    void OnTriggerExit(Collider _other)
    {
        foreach (ParticleEmitter emitter in m_Emitters)
        {
            emitter.emit = false;
        }

        DeactivateDepository();
    }
}

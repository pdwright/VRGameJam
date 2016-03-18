using UnityEngine;
using System.Collections;

// ----------------------------------------------------------------------------------------------------
// Manages the game score and win/lose game state
// ----------------------------------------------------------------------------------------------------
public class ScoreKeeper : MonoBehaviour
{
    int m_Carrying;
    int m_Deposited;

    public int m_CarryLimit;
    public int m_WinScore;
    public int m_GameLength;
    
    public GameObject m_GuiMessage;
    public GUIText m_CarryingGui;
    public GUIText m_DepositedGui;
    public GUIText m_TimerGui;

    public AudioClip[] m_CollectSounds;
    public AudioClip m_WinSound;
    public AudioClip m_LoseSound;
    public AudioClip m_PickupSound;
    public AudioClip m_DepositSound;

    private float m_TimeSinceLastPlay;
    private float m_TimeLeft;

    // ----------------------------------------------------------------------------------------------------
    // Use this for initialization
    // ----------------------------------------------------------------------------------------------------
	void Start()
    {
        m_TimeLeft = m_GameLength;
        m_TimeSinceLastPlay = Time.time;
        UpdateCarryingGui();
        UpdateDepositedGui();
        StartCoroutine(CheckTime());
	}

    // ----------------------------------------------------------------------------------------------------
    // Update is called once per frame
    // ----------------------------------------------------------------------------------------------------
	void Update()
    {
	
	}

    // ----------------------------------------------------------------------------------------------------
    // Updates the text showing carrying number and limit
    // ----------------------------------------------------------------------------------------------------
    void UpdateCarryingGui()
    {
        m_CarryingGui.text = "Carrying: " + m_Carrying + " of " + m_CarryLimit;
    }

    // ----------------------------------------------------------------------------------------------------
    // Updates the text showing deposited number and total needed
    // ----------------------------------------------------------------------------------------------------
    void UpdateDepositedGui()
    {
        m_DepositedGui.text = "Deposited: " + m_Deposited + " of " + m_WinScore;
    }

    // ----------------------------------------------------------------------------------------------------
    // Updates the game timer
    // ----------------------------------------------------------------------------------------------------
    void UpdateTimerGui()
    {
        m_TimerGui.text = "Time: " + TimeRemaining();
    }

    // ----------------------------------------------------------------------------------------------------
    // Manages the remaining game time and ends the game when time is up
    // ----------------------------------------------------------------------------------------------------
    IEnumerator CheckTime()
    {
        while (m_TimeLeft > 0.0f)
        {
            UpdateTimerGui();
            yield return new WaitForSeconds(1);
            m_TimeLeft -= 1.0f;
        }

        UpdateTimerGui();
        StartCoroutine(EndGame());
    }

    // ----------------------------------------------------------------------------------------------------
    // Ends the game showing the result and reloads the level after a player input
    // ----------------------------------------------------------------------------------------------------
    IEnumerator EndGame()
    {
        MainCharacter animationController = GetComponent<MainCharacter>();
	    GameObject prefab = (GameObject)Instantiate(m_GuiMessage);
	    GUIText endMessage = prefab.GetComponent<GUIText>();

	    if (m_Deposited >= m_WinScore)
	    {
		    // Player wins
		    endMessage.text = "You win!";
		    Audio.PlayClip(m_WinSound, Vector3.zero, 1.0f);
		    animationController.m_AnimationTarget.Play("WIN");
	    }
	    else
	    {
		    // Player loses
		    endMessage.text = "Oh no...You lose!";
		    Audio.PlayClip(m_LoseSound, Vector3.zero, 1.0f);
		    animationController.m_AnimationTarget.Play("LOSE");		
	    }
    	
	    // Alert other components on this GameObject that the game has ended
	    SendMessage("OnEndGame");
    	
	    while (true)
	    {
		    // Wait for a touch before reloading the intro level
		    yield return new WaitForFixedUpdate();
            if (iPhoneInputSim.touchCount > 0 && iPhoneInputSim.GetTouch(0).phase == iPhoneTouchPhaseSim.Began)
            {
                break;
            }
	    }
    	
	    Application.LoadLevel(0);
    }

    // ----------------------------------------------------------------------------------------------------
    // Picks up an orb if there is room available for it
    // ----------------------------------------------------------------------------------------------------
    public void Pickup(ParticlePickup _pickup)
    {
	    if (m_Carrying < m_CarryLimit)
	    {
	 	    m_Carrying++;
		    UpdateCarryingGui();	 	
    		
		    // We don't want a voice played for every pickup as this would be annoying.
		    // Only allow a voice to play with a random percentage of chance and only
		    // after a minimum time has passed.
		    float minTimeBetweenPlays = 5.0f;
		    if (Random.value < 0.1f && Time.time > (minTimeBetweenPlays + m_TimeSinceLastPlay))
		    {
			    Audio.PlayClip(m_CollectSounds[Random.Range(0, m_CollectSounds.Length)], Vector3.zero, 0.25f);
			    m_TimeSinceLastPlay = Time.time;
		    }
    		
	 	    _pickup.Collected();
		    Audio.PlayClip(m_PickupSound, _pickup.transform.position, 1.0f);
	    }
	    else
	    {
		    GameObject warning = (GameObject)Instantiate(m_GuiMessage);
		    warning.guiText.text = "You can't carry any more";
		    Destroy(warning, 2.0f);
	    }
    	
	    // Show the player where to deposit the orbs
        if (m_Carrying >= m_CarryLimit)
        {
            _pickup.m_Emitter.SendMessage("ActivateDepository");
        }
    }

    // ----------------------------------------------------------------------------------------------------
    // Depots an orb
    // ----------------------------------------------------------------------------------------------------
    void Deposit()
    {
        m_Deposited += m_Carrying;
        m_Carrying = 0;
        UpdateCarryingGui();
        UpdateDepositedGui();
        Audio.PlayClip(m_DepositSound, transform.position, 1.0f);
    }

    // ----------------------------------------------------------------------------------------------------
    // Gets a string representing the remaining time
    // ----------------------------------------------------------------------------------------------------
    string TimeRemaining()
    {
	    int remaining = (int)m_TimeLeft;
	    string val = "";

        // Minutes
	    if (remaining > 59)
        {
	        val += remaining / 60 + ".";
        }
    	
        // Seconds
	    if (remaining >= 0)
	    {
            string seconds = (remaining % 60).ToString();

            if (seconds.Length < 2)
            {
                // Insert leading 0
                val += "0" + seconds;
            }
            else
            {
                val += seconds;
            }
	    }
    		
	    return val;
    }
}

using UnityEngine;
using System.Collections;

// ----------------------------------------------------------------------------------------------------
// Controls the game menu
// ----------------------------------------------------------------------------------------------------
[RequireComponent(typeof(GUITexture))]
public class ControlMenu : MonoBehaviour
{
    // ----------------------------------------------------------------------------------------------------
    // A scene with a control setup
    // ----------------------------------------------------------------------------------------------------
    [System.Serializable]
    public class ControllerScene
    {
        public string m_Label;
        public string m_ControlScene;
    }

    public Texture2D m_Background;
    public bool m_Display = false;
    public Font m_Font;
    public ControllerScene[] m_Controllers;
    public Transform[] m_DestroyOnLoad;
    public GameObject m_LaunchIntro;
    public GameObject m_ParticleLaunch;
    public GameObject m_OrbEmitter;
    public Transition m_ExplosionCamera;

    private int m_Selection = -1;
    private bool m_DisplayBackground = false;

    // ----------------------------------------------------------------------------------------------------
    // Use this for initialization
    // ----------------------------------------------------------------------------------------------------
    void Start()
    {
        m_LaunchIntro.SetActiveRecursively(false);
        m_OrbEmitter.SetActiveRecursively(false);
    }

    // ----------------------------------------------------------------------------------------------------
    // Update is called once per frame
    // ----------------------------------------------------------------------------------------------------
    void Update()
    {
        if (!m_Display && m_Selection == -1 && iPhoneInputSim.touchCount > 0)
        {
            for (int i = 0; i < iPhoneInputSim.touchCount; i++)
            {
                iPhoneTouchSim touch = iPhoneInputSim.GetTouch(i);

                if (touch.phase == iPhoneTouchPhaseSim.Began && guiTexture.HitTest(touch.position))
                {
                    m_Display = true;
                    m_DisplayBackground = false;
                    guiTexture.enabled = false;
                }
            }
        }
    }

    // ----------------------------------------------------------------------------------------------------
    // When GUI shows
    // ----------------------------------------------------------------------------------------------------
    void OnGUI() 
    {
	    GUI.skin.font = m_Font;

	    if (m_DisplayBackground)
        {
		    GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), m_Background, ScaleMode.StretchToFill, false);
        }
    	
	    if (m_Display)
	    {			
		    int hit = -1;
		    int minHeight = 60;
		    int areaWidth = 400;
		    GUILayout.BeginArea(new Rect((Screen.width - areaWidth) / 2, (Screen.height - minHeight) / 2, areaWidth, minHeight));
		    GUILayout.BeginHorizontal();

            for (int i = 0; i < m_Controllers.Length; i++)
		    {
			    // Show the buttons for all the separate control schemes
			    if (GUILayout.Button(m_Controllers[i].m_Label, GUILayout.MinHeight(minHeight)))
			    {
				    hit = i;
			    }
		    }
    		
		    // If we received a selection, then load those controls
		    if (hit >= 0)
		    {
			    m_Selection = hit;
			    guiTexture.enabled = false;
			    m_Display = false;
			    m_DisplayBackground = false;
                StartCoroutine(ChangeControls());
		    }

		    GUILayout.EndHorizontal();
		    GUILayout.EndArea();
	    }
    }

    // ----------------------------------------------------------------------------------------------------
    // Applies selected control setup
    // ----------------------------------------------------------------------------------------------------
    IEnumerator ChangeControls()
    {
        InstructorCamera camera = FindObjectOfType(typeof(InstructorCamera)) as InstructorCamera;
        camera.AddCut(m_ExplosionCamera);

        foreach (Transform t in m_DestroyOnLoad)
        {
            Destroy(t.gameObject);
        }

        m_LaunchIntro.SetActiveRecursively(true);
        m_ParticleLaunch.SetActiveRecursively(true);

        yield return StartCoroutine(WaitUntilObjectDestroyed(m_LaunchIntro));

        m_DisplayBackground = true;
        m_OrbEmitter.SetActiveRecursively(true);

        Application.LoadLevelAdditive(m_Controllers[m_Selection].m_ControlScene);

        yield return StartCoroutine(WaitUntilDefaultTransitionLoaded());

        // Loads the default game camera
        GameObject defaultCamera = GameObject.Find("Transition_CameraDefault");
        CameraTransition defaultTransition = defaultCamera.GetComponent<CameraTransition>();
        camera.AddCut(defaultTransition);

        Destroy(m_ExplosionCamera);
        Destroy(gameObject, 1.0f);
    }

    // ----------------------------------------------------------------------------------------------------
    // Holds any further execution until the default camera is loaded
    // ----------------------------------------------------------------------------------------------------
    IEnumerator WaitUntilDefaultTransitionLoaded()
    {
        GameObject defaultCamera = GameObject.Find("Transition_CameraDefault");

        while (!defaultCamera)
        {
            yield return new WaitForFixedUpdate();
            defaultCamera = GameObject.Find("Transition_CameraDefault");
        }
    }

    // ----------------------------------------------------------------------------------------------------
    // Holds any further execution while an object still exists
    // ----------------------------------------------------------------------------------------------------
    IEnumerator WaitUntilObjectDestroyed(Object _o)
    {
        while (_o)
        {
            yield return new WaitForFixedUpdate();
        }
    }
}

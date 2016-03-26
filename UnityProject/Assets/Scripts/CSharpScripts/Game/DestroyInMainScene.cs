using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

// ----------------------------------------------------------------------------------------------------
// Attach this script to a game object to have it destroyed when loaded in the main scene
// ----------------------------------------------------------------------------------------------------
public class DestroyInMainScene : MonoBehaviour
{
    // ----------------------------------------------------------------------------------------------------
    // Use this for initialization
    // ----------------------------------------------------------------------------------------------------
	void Start()
    {
		if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            Destroy(gameObject);
        }
	}

    // ----------------------------------------------------------------------------------------------------
    // Update is called once per frame
    // ----------------------------------------------------------------------------------------------------
	void Update()
    {
	
	}
}

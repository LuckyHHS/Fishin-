using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameLoad : MonoBehaviour
{
    //* THis just loads the handlers.

    // PRIVATES
    public static bool loadedHandlers = false;

    // Start is called before the first frame update
    void Start()
    {
        if (SteamLobbies.instance.SwitchingSceneObject != null)
        {
            // Close the switching scene screen.
            SteamLobbies.instance.SwitchingSceneObject.GetComponent<Animator>().SetBool("Open", false);

            // Destroy it.
            Destroy(SteamLobbies.instance.SwitchingSceneObject, 2f);
            SteamLobbies.instance.SwitchingSceneObject = null;
        }   
        if (loadedHandlers == false)
        {
            loadedHandlers = true;
            Debug.Log("Loaded handlers");
            SceneManager.LoadScene("Handlers", LoadSceneMode.Additive);
        }
    }

    
}

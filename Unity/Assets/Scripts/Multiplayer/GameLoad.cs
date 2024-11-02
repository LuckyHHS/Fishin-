using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameLoad : MonoBehaviour
{
    //* THis just loads the handlers.
    // Start is called before the first frame update
    void Start()
    {
        if (!CustomNetworkManager.instance)
        {
            Debug.Log("Loaded handlers");
            SceneManager.LoadScene("Handlers", LoadSceneMode.Additive);
        }
    }

    
}

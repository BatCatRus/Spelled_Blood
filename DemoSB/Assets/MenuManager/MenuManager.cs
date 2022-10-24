using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class MenuManager : MonoBehaviour
{
    
    public void GoToGame()
    {
        SceneTransition.SwitchToScene("Level1");
    }

  
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && SceneManager.GetActiveScene().name == "Level1")
        {
            SceneTransition.SwitchToScene("SceneMenu");
        }
    }
}

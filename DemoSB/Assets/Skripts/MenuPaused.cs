using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuPaused : MonoBehaviour
{
    public GameObject menuPaused; //Переменные
    [SerializeField] KeyCode keyMenuPaused;
    bool isMenuPaused = false;


    private void Start()
    {
        menuPaused.SetActive(false);
    }

    private void Update()
    {
        ActiveMenu();
    }

    void ActiveMenu()
    {
        if (Input.GetKeyDown(keyMenuPaused))
        {
            isMenuPaused = !isMenuPaused;
        }

        if (isMenuPaused)
        {
            menuPaused.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Time.timeScale = 0f;
        }

        else
        {
            menuPaused.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            Time.timeScale = 1f;
        }
    }

    public void MenuPausedContinue()
    {
        isMenuPaused = false;
    }

    public void MenuPausedSettings()
    {
        Debug.Log("Settings");
    }

    public void MenuPausedMainMenu()
    {
        SceneManager.LoadScene("Menu");
    }
}

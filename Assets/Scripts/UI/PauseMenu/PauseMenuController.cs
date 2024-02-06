using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenuController : MonoBehaviour
{
    [Header("UI")]
    public GameObject pauseMenuUI;
    public GameObject inventoryUI;
    public GameObject optionsMenuUI;

    [Header("Player")]
    public GameObject player;

    [Header("Info")]
    public bool isGamePaused;

    void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            if (!isGamePaused)
            {
                PauseGame();
            }
            else
            {
                ResumeGame();
            }
        }
    }
    public void PauseGame()
    {
        isGamePaused = true;

        pauseMenuUI.SetActive(true);
        inventoryUI.SetActive(false);

        player.GetComponent<CharacterController>().FreezeControls();
    }

    public void ResumeGame()
    {
        isGamePaused = false;

        pauseMenuUI.SetActive(false);
        optionsMenuUI.SetActive(false);

        player.GetComponent<CharacterController>().UnfreezeControls();
    }

    public void OpenOptions()
    {
        optionsMenuUI.SetActive(true);
    }

    public void CloseOptions()
    {
        optionsMenuUI.SetActive(false);
    }

    public void ReturnToMainMenu()
    {
        this.GetComponent<LoadingScreenController>().loadScene(0); //Load the main menu
    }
}

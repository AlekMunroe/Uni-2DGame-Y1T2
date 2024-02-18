using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UI_Death : MonoBehaviour
{
    public GameObject deathUI;
    public GameObject enemiesParent;

    public void PlayerDied()
    {
        Debug.Log("Death");

        deathUI.SetActive(true);
        enemiesParent.SetActive(false);

        PlayerPrefs.SetFloat("PlayerHealth", 0);
    }

    public void RestartLevel()
    {
        this.GetComponent<LoadingScreenController>().loadScene(SceneManager.GetActiveScene().buildIndex); //Reload this scene
    }

    public void ExitToMainMenu()
    {
        this.GetComponent<LoadingScreenController>().loadScene(0); //Load the main menu
    }
}

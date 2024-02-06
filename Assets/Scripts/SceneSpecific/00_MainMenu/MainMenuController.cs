using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    [Header("Debugging")]
    public bool debug_Loading;
    public bool debug_Menus;

    [Header("Menu screens")]
    public GameObject menuScreen;
    public GameObject optionsScreen;

    [Header("Controllers")]
    GameObject Controller;

    void Start()
    {
        Controller = this.gameObject;

        OpenMainMenu();
        CloseOptions();
    }
    //----------Buttons----------

    public void StartGame()
    {
        if(PlayerPrefs.GetInt("Single_HasPlayed") == 1)
        {
            if(debug_Loading) Debug.Log("Has played before");

            //Open loading screen
            Controller.GetComponent<LoadingScreenController>().loadScene(PlayerPrefs.GetInt("Scene_Current"));
        }
        else
        {
            if (debug_Loading) Debug.Log("Has not played before");

            PlayerPrefs.SetInt("Scene_Current", 1);

            //Open loading screen
            Controller.GetComponent<LoadingScreenController>().loadScene(PlayerPrefs.GetInt("Scene_Current"));
        }
    }

    public void OpenMainMenu()
    {
        if (debug_Menus) Debug.Log("Open main menu");
        //Open main menu
        menuScreen.SetActive(true);
    }

    public void OpenOptions()
    {
        if (debug_Menus) Debug.Log("Open options");

        //Open options screen
        optionsScreen.SetActive(true);

        //Close menu screen
        menuScreen.SetActive(false);
    }

    public void CloseOptions()
    {
        if (debug_Menus) Debug.Log("Close options");

        //Open menu screen
        menuScreen.SetActive(true);

        //Close options screen
        optionsScreen.SetActive(false);
    }

    public void QuitGame()
    {
        if (debug_Menus) Debug.Log("Quit game");

        //Close game safely

        Application.Quit();

        //Close the game in the editor
#if UNITY_EDITOR || UNITY_EDITOR_64 || UNITY_EDITOR_OSX
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}

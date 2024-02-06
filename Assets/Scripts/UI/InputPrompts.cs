using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputPrompts : MonoBehaviour
{
    string currentController;

    [Header("Debugging")]
    public bool debug_InputPrompt;

    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.GetString("UI_Platform") == null)
        {
            GetPlatform();
        }
        else if (PlayerPrefs.GetString("UI_Platform") == "Computer")
        {
            SetComputer();
        }
        else if (PlayerPrefs.GetString("UI_Platform") == "Playstation")
        {
            SetPlaystation();
        }
        else if (PlayerPrefs.GetString("UI_Platform") == "Xbox")
        {
            SetXbox();
        }
    }

    //----------CHECKING PLATFORM----------
    void GetPlatform()
    {
        RuntimePlatform platform = Application.platform;

        if (platform == RuntimePlatform.WindowsEditor || platform == RuntimePlatform.WindowsPlayer
            || platform == RuntimePlatform.OSXEditor || platform == RuntimePlatform.OSXPlayer)
        {
            PlayerPrefs.SetString("UI_Platform", "Computer");

            SetComputer();
        }
        else if (platform == RuntimePlatform.PS4 || platform == RuntimePlatform.PS5)
        {
            PlayerPrefs.SetString("UI_Platform", "Playstation");

            SetPlaystation();
        }
        else if (platform == RuntimePlatform.XboxOne)
        {
            PlayerPrefs.SetString("UI_Platform", "Xbox");

            SetXbox();
        }
        else //Error correction
        {
            Debug.Log("Unable to detect platform, defaulting to Computer");

            PlayerPrefs.SetString("UI_Platform", "Computer");

            SetComputer();
        }
    }

    //----------PLATFORM SPECIFIC----------

    void SetPlaystation()
    {
        if (debug_InputPrompt) { Debug.Log("Input Prompt: Set Playstation"); }
    }

    void SetComputer()
    {
        if (debug_InputPrompt) { Debug.Log("Input Prompt: Set Computer"); }
    }

    void SetXbox()
    {
        if (debug_InputPrompt) { Debug.Log("Input Prompt: Set Xbox"); }
    }
}

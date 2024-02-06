using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class OptionsMenuController : MonoBehaviour
{
    [Header("Options UI")]
    public Slider volumeSlider;
    public Toggle fullscreenToggle;
    public Toggle cursorToggle; //Adding this since cursor is not required in this game
    public TMP_Dropdown resolutionDropdown;

    [Header("Other")]
    public GameObject player;

    private void Start()
    {
        LoadSettings();
    }

    public void OnVolumeChange()
    {
        AudioListener.volume = volumeSlider.value; //Actually set the volume
        PlayerPrefs.SetFloat("Option_Volume", volumeSlider.value); //Save the volume
    }

    public void OnFullscreenToggle()
    {
        Screen.fullScreen = fullscreenToggle.isOn; //Actually set the fullscreen toggle
        PlayerPrefs.SetInt("Option_Fullscreen", fullscreenToggle.isOn ? 1 : 0); //Save the fullscreen
    }

    public void OnResolutionChange()
    {
        Resolution selectedResolution = Screen.resolutions[resolutionDropdown.value]; //Get the selected resolution from the dropdown
        Screen.SetResolution(selectedResolution.width, selectedResolution.height, Screen.fullScreen); //Actually set the resolution
        PlayerPrefs.SetInt("Option_Resolution", resolutionDropdown.value); //Save the resolution
    }

    void LoadSettings()
    {
        //Volume
        if (PlayerPrefs.HasKey("Option_Volume"))
        {
            float savedVolume = PlayerPrefs.GetFloat("Option_Volume");
            volumeSlider.value = savedVolume;
            AudioListener.volume = savedVolume;
        }

        //Fullscreen
        if (PlayerPrefs.HasKey("Option_Fullscreen"))
        {
            bool isFullscreen = PlayerPrefs.GetInt("Option_Fullscreen") == 1;
            fullscreenToggle.isOn = isFullscreen;
            Screen.fullScreen = isFullscreen;
        }

        //Resolution
        resolutionDropdown.ClearOptions(); //Clear the options in the dropdown

        List<string> options = new List<string>(); //Look through the resolution list

        int currentResolutionIndex = 0; //This is what represents the resolution, it will be the position in the dropdown and the int in the playerpred "Option_Resolution"

        //Add all the resolutions to the dropdown
        for (int i = 0; i < Screen.resolutions.Length; i++)
        {
            Resolution resolution = Screen.resolutions[i];
            string option = resolution.width + " x " + resolution.height;
            options.Add(option);

            if (resolution.width == Screen.currentResolution.width &&
                resolution.height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        //Actually add the resolutions to the dropdown
        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();

        if (PlayerPrefs.HasKey("Option_Resolution"))
        {
            resolutionDropdown.value = PlayerPrefs.GetInt("Option_Resolution");
            OnResolutionChange(); //Actually set the resolution
        }
    }
}

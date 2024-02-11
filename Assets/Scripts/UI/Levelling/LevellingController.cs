using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Audio;

public class LevellingController : MonoBehaviour
{
    [Header("Generic")]
    public GameObject cam;

    [Header("Information")]
    public float currentLevel;
    public float oldLevel;
    public bool isUpdating = false;

    [Header("UI")]
    public Slider slider;
    public TMP_Text levelText;

    public float sliderIncreaseSpeed = 0.5f;

    [Header("Audio")]
    AudioSource audioSource;
    public AudioClip[] levelUpAudio;
    public int levelUpAudioPlay;

    // Start is called before the first frame update
    void Start()
    {
        //Setting the playerpref
        if (!PlayerPrefs.HasKey("PlayerLevel"))
        {
            PlayerPrefs.SetFloat("PlayerLevel", 0);
        }

        currentLevel = PlayerPrefs.GetFloat("PlayerLevel");

        //Text
        levelText.text = "Level: " + Mathf.Floor(currentLevel).ToString("0");

        //Slider
        slider.value = currentLevel - Mathf.Floor(currentLevel);

        oldLevel = currentLevel;

        slider.value = currentLevel - Mathf.Floor(currentLevel);

        //Set the audio
        audioSource = cam.GetComponent<AudioSource>();
    }

    private void Update()
    {
        UpdateUI();

        if (currentLevel > oldLevel || isUpdating)
        {
            SmoothIncreaseSlider();
        }
    }

    public void UpdateUI()
    {
        if ((int)currentLevel > (int)oldLevel)
        {
            Debug.Log("level > oldLevel");

            //Audio
            audioSource.clip = levelUpAudio[levelUpAudioPlay]; //Add levelUpAudio to the audioSource
            audioSource.Play(); //Play the audio

            
        }

        if((int)currentLevel == (int)oldLevel){
            //Update the text and remove the float
            levelText.text = "Level: " + Mathf.Floor(currentLevel).ToString("0");

            //Save the level
            PlayerPrefs.SetFloat("PlayerLevel", currentLevel);
        }
    }

    void SmoothIncreaseSlider()
    {
        //Set the target and start values for the slider
        float targetSliderValue = currentLevel - Mathf.Floor(currentLevel);
        float startSliderValue = slider.value;

        //Get the amount of times the slider needs to fill up
        int levelsIncreased = (int)Mathf.Floor(currentLevel) - (int)Mathf.Floor(oldLevel);

        //Get the amount the slider needs to increase one time
        float fillAmount = sliderIncreaseSpeed * Time.deltaTime;

        if (levelsIncreased >= 1)
        {
            //If the slider needs to fill multiple times, fill the slider to 1
            if (slider.value < 1.0f)
            {
                slider.value += fillAmount;
                if (slider.value >= 1.0f) //If the slider is fill
                {

                    //Update for the next slide increase
                    oldLevel += 1;
                    slider.value = 0;
                    isUpdating = true;
                }
            }
        }
        else
        {
            //increase to the targetSliderValue for the current level
            if (slider.value < targetSliderValue)
            {
                slider.value += fillAmount;
                if (slider.value > targetSliderValue)
                {
                    slider.value = targetSliderValue; //Make sure the slider exactly matches the target
                }
            }

            //Set isUpdating based on if it is updating
            isUpdating = !(Mathf.Approximately(currentLevel, oldLevel + slider.value));
        }

        //If the currentLevel is exactly a whole level e.g. 1.0
        if (Mathf.Floor(currentLevel) > Mathf.Floor(oldLevel) && slider.value == 0)
        {
            isUpdating = true; //Continue updating the slider to make sure the slider matches the currentLevel
        }
    }




}

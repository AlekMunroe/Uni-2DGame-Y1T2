using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevellingController : MonoBehaviour
{
    [Header("Information")]
    public float currentLevel;
    public float oldLevel;
    public bool isUpdating = false;

    [Header("UI")]
    public Slider slider;
    public TMP_Text levelText;

    public float sliderIncreaseSpeed = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        //Setting the playerpref
        if (!PlayerPrefs.HasKey("PlayerLevel"))
        {
            PlayerPrefs.SetFloat("PlayerLevel", 0);
        }

        currentLevel = PlayerPrefs.GetFloat("PlayerLevel");

        //Slider
        slider.value = currentLevel - Mathf.Floor(currentLevel);

        oldLevel = currentLevel;

        slider.value = currentLevel - Mathf.Floor(currentLevel);
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
        //Update the text and remove the float
        levelText.text = "Level: " + Mathf.Floor(currentLevel).ToString("0");

        //Save the level
        PlayerPrefs.SetFloat("PlayerLevel", currentLevel);
    }

    void SmoothIncreaseSlider()
    {
        //Calculate the amount to fill for the current increase amount (0-1)
        float fillAmount = sliderIncreaseSpeed * Time.deltaTime;

        //Calculate the entire amount of times it needs to fill including floats
        float totalRequiredFills = (currentLevel - oldLevel) + (currentLevel - Mathf.Floor(currentLevel));

        //The current progress
        float currentFill = oldLevel + slider.value;

        if (currentFill < currentLevel)
        {
            //If slider.value + oldLevel is less than currentLevel, keep filling
            slider.value += fillAmount;

            //If the slider has reached or passed 1.0, reset
            if (slider.value >= 1.0f)
            {
                slider.value = 0f; //Reset slider
                oldLevel += 1.0f; //Add 1 to oldLevel to show a full fill

                //Error correction, making sure the slider doesnt go over currentLevel
                if (oldLevel > currentLevel)
                {
                    oldLevel = currentLevel;
                }
            }
        }
        else
        {
            //Add the final decimal
            slider.value = currentLevel - Mathf.Floor(currentLevel);
            isUpdating = false;
        }
    }


}

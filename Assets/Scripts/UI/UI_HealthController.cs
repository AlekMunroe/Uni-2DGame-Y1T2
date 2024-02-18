using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_HealthController : MonoBehaviour
{
    //Debugging
    [Header("Debugging")]
    public bool debug_HeartCounter;

    //UI
    [Header("UI")]
    public GameObject[] HeartIcons;
    public Sprite FullHeart;
    public Sprite HalfHeart;

    public GameObject Player;

    //Storing information
    public float activeHearts;
    public float defaultHealth;
    public float maxHealth;


    public void Start()
    {
        //activeHearts = Player.GetComponent<CharacterController>().health;
        defaultHealth = Player.GetComponent<CharacterController>().startingHealth;

        //smaxHealth = Player.GetComponent<CharacterController>().maxHealth;
    }

    public void AddHeart()
    {
        if (activeHearts < defaultHealth)
        {
            activeHearts = activeHearts + 1;

            if (debug_HeartCounter) Debug.Log("Added heart: " + activeHearts);

            if (Player.GetComponent<CharacterController>().useHalfHealth)
            {
                SetHearts_HalfHeart();
            }
            else if (Player.GetComponent<CharacterController>().useHalfHealth == false)
            {
                SetHearts();
            }
        }
    }

    public void AddExtraHeart()
    {
        //If you have three hearts, the extra heart becomes enabled and the extra heart is not enabled (Extra life)
        if (!HeartIcons[3].gameObject.activeInHierarchy)
        {
            activeHearts = maxHealth;

            //Add extra heart
            HeartIcons[0].gameObject.SetActive(true);
            HeartIcons[1].gameObject.SetActive(true);
            HeartIcons[2].gameObject.SetActive(true);
            HeartIcons[3].gameObject.SetActive(true);

            HeartIcons[0].gameObject.GetComponent<Image>().sprite = FullHeart;
            HeartIcons[1].gameObject.GetComponent<Image>().sprite = FullHeart;
            HeartIcons[2].gameObject.GetComponent<Image>().sprite = FullHeart;
            HeartIcons[3].gameObject.GetComponent<Image>().sprite = FullHeart;

            if (debug_HeartCounter) Debug.Log("Add extra heart: " + activeHearts);
        }
    }

    public void RemoveHeart(float attackAmount)
    {
        if (activeHearts <= maxHealth)
        {
            activeHearts = activeHearts - attackAmount;

            if(debug_HeartCounter) Debug.Log("Removed heart: " + activeHearts);

            if (Player.GetComponent<CharacterController>().useHalfHealth)
            {
                SetHearts_HalfHeart();
            }
            else if (Player.GetComponent<CharacterController>().useHalfHealth == false)
            {
                SetHearts();
            }
        }
    }


    public void SetHearts()
    {
        if (activeHearts < maxHealth)
        {
            //Set hearts
            if (activeHearts == 0)
            {
                //Remove all hearts
                HeartIcons[0].gameObject.SetActive(false);
                HeartIcons[1].gameObject.SetActive(false);
                HeartIcons[2].gameObject.SetActive(false);
                HeartIcons[3].gameObject.SetActive(false);
            }
            else if (activeHearts == 1)
            {
                //Set heart 1
                HeartIcons[0].gameObject.SetActive(true);

                HeartIcons[1].gameObject.SetActive(false);
                HeartIcons[2].gameObject.SetActive(false);
                HeartIcons[3].gameObject.SetActive(false);

            }
            else if(activeHearts == 2)
            {
                //Set heart 2
                HeartIcons[0].gameObject.SetActive(true);
                HeartIcons[1].gameObject.SetActive(true);

                HeartIcons[2].gameObject.SetActive(false);
                HeartIcons[3].gameObject.SetActive(false);


            }
            else if(activeHearts == 3)
            {
                //Set heart 3
                HeartIcons[0].gameObject.SetActive(true);
                HeartIcons[1].gameObject.SetActive(true);
                HeartIcons[2].gameObject.SetActive(true);

                HeartIcons[3].gameObject.SetActive(false);
            }
            else
            {
                //Used for error correction
                if(debug_HeartCounter) Debug.Log("AddHeart: Active hearts is not in the scale, resetting to 3 hearts");

                //Reset the active hearts to three
                activeHearts = 3;

                //Restart this function to correct this
                AddHeart();
            }
        }
    }


    //If I want half hearts
    public void SetHearts_HalfHeart()
    {
        if (activeHearts < maxHealth)
        {
            //Set hearts
            if (activeHearts == 0)
            {
                //Remove all hearts
                HeartIcons[0].gameObject.SetActive(false);
                HeartIcons[1].gameObject.SetActive(false);
                HeartIcons[2].gameObject.SetActive(false);
                HeartIcons[3].gameObject.SetActive(false);
            }
            else if (activeHearts == 0.5)
            {
                //0.5 hearts
                HeartIcons[0].gameObject.SetActive(true);

                HeartIcons[0].gameObject.GetComponent<Image>().sprite = HalfHeart;

                HeartIcons[1].gameObject.SetActive(false);
                HeartIcons[2].gameObject.SetActive(false);
                HeartIcons[3].gameObject.SetActive(false);

            }
            else if (activeHearts == 1)
            {
                //1 heart
                HeartIcons[0].gameObject.SetActive(true);

                HeartIcons[0].gameObject.GetComponent<Image>().sprite = FullHeart;

                HeartIcons[1].gameObject.SetActive(false);
                HeartIcons[2].gameObject.SetActive(false);
                HeartIcons[3].gameObject.SetActive(false);


            }
            else if (activeHearts == 1.5)
            {
                //1.5 hearts
                HeartIcons[0].gameObject.SetActive(true);
                HeartIcons[1].gameObject.SetActive(true);

                HeartIcons[1].gameObject.GetComponent<Image>().sprite = HalfHeart;

                HeartIcons[2].gameObject.SetActive(false);
                HeartIcons[3].gameObject.SetActive(false);

            }
            else if (activeHearts == 2)
            {
                //2 hearts

                HeartIcons[0].gameObject.SetActive(true);
                HeartIcons[1].gameObject.SetActive(true);

                HeartIcons[1].gameObject.GetComponent<Image>().sprite = FullHeart;

                HeartIcons[2].gameObject.SetActive(false);
                HeartIcons[3].gameObject.SetActive(false);
            }
            else if (activeHearts == 2.5)
            {
                //2.5 hearts

                HeartIcons[0].gameObject.SetActive(true);
                HeartIcons[1].gameObject.SetActive(true);
                HeartIcons[2].gameObject.SetActive(true);

                HeartIcons[2].gameObject.GetComponent<Image>().sprite = HalfHeart;

                HeartIcons[3].gameObject.SetActive(false);

            }
            else if (activeHearts == 3)
            {
                //3 hearts
                HeartIcons[0].gameObject.SetActive(true);
                HeartIcons[1].gameObject.SetActive(true);
                HeartIcons[2].gameObject.SetActive(true);

                HeartIcons[3].gameObject.SetActive(false); //Extra heart
            }
            else if (activeHearts == 3.5) //Used only if extra heart is used
            {
                //3.5 hearts
                HeartIcons[0].gameObject.SetActive(true);
                HeartIcons[1].gameObject.SetActive(true);
                HeartIcons[2].gameObject.SetActive(true);

                HeartIcons[3].gameObject.SetActive(true); //Extra heart
                HeartIcons[3].gameObject.GetComponent<Image>().sprite = HalfHeart;
            }
            else if (activeHearts == 4) //Used only if extra heart is used
            {
                //4 hearts
                HeartIcons[0].gameObject.SetActive(true);
                HeartIcons[1].gameObject.SetActive(true);
                HeartIcons[2].gameObject.SetActive(false);

                HeartIcons[3].gameObject.SetActive(true); //Extra heart
                HeartIcons[3].gameObject.GetComponent<Image>().sprite = FullHeart;


            }
            else
            {
                //Used for error correction
                if (debug_HeartCounter) Debug.Log("AddHeart: Active hearts is not in the scale, resetting to 3 hearts");

                //Reset the active hearts to three
                activeHearts = 3;

                //Restart this function to correct this
                AddHeart();
            }
        }
    }
}

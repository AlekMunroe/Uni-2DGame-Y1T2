using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Playables;

public class ButtonTutorial_01 : MonoBehaviour
{
    public GameObject interactionUI;
    public TMP_Text interactionText;

    public CutsceneController_01 cutsceneController_01;

    bool isPlayerNearby;

    bool canPressButton = true;

    public PlayableDirector director;

    private void Update()
    {
        if (canPressButton == true)
        {
            if (isPlayerNearby)
            {
                interactionUI.SetActive(true);
                interactionText.text = "Press E to interact";

                if (Input.GetButtonDown("Interact"))
                {
                    isPlayerNearby = false;
                    canPressButton = false;

                    interactionUI.SetActive(false);
                    interactionText.text = "";

                    director.Play();
                }
            }
            else
            {
                interactionUI.SetActive(false);
                interactionText.text = "";
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            isPlayerNearby = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            isPlayerNearby = false;
        }
    }
}

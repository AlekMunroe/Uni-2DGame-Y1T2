using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

[System.Serializable]
public class DialogueLine_Sing
{
    public string characterName;
    public string line;
    public Sprite signSprite;
}

public class SignController : MonoBehaviour
{

    [Header("Generic")]
    public GameObject player;

    [Header("Check radius")]
    public bool waitForInput = true;
    bool isPlayerNearby;

    [Header("Interaction")]
    public GameObject interactionUI;
    public TMP_Text interactionText;
    public bool enableInteractionAfterDialogue;

    [Header("Subtitles")]
    public GameObject subtitlesUI;
    public TMP_Text characterNameText;
    public TMP_Text subtitleText;
    public Image characterImage;

    [Header("Dialogue")]
    public DialogueLine_Sing[] dialogue;
    int dialogueIndex = 0;

    public float dialogueDelay = 0; //Used if you want a delay after changing dialogue

    private void Update()
    {
        //Start dialogue
        if (isPlayerNearby)
        {
            if (Input.GetButtonDown("Interact"))
            {
                //Hide the interaction UI
                this.GetComponent<BoxCollider2D>().enabled = false;
                interactionUI.SetActive(false);

                //Freeze the player
                player.GetComponent<CharacterController>().FreezeControls();

                StartCoroutine(Dialogue());
            }
        }
    }




    //Talk
    IEnumerator Dialogue()
    {
        bool canProgressDialogue = false;

        foreach (DialogueLine_Sing line in dialogue)
        {
            //Update the text and images
            characterNameText.text = line.characterName;
            characterImage.sprite = line.signSprite;
            subtitleText.text = line.line;

            subtitlesUI.SetActive(true);

            //Dialogue delay
            yield return new WaitForSeconds(dialogueDelay);
            canProgressDialogue = true;

            //If the script is ready to progress and you press the interact button
            yield return new WaitUntil(() => canProgressDialogue && Input.GetButtonDown("Interact"));

            //Move the dialogue forward
            dialogueIndex++;

            //Reset progress
            canProgressDialogue = false;
            yield return new WaitForSeconds(dialogueDelay);
        }

        //Unfreeze the player
        player.GetComponent<CharacterController>().UnfreezeControls();

        //Hide dialogue UI after the dialogue is complete
        subtitlesUI.SetActive(false);

        if (enableInteractionAfterDialogue)
        {
            this.GetComponent<BoxCollider2D>().enabled = true;
        }
    }




    //----------TRIGGERS + COLLIDERS----------
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            //Enable the interaction UI "Press (button) to interact"
            interactionUI.SetActive(true);
            interactionText.text = "Press INTERACTION to interact";

            isPlayerNearby = true;

        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            //Disable the interaction UI
            interactionUI.SetActive(false);

            isPlayerNearby = false;
        }
    }
}

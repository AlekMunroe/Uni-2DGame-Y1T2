using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Animations;

[System.Serializable]
public class DialogueLine
{
    public string characterName;
    public string line;
    public Sprite characterSprite;
}

public class NPC_Interact : MonoBehaviour
{
    [Header("Debugging")]
    public bool debug_Animation;

    [Header("Generic")]
    public GameObject player;
    public GameObject thisParent;

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
    public DialogueLine[] dialogue;
    int dialogueIndex = 0;

    public float dialogueDelay = 0; //Used if you want a delay after changing dialogue

    [Header("NPC Movement")]
    public bool moveNPCBeforeDialogue;
    public bool isNPCMoving;
    public float NPCMoveSpeed = 1;

    bool isHorizontalMovement;

    Vector2 lastMovement;

    [Header("Animations")]
    public Animator animator;

    private void Update()
    {
        //Start dialogue
        if (isPlayerNearby)
        {
            if (waitForInput) //Wait for input
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
            else //Skip input
            {
                //Hide the interaction UI
                this.GetComponent<BoxCollider2D>().enabled = false;
                interactionUI.SetActive(false);

                //Freeze the player
                player.GetComponent<CharacterController>().FreezeControls();

                if (moveNPCBeforeDialogue)
                {
                    MoveNPC();
                }
                else
                {
                    StartCoroutine(Dialogue());
                }
            }
        }

        //Making sure the NPC continues to move
        if (isNPCMoving)
        {
            MoveNPC();
        }
    }

    void MoveNPC()
    {
        isNPCMoving = true;

        Vector3 playerPosition = player.transform.position; //Players position
        Vector3 npcPosition = thisParent.transform.position; //NPCs current position

        float stoppingDistance = 1f;

        if (npcPosition.y != playerPosition.y)
        {
            Vector3 newPositionY = new Vector3(npcPosition.x, playerPosition.y, npcPosition.z);
            thisParent.transform.position = Vector3.MoveTowards(npcPosition, newPositionY, NPCMoveSpeed * Time.deltaTime);
        }
        if (npcPosition.y == playerPosition.y && npcPosition.x != playerPosition.x)
        {
            Vector3 newPositionX = new Vector3(playerPosition.x, npcPosition.y, npcPosition.z);
            thisParent.transform.position = Vector3.MoveTowards(npcPosition, newPositionX, NPCMoveSpeed * Time.deltaTime);
        }

        //Check if the NPC is close to the player so it will stop
        if (Vector3.Distance(npcPosition, playerPosition) < stoppingDistance)
        {
            isNPCMoving = false;

            player.GetComponent<CharacterController>().FreezeControls(); //Used to make sure the player is rotated correctly

            PlayIdleAnimation(lastMovement);

            StartCoroutine(Dialogue());
        }


        //Setting animations
        if (isHorizontalMovement)
        {
            //Horizontal movement
            if (npcPosition.x < playerPosition.x)
            {
                //Move right
                PlayMovementAnimation(Vector2.right, 1, 0);
                lastMovement = Vector2.right;
            }
            else
            {
                //Move Left
                PlayMovementAnimation(Vector2.left, -1, 0);
                lastMovement = Vector2.left;
            }
        }
        else
        {
            //Vertical movement
            if (npcPosition.y < playerPosition.y)
            {
                //Move up
                PlayMovementAnimation(Vector2.up, 0, 1);
                lastMovement = Vector2.up;
            }
            else
            {
                //Move down
                PlayMovementAnimation(Vector2.down, 0, -1);
                lastMovement = Vector2.down;
            }
        }
    }




    //Talk
    IEnumerator Dialogue()
    {
        bool canProgressDialogue = false;

        foreach (DialogueLine line in dialogue)
        {
            //Update the text and images
            characterNameText.text = line.characterName;
            characterImage.sprite = line.characterSprite;
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
        if(other.gameObject.tag == "Player")
        {
            //Enable the interaction UI "Press (button) to interact"
            interactionUI.SetActive(true);
            interactionText.text = "Press E to interact";

            isPlayerNearby = true;

        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if(other.gameObject.tag == "Player")
        {
            //Disable the interaction UI
            interactionUI.SetActive(false);

            isPlayerNearby = false;
        }
    }

    //----------ANIMATIONS----------

    //Moving (Walking)
    void PlayMovementAnimation(Vector2 movement, float moveX, float moveY)
    {
        Vector2 lastMovement = Vector2.down;

        if (movement != Vector2.zero)
        {
            if (Mathf.Abs(moveX) > Mathf.Abs(moveY))
            {
                if (moveX > 0)
                {
                    //Walk right
                    animator.Play("WalkRight");
                    lastMovement = Vector2.right;

                    if (debug_Animation) Debug.Log("Walk Right");
                }
                else
                {
                    //Walk left
                    animator.Play("WalkLeft");
                    lastMovement = Vector2.left;

                    if (debug_Animation) Debug.Log("Walk Left");
                }
            }
            else
            {
                if (moveY > 0)
                {
                    //Walk up
                    animator.Play("WalkUp");
                    lastMovement = Vector2.up;

                    if (debug_Animation) Debug.Log("Walk Up");
                }
                else
                {
                    //Walk down
                    animator.Play("WalkDown");
                    lastMovement = Vector2.down;

                    if (debug_Animation) Debug.Log("Walk Down");
                }
            }
        }
        else
        {
            //Play the idle animation with the last movement
            PlayIdleAnimation(lastMovement);
        }
    }

    //Idle
    void PlayIdleAnimation(Vector2 direction)
    {
        if (direction == Vector2.right)
        {
            //Idle right
            animator.Play("IdleRight");

            if (debug_Animation) Debug.Log("Idle Right");
        }
        else if (direction == Vector2.left)
        {
            //Idle left
            animator.Play("IdleLeft");

            if (debug_Animation) Debug.Log("Idle Left");
        }
        else if (direction == Vector2.up)
        {
            //Idle up
            animator.Play("IdleUp");

            if (debug_Animation) Debug.Log("Idle Up");
        }
        else if (direction == Vector2.down)
        {
            //Idle down
            animator.Play("IdleDown");

            if (debug_Animation) Debug.Log("Idle Down");
        }
    }
}

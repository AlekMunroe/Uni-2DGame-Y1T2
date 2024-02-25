using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;


public class CharacterController : MonoBehaviour
{
    //----------VARIABLES----------
    //Debugging
    [Header("Debugging")]
    public bool debug_Movement;
    public bool debug_Idle;
    public bool debug_Attacking;
    public bool debug_Death;
    public bool debug_Health;

    //Generic
    [Header("Generic")]
    public GameObject playerSprite;
    public GameObject worldController;

    private Rigidbody2D rb;

    public GameObject cam;

    //Movement + Animation
    [Header("Movement + Animation")]
    public float moveSpeed = 5.0f;
    float defaultMoveSpeed;

    Vector2 movement;
    Vector2 lastMovement = new Vector2();

    Animator animator;

    public bool canMoveDiagonally = true;

    [Header("Locking/Unlocking")]
    public bool canMove = true;
    public bool canAttack = true;

    public Vector2 playerStopDirection = Vector2.down; //Y-1 = down, Y1 = up, X-1 = left, X1 = right

    //Attacking
    [Header("Attacking")]
    public float attackTime = 0.2f;

    public bool isAttacking;

    public GameObject[] AttackSpots;
    public GameObject attackUI;

    public string attackType = "melee";
    public int attackTypeInt = 0;

    public GameObject[] attackSpots_Fire;

    [Header("Attack delays")]
    public float attackDelay_Fire = 1.5f;
    public bool canAttack_Fire;



    //Health
    [Header("Health")]
    public float startingHealth = 3f; //Used for saving/getting the saved health
    public float health = 3f;
    public float maxHealth;
    public float defaultHealth;
    float internalHealthCounter; //This is used to remember the last health amount temporarily
    public bool useHalfHealth = true;
    public UI_HealthController healthController;

    //Items
    [Header("Items")]
    public List<string> heldItems;
    bool canPickupItem = true;
    public float pickupDelayTime = 0.25f;

    //UI
    [Header("Inventory")]
    public bool isInventoryOpen;

    public GameObject inventoryPanel;
    public InventoryController inventoryController;

    //Audio
    [Header("Audio")]
    AudioSource audioSource;
    public AudioClip[] attackAudio;
    public int attackAudioPlay;

    public AudioClip[] gainAudio;
    public int gainAudioPlay;

    public AudioClip[] healthAudio;
    public int healthAudioPlay;
    public int extraHealthAudioPlay;


    //----------DEFAULT FUNCTIONS----------

    void Start()
    {
        //Linking the variables
        animator = playerSprite.GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        internalHealthCounter = health;
        defaultMoveSpeed = moveSpeed;
        health = startingHealth;
        defaultHealth = startingHealth;
        lastMovement = Vector2.down;

        InitialisePosition();

        InitialiseHealth();

        //Disable the attack spots
        foreach (GameObject spot in AttackSpots)
        {
            if (spot != null)
            {
                spot.SetActive(false);
            }
        }

        InitializeInventory();

        //Set audio
        audioSource = cam.GetComponent<AudioSource>();
}

    void Update()
    {
        //Attacking
        if (Input.GetButton("Fire1") && !isAttacking && canAttack)
        {
            StartCoroutine(Attacking());
        }

        //Change attack type
        if (Input.GetAxis("Mouse ScrollWheel") > 0f)
        {
            attackTypeInt++;
            if(attackTypeInt > 3)
            {
                attackTypeInt = 0;
            }
        }
        else if(Input.GetAxis("Mouse ScrollWheel") < 0f)
        {
            attackTypeInt = attackTypeInt - 1;
            if (attackTypeInt < 0)
            {
                attackTypeInt = 3;
            }
        }

        ChangeAttack();

        //Opening/closing inventory
        if (Input.GetButtonDown("Inventory"))
        {
            if (!isInventoryOpen) //Open inventory
            {
                isInventoryOpen = true;

                inventoryPanel.SetActive(true);

                inventoryController.UpdateInventory(heldItems);
            }
            else //Close inventory
            {
                inventoryController.externalCellSize = inventoryController.cellSize; //Reset externalCellSize for next check

                isInventoryOpen = false;

                inventoryPanel.SetActive(false);
            }
        }

        if (worldController.GetComponent<PauseMenuController>().isGamePaused)
        {
            inventoryController.externalCellSize = inventoryController.cellSize; //Reset externalCellSize for next check

            isInventoryOpen = false;

            inventoryPanel.SetActive(false);
        }
    }

    void FixedUpdate()
    {
        //Movement
        if(canMove) MovePlayer();
    }


    //----------INVENTORY----------
    private void InitializeInventory()
    {
        //Fill the heldItems list with "null" to use as placeholders
        heldItems = new List<string>(new string[inventoryController.maxSlotCount]);

        for (int i = 0; i < inventoryController.maxSlotCount; i++)
        {
            heldItems[i] = null;
        }
    }

    public void SwapHeldItems(int indexOne, int indexTwo)
    {
        Debug.Log("Transfer");
        //Making sure it is swapping an actual item
        if (indexOne >= 0 && indexOne < heldItems.Count && indexTwo >= 0 && indexTwo < heldItems.Count)
        {
            string temp = heldItems[indexOne];
            heldItems[indexOne] = heldItems[indexTwo];
            heldItems[indexTwo] = temp;

            //Error correction
            inventoryController.UpdateInventory(heldItems);
        }
    }





    //----------CONTROLLING THE PLAYER----------

    //Movement
    void MovePlayer()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        

        //Checking to move either X or Y
        if (!canMoveDiagonally)
        {
            //Moving without horizontal movement
            if (Mathf.Abs(moveX) > Mathf.Abs(moveY))
            {
                //Move horizontally
                movement = new Vector2(moveX, 0);
            }
            else
            {
                //Move vertically
                movement = new Vector2(0, moveY);
            }
        }
        else
        {
            //Moving with horizontal movement
            movement = new Vector2(moveX, moveY);
        }

        movement = movement.normalized * moveSpeed;

        //Actually moving
        rb.velocity = movement;

        //Play the movement animations
        if (!isAttacking) { PlayMovementAnimation(movement, moveX, moveY); }
    }

    public void FreezeControls()
    {
        //Attacking
        canAttack = false;

        //Movement
        canMove = false;

        //Stopping movement
        movement = new Vector2(0, 0);
        rb.velocity = movement;


        //Animation
        //Stopping animation
        if (!isAttacking) { PlayIdleAnimation(playerStopDirection); }
    }

    public void UnfreezeControls()
    {
        //Attacking
        canAttack = true;

        //Movement
        canMove = true;
    }


    //Attacking
    public IEnumerator Attacking()
    {
        //Disable repeat attacking
        isAttacking = true;

        PlayAttackAnimation(lastMovement);

        //Give time to play the animation before enabling the attack spot
        yield return new WaitForSeconds(0.05f);

        //Attack type
        if(attackType == "melee")
        {
            //Disable attack specifics

            //Disable fire
            attackSpots_Fire[0].SetActive(false);
            attackSpots_Fire[1].SetActive(false);
            attackSpots_Fire[2].SetActive(false);
            attackSpots_Fire[3].SetActive(false);
        }
        else if(attackType == "fire" && canAttack_Fire)
        {
            //Disable attack specifics

            //Enable attack specifics
            attackSpots_Fire[0].SetActive(true);
            attackSpots_Fire[1].SetActive(true);
            attackSpots_Fire[2].SetActive(true);
            attackSpots_Fire[3].SetActive(true);
        }
        else
        {
            //Unknown, disable all attack specifics

            //Disable fire
            attackSpots_Fire[0].SetActive(false);
            attackSpots_Fire[1].SetActive(false);
            attackSpots_Fire[2].SetActive(false);
            attackSpots_Fire[3].SetActive(false);
        }

        if (lastMovement == Vector2.right)
        {
            //Enable the attack spot
            AttackSpots[0].SetActive(true);

            if (debug_Attacking) Debug.Log("Attack Right");

            //Give a delay to keep the attack spot time 
            yield return new WaitForSeconds(0.5f);

            AttackSpots[0].SetActive(false);
        }
        else if (lastMovement == Vector2.left)
        {
            AttackSpots[1].SetActive(true);

            if (debug_Attacking) Debug.Log("Attack Left");

            yield return new WaitForSeconds(0.2f);

            AttackSpots[1].SetActive(false);
        }
        else if (lastMovement == Vector2.up)
        {
            AttackSpots[2].SetActive(true);

            if (debug_Attacking) Debug.Log("Attack Up");

            yield return new WaitForSeconds(0.2f);

            AttackSpots[2].SetActive(false);
        }
        else if (lastMovement == Vector2.down)
        {
            AttackSpots[3].SetActive(true);

            if (debug_Attacking) Debug.Log("Attack Down");

            yield return new WaitForSeconds(0.2f);

            AttackSpots[3].SetActive(false);
        }

        yield return new WaitForSeconds(attackTime - 0.05f);

        //Enable attacking
        isAttacking = false;
    }

    //Change attack type
    void ChangeAttack()
    {
        if (Input.GetButtonDown("Attack_Change_1"))
        {
            attackTypeInt = 0;
        }
        else if (Input.GetButtonDown("Attack_Change_2"))
        {
            attackTypeInt = 1;
        }
        else if (Input.GetButtonDown("Attack_Change_3"))
        {
            attackTypeInt = 2;
        }
        else if (Input.GetButtonDown("Attack_Change_4"))
        {
            attackTypeInt = 3;
        }


        if (attackTypeInt == 0 || Input.GetButtonDown("Attack_Change_1") || Input.GetAxis("D-Pad_Down-Up") < 0)
        {
            attackTypeInt = 0;

            //Change the type in this script
            attackType = "melee";

            //Change the type directly on the colliders
            AttackSpots[0].GetComponent<AttackInfo>().type = "melee";
            AttackSpots[1].GetComponent<AttackInfo>().type = "melee";
            AttackSpots[2].GetComponent<AttackInfo>().type = "melee";
            AttackSpots[3].GetComponent<AttackInfo>().type = "melee";

            //Change the UI selection
            attackUI.GetComponent<UI_Attack>().changeSelection(0);
        }
        else if (attackTypeInt == 1 || Input.GetButtonDown("Attack_Change_2") || Input.GetAxis("D-Pad_Left-Right") > 0)
        {
            attackTypeInt = 1;

            attackType = "fire";

            AttackSpots[0].GetComponent<AttackInfo>().type = "fire";
            AttackSpots[1].GetComponent<AttackInfo>().type = "fire";
            AttackSpots[2].GetComponent<AttackInfo>().type = "fire";
            AttackSpots[3].GetComponent<AttackInfo>().type = "fire";

            attackUI.GetComponent<UI_Attack>().changeSelection(1);
        }
        else if (attackTypeInt == 2 || Input.GetButtonDown("Attack_Change_3") || Input.GetAxis("D-Pad_Down-Up") > 0)
        {
            attackTypeInt = 2;

            attackType = "Attack 3";

            AttackSpots[0].GetComponent<AttackInfo>().type = "Attack 3";
            AttackSpots[1].GetComponent<AttackInfo>().type = "Attack 3";
            AttackSpots[2].GetComponent<AttackInfo>().type = "Attack 3";
            AttackSpots[3].GetComponent<AttackInfo>().type = "Attack 3";

            attackUI.GetComponent<UI_Attack>().changeSelection(2);
        }
        else if (attackTypeInt == 3 || Input.GetButtonDown("Attack_Change_4") || Input.GetAxis("D-Pad_Left-Right") < 0)
        {
            attackTypeInt = 3;

            attackType = "Attack 4";
            AttackSpots[0].GetComponent<AttackInfo>().type = "Attack 4";
            AttackSpots[1].GetComponent<AttackInfo>().type = "Attack 4";
            AttackSpots[2].GetComponent<AttackInfo>().type = "Attack 4";
            AttackSpots[3].GetComponent<AttackInfo>().type = "Attack 4";

            attackUI.GetComponent<UI_Attack>().changeSelection(3);
        }
    }




    //----------Health----------

    //Initialise health
    void InitialiseHealth()
    {
        if (PlayerPrefs.HasKey("PlayerHealth") && PlayerPrefs.GetFloat("PlayerHealth") > 0.1f)
        {
            if (PlayerPrefs.GetFloat("PlayerHealth") < startingHealth)
            {
                health = PlayerPrefs.GetFloat("PlayerHealth");
            }
        }
        else
        {
            PlayerPrefs.SetFloat("PlayerHealth", startingHealth);
            health = startingHealth;
        }

        //Setting maxHealth correctly
        if (useHalfHealth)
        {
            maxHealth = defaultHealth + 2;
        }
        else if (useHalfHealth == false)
        {
            maxHealth = defaultHealth + 1;
        }
        else
        {
            //Used as the script is setup for health beign either 3 or 6
            Debug.LogError("startingHealth must be either 3 or 6");
        }

        worldController.GetComponent<UI_HealthController>().maxHealth = maxHealth;

        //Update the health UI
        worldController.GetComponent<UI_HealthController>().activeHearts = health;

        if (useHalfHealth)
        {
            worldController.GetComponent<UI_HealthController>().SetHearts_HalfHeart();
        }
        else if (useHalfHealth == false)
        {
            worldController.GetComponent<UI_HealthController>().SetHearts();
        }
    }

    //Check death
    public void checkDeath()
    {
        if(health < 0.1f)
        {
            //The player died
            if (debug_Death) Debug.Log("Player died");

            FreezeControls();
            worldController.GetComponent<UI_Death>().PlayerDied();
        }
        else
        {
            //The player did not die
            PlayerPrefs.SetFloat("PlayerHealth", health);
        }
    }

    //Lose health
    public void LoseHealth(float attackAmount)
    {
        if (health > 0)
        {
            if(debug_Health) Debug.Log("Lose health");

            //Remove health
            health = health - attackAmount;

            internalHealthCounter = health;
            worldController.GetComponent<UI_HealthController>().RemoveHeart(attackAmount);

            //Check if the player is dead
            checkDeath();
        }
    }

    //Add health
    public void addHealth(GameObject other)
    {
        if (health < defaultHealth)
        {
            //Audio
            audioSource.clip = healthAudio[healthAudioPlay]; //Add gainAudio to the audioSource
            audioSource.Play(); //Play the audio

            //Remove the heart
            Destroy(other);

            //Save the destroyed item
            if (other.GetComponent<Identifiers>() != null)
            {
                if (other.GetComponent<Identifiers>().IdentifierName != null)
                {
                    PlayerPrefs.SetInt("Destroyed_Item_" + other.GetComponent<Identifiers>().IdentifierName, 1);
                }
            }

            //Add health
            health = health + 1;

            if(debug_Health) Debug.Log("Add health: " + health);

            internalHealthCounter = health;
            worldController.GetComponent<UI_HealthController>().AddHeart();

            //Check if the player is dead
            checkDeath();
        }
    }

    //Add extra health
    public void addExtraHealth(GameObject other)
    {
        if (health != maxHealth)
        {
            //Audio
            audioSource.clip = healthAudio[extraHealthAudioPlay]; //Add gainAudio to the audioSource
            audioSource.Play(); //Play the audio

            //Remove the heart
            Destroy(other);

            //Save the destroyed item
            if (other.GetComponent<Identifiers>() != null)
            {
                if (other.GetComponent<Identifiers>().IdentifierName != null)
                {
                    PlayerPrefs.SetInt("Destroyed_Item_" + other.GetComponent<Identifiers>().IdentifierName, 1);
                }
            }

            //Add maximum health
            health = maxHealth;

            if(debug_Health) Debug.Log("Add extra health: " + health);

            internalHealthCounter = health;

            worldController.GetComponent<UI_HealthController>().AddExtraHeart();

            //Check if the player is dead
            checkDeath();
        }
    }




    //----------LOCATION----------
    void InitialisePosition()
    {
        if (PlayerPrefs.HasKey("Player_PositionX") && PlayerPrefs.HasKey("Player_PositionY")) //If the positions exist (A save state has been set)
        {
            //Get the saved positions
            float savedX = PlayerPrefs.GetFloat("Player_PositionX");
            float savedY = PlayerPrefs.GetFloat("Player_PositionY");

            //Set the player to the saved positions
            this.transform.position = new Vector3(savedX, savedY, 0);
        }
    }





    //----------ANIMATIONS----------

    //Moving (Walking)
    void PlayMovementAnimation(Vector2 movement, float moveX, float moveY)
    {
        if (movement != Vector2.zero)
        {
            if (Mathf.Abs(moveX) > Mathf.Abs(moveY))
            {
                if (moveX > 0)
                {
                    //Walk right
                    animator.Play("WalkSide");
                    lastMovement = Vector2.right;

                    //Flip sprite (Flipped)
                    bool isFlipped = true;
                    FlipPlayerSprite(isFlipped);

                    if (debug_Movement) Debug.Log("Walk Right");
                }
                else
                {
                    //Walk left
                    animator.Play("WalkSide");
                    lastMovement = Vector2.left;

                    //Flip sprite (Normal)
                    bool isFlipped = false;
                    FlipPlayerSprite(isFlipped);

                    if (debug_Movement) Debug.Log("Walk Left");
                }
            }
            else
            {
                if (moveY > 0)
                {
                    //Walk up
                    animator.Play("WalkUp");
                    lastMovement = Vector2.up;

                    //Flip sprite (Normal)
                    bool isFlipped = false;
                    FlipPlayerSprite(isFlipped);

                    if (debug_Movement) Debug.Log("Walk Up");
                }
                else
                {
                    //Walk down
                    animator.Play("WalkDown");
                    lastMovement = Vector2.down;

                    //Flip sprite (Normal)
                    bool isFlipped = false;
                    FlipPlayerSprite(isFlipped);

                    if (debug_Movement) Debug.Log("Walk Down");
                }
            }
        }
        else
        {
            //Play the idle animation with the last movement
            if (!isAttacking) { PlayIdleAnimation(lastMovement); }
        }
    }

    //Idle
    void PlayIdleAnimation(Vector2 direction)
    {
        if (direction == Vector2.right)
        {
            //Idle right
            animator.Play("IdleRight");

            //Flip sprite (Flipped)
            bool isFlipped = true;
            FlipPlayerSprite(isFlipped);

            if (debug_Idle) Debug.Log("Idle Right");
        }
        else if (direction == Vector2.left)
        {
            //Idle left
            animator.Play("IdleLeft");

            //Flip sprite (Normal)
            bool isFlipped = false;
            FlipPlayerSprite(isFlipped);

            if (debug_Idle) Debug.Log("Idle Left");
        }
        else if (direction == Vector2.up)
        {
            //Idle up
            animator.Play("IdleUp");

            //Flip sprite (Normal)
            bool isFlipped = false;
            FlipPlayerSprite(isFlipped);

            if (debug_Idle) Debug.Log("Idle Up");
        }
        else if (direction == Vector2.down)
        {
            //Idle down
            animator.Play("IdleDown");

            //Flip sprite (Normal)
            bool isFlipped = false;
            FlipPlayerSprite(isFlipped);

            if (debug_Idle) Debug.Log("Idle Down");
        }
    }

    //Attack
    void PlayAttackAnimation(Vector2 direction)
    {
        //Audio
        //audioSource.clip = attackAudio[attackAudioPlay]; //Add attackAudio to the audioSource
        //audioSource.Play(); //Play the audio

        //Melee
        if (attackType == "melee")
        {
            if (direction == Vector2.right)
            {
                //Melee right
                animator.Play("Attack_Melee_Right");

                //Flip sprite (Flipped)
                bool isFlipped = true;
                FlipPlayerSprite(isFlipped);

                if (debug_Attacking) Debug.Log("Melee Right");
            }
            else if (direction == Vector2.left)
            {
                //Melee left
                animator.Play("Attack_Melee_Left");

                //Flip sprite (Normal)
                bool isFlipped = false;
                FlipPlayerSprite(isFlipped);

                if (debug_Attacking) Debug.Log("Melee Left");
            }
            else if (direction == Vector2.up)
            {
                //Melee up
                animator.Play("Attack_Melee_Up");

                //Flip sprite (Normal)
                bool isFlipped = false;
                FlipPlayerSprite(isFlipped);

                if (debug_Attacking) Debug.Log("Melee Up");
            }
            else if (direction == Vector2.down)
            {
                //Melee down
                animator.Play("Attack_Melee_Down");

                //Flip sprite (Normal)
                bool isFlipped = false;
                FlipPlayerSprite(isFlipped);

                if (debug_Attacking) Debug.Log("Melee Down");
            }
        }




        //Fire
        else if (attackType == "fire")
        {
            if (direction == Vector2.right)
            {
                //Melee right
                animator.Play("IdleRight");

                //Flip sprite (Normal)
                bool isFlipped = false;
                FlipPlayerSprite(isFlipped);

                if (debug_Attacking) Debug.Log("Fire Right");
            }
            else if (direction == Vector2.left)
            {
                //Melee left
                animator.Play("IdleLeft");

                //Flip sprite (Flipped)
                bool isFlipped = true;
                FlipPlayerSprite(isFlipped);

                if (debug_Attacking) Debug.Log("Fire Left");
            }
            else if (direction == Vector2.up)
            {
                //Melee up
                animator.Play("IdleUp");

                //Flip sprite (Normal)
                bool isFlipped = false;
                FlipPlayerSprite(isFlipped);

                if (debug_Attacking) Debug.Log("Fire Up");
            }
            else if (direction == Vector2.down)
            {
                //Melee down
                animator.Play("IdleDown");

                //Flip sprite (Normal)
                bool isFlipped = false;
                FlipPlayerSprite(isFlipped);

                if (debug_Attacking) Debug.Log("Fire Down");
            }
        }
    }

    //----------COLLIDERS + TRIGGERS----------

    private void OnTriggerEnter2D(Collider2D other)
    {
        //Health
        if (other.gameObject.tag == "Health")
        {
            //Health
            if (other.gameObject.name == "Heart")
            {
                if (!healthController.fullHearts)
                {
                    //Add health
                    addHealth(other.gameObject);
                }
                else
                {
                    //Add health to inventory
                    if (canPickupItem)
                    {
                        StartCoroutine(PickupItem(other.gameObject));
                    }
                }
            }
            else if(other.gameObject.name == "Heart_Extra")
            {
                if (!healthController.maxHearts)
                {
                    //Add health
                    addExtraHealth(other.gameObject);
                }
                else
                {
                    //Add health to inventory
                    if (canPickupItem)
                    {
                        StartCoroutine(PickupItem(other.gameObject));
                    }
                }
            }
        }

        //World colliders
        if(other.gameObject.tag == "WorldCollider")
        {
            if(other.GetComponent<WorldController>().enabled == true)
            {
                other.GetComponent<WorldController>().RunTask();
            }
            else
            {
                Debug.LogError(other.gameObject.name + " Does not have a WorldController script attached.");
            }
        }

        if(other.gameObject.tag == "Pickupable")
        {
            if(canPickupItem){
                StartCoroutine(PickupItem(other.gameObject));
            }
        }
    }

    IEnumerator PickupItem(GameObject item)
    {
        canPickupItem = false;

        //Find an empty slot in heldItems
        int emptySlotIndex = heldItems.FindIndex(slot => string.IsNullOrEmpty(slot));

        //If an empty slot is found
        if (emptySlotIndex != -1)
        {
            heldItems[emptySlotIndex] = item.gameObject.name; //Replace the empty slot with the picked up item
        }
        else
        {
            //Id there is no empty slot available
            Debug.Log("No empty slot available for " + item.gameObject.name);
        }

        //Destroy the picked up item
        Destroy(item.gameObject);

        //Save the destroyed item
        if(item.GetComponent<Identifiers>() != null)
        {
            if(item.GetComponent<Identifiers>().IdentifierName != null)
            {
                PlayerPrefs.SetInt("Destroyed_Item_" + item.GetComponent<Identifiers>().IdentifierName, 1);
            }
        }

        //Audio
        audioSource.clip = gainAudio[gainAudioPlay]; //Add gainAudio to the audioSource
        audioSource.Play(); //Play the audio

        yield return new WaitForSeconds(pickupDelayTime); //Add a delay to prevent picking up multiple items at once

        canPickupItem = true;
    }


    void FlipPlayerSprite(bool isFlipped)
    {
        if(!isFlipped && playerSprite.transform.localScale.x < 0)
        {
            //Flip back to normal
            playerSprite.transform.localScale = new Vector3(playerSprite.transform.localScale.x * -1, playerSprite.transform.localScale.y, playerSprite.transform.localScale.z);
        }
        else if(isFlipped && playerSprite.transform.localScale.x > 0)
        {
            //Flip sprite reversed
            playerSprite.transform.localScale = new Vector3(-playerSprite.transform.localScale.x, playerSprite.transform.localScale.y, playerSprite.transform.localScale.z);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.Sprites;
using UnityEngine.Audio;

public class EnemiesController : MonoBehaviour
{
    [Header("Debugging")]
    public bool debug_Health;
    public bool debug_Death;
    public bool debug_Attacking;
    public bool debug_Level;

    [Header("Enemies Info")]
    public bool canAttack;
    public bool canFollow;
    public bool canDropItem;
    public float health = 2;

    public bool calculateLevel;

    public bool canAttackState;
    public bool canFollowState;

    [Header("Player")]
    public GameObject player;

    public float playerStopDistance = 1.5f;

    public GameObject cam;

    [Header("Attacking")]
    public float attackTime = 2f;
    public float attackAmount = 1.5f;
    bool onAttackCooldown;
    bool isAttacking;

    [Header("Following")]
    public Transform playerTransform;
    public float followSpeed = 1.5f;
    public float stoppingDistance = 1f; //The distance from the player where the enemy will stop moving
    public LayerMask obstacleMask;
    bool isMoving;

    Vector2 previousPos; //Used for flipping the sprite
    bool isMovingLeft;

    [Header("Death")]
    public GameObject itemToDrop;
    public string droppedItemName;

    public GameObject dropParent;

    bool isDead;

    [Header("World")]
    public GameObject worldController;

    [Header("Animation")]
    public GameObject thisSprite;
    Animator animator;

    public bool canMoveDiagonally = true;

    public float deathAnimationTime = 2.1f;

    [Header("Levelling")]
    public float giveLevel = 0.5f;

    [Header("Audio")]
    AudioSource audioSource;
    public AudioClip[] deathAudio;
    public int deathAudioPlay;

    public AudioClip[] attackAudio;
    public int attackAudioPlay;


    void Start()
    {
        //Setting functions
        //Look for the player
        GameObject[] playersWithTag = GameObject.FindGameObjectsWithTag("Player");
        foreach (var p in playersWithTag)
        {
            if (p.name == "Player")
            {
                player = p.gameObject;
                break; //Stop if the player is found
            }
        }

        if (player == null)
        {
            Debug.LogError("Player not found!");
        }

        //Set the playertransform and previousPos
        playerTransform = player.gameObject.transform;

        previousPos = transform.position;

        //Set the states for pausing and unpausing the game;
        canAttackState = canAttack;
        canFollowState = canFollow;

        //Set the sprite and animator
        thisSprite = this.gameObject;
        animator = this.GetComponent<Animator>();

        //Get the audio
        audioSource = cam.GetComponent<AudioSource>();
    }

    void Update()
    {
        if (!isDead)
        {
            //Move
            if (canFollow && !onAttackCooldown) { Follow(); }

            //Pausing/Unpausing the game
            if (worldController.GetComponent<PauseMenuController>().isGamePaused)
            {
                //If the game pauses
                canAttack = false;
                canFollow = false;
            }
            else if (!worldController.GetComponent<PauseMenuController>().isGamePaused && (canAttack != canAttackState || canFollow != canFollowState))
            {
                //If the game unpauses
                canFollow = canFollowState;
                canAttack = canAttackState;
            }

            if (!isAttacking)
            {

                if (isMoving)
                {
                    PlayMoveAnimation();
                }
                else
                {
                    PlayIdleAnimation();
                }
            }
        }
    }

    //----------CONTROLLING----------
    IEnumerator Death()
    {
        isDead = true;

        //Debug
        if (debug_Death) { Debug.Log(this.gameObject.name + " Died"); }

        PlayDeathAnimation();

        yield return new WaitForSeconds(deathAnimationTime);

        if (canDropItem)
        {
            //Drop item from prefab
            GameObject droppedItem = Instantiate(itemToDrop, this.transform);
            droppedItem.name = droppedItemName; //Rename the item

            droppedItem.transform.parent = dropParent.transform; //Change the parent before it is destroyed
        }

        //Update level
        if (calculateLevel)
        {
            float currentLevel = worldController.GetComponent<LevellingController>().currentLevel + 0.1f;

            //Error correction, make the level act as level 1
            if(currentLevel < 1)
            {
                currentLevel = 1;
            }

            //Calculate the level to give, the higher your level the harder it gets
            giveLevel = currentLevel / (Mathf.Pow(Mathf.Floor(currentLevel), 2) / 2); //I dont understand this properly, I used google for this. Mathf.Pow apparently squares the number
        }

        //Format giveLevel correctly so it is only to two decimal places
        giveLevel = Mathf.Floor(giveLevel * 100) / 100.0f;

        if (debug_Level) Debug.Log("Gave level: " + giveLevel);

        worldController.GetComponent<LevellingController>().currentLevel = giveLevel + worldController.GetComponent<LevellingController>().currentLevel;
        //worldController.GetComponent<LevellingController>().UpdateUI();

        //This should be switched for an animation with a fade coming after
        Destroy(this.gameObject);
    }

    void Follow()
    {
        //Check distance to the player
        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

        //Check if the enemy is within playerStopDistance
        if (distanceToPlayer <= playerStopDistance)
        {
            //Stop moving
            isMoving = false;

            return;
        }

        //If the enemy is far away from the player, move
        if (distanceToPlayer > stoppingDistance)
        {
            //Set direction
            Vector2 directionToPlayer = (playerTransform.position - transform.position).normalized;

            //Check for obstacles
            RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToPlayer, distanceToPlayer, obstacleMask);
            if (hit.collider == null) //If there are no obstacles
            {
                //Actually move
                transform.position = Vector2.MoveTowards(transform.position, playerTransform.position, followSpeed * Time.deltaTime);
                isMoving = true;
            }
            else
            {
                isMoving = false; //Stop moving if hit an obstacle
            }

            //Checking last position for flipping the sprite
            isMovingLeft = (transform.position.x < previousPos.x);
            

            //Update previousPos
            previousPos = transform.position;
        }
        else
        {
            isMoving = false;
        }
    }



    IEnumerator Attack()
    {
        onAttackCooldown = true;
        isAttacking = true;

        if (debug_Attacking) { Debug.Log("Attack"); }

        PlayAttackAnimation();

        //Lose health
        player.GetComponent<CharacterController>().LoseHealth(attackAmount);

        yield return new WaitForSeconds(attackTime);

        isAttacking = false;
        onAttackCooldown = false;

        if (debug_Attacking) { Debug.Log("End attack"); }
    }







    //----------COLLISIONS + TRIGGERS----------

    void OnCollisionEnter2D(Collision2D other)
    {
        //Attacking
        if (other.gameObject.tag == "Player" && canAttack && !isDead)
        {
            if (!onAttackCooldown)
            {
                StartCoroutine(Attack());
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        //Damage
        if (other.gameObject.tag == "Player_AttackSpots")
        {
            var attackInfo = other.gameObject.GetComponent<AttackInfo>();
            if (attackInfo != null) //Check if AttackInfo exists
            {
                //Take away health
                health -= attackInfo.attackAmount;

                //Checking if killed
                if (health < 0.1f)
                {
                    StartCoroutine(Death());
                }

                //Debug
                if (debug_Health) { Debug.Log(this.gameObject.name + " Health: " + health); }
            }
            else
            {
                //Error
                Debug.LogWarning("AttackInfo is not on Player_AttackSpots");
            }
        }
    }




    //----------ANIMATIONS----------

    void PlayIdleAnimation()
    {
        animator.Play("IdleDown");
    }

    void PlayMoveAnimation()
    {
        animator.Play("WalkDown");

        if (isMovingLeft)
        {
            FlipSprite(true);
        }
        else
        {
            FlipSprite(false);
        }
    }

    void PlayAttackAnimation()
    {
        //Animation
        animator.Play("Attack");

        //Audio
        audioSource.clip = attackAudio[attackAudioPlay]; //Add attackAudio to the audioSource
        audioSource.Play(); //Play the audio
    }

    void PlayDeathAnimation()
    {
        //Animation
        animator.Play("Die");

        //Audio
        audioSource.clip = deathAudio[deathAudioPlay]; //Add deathAudio to the audioSource
        audioSource.Play(); //Play the audio
    }
    

    void FlipSprite(bool isFlipped)
    {
        if (!isFlipped && thisSprite.transform.localScale.x < 0)
        {
            //Flip back to normal
            thisSprite.transform.localScale = new Vector3( thisSprite.transform.localScale.x * -1, thisSprite.transform.localScale.y, thisSprite.transform.localScale.z);
        }
        else if (isFlipped && thisSprite.transform.localScale.x > 0)
        {
            //Flip sprite reversed
            thisSprite.transform.localScale = new Vector3(- thisSprite.transform.localScale.x, thisSprite.transform.localScale.y, thisSprite.transform.localScale.z);
        }
    }
}
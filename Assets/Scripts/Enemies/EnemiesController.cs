using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemiesController : MonoBehaviour
{
    [Header("Debugging")]
    public bool debug_Health;
    public bool debug_Death;
    public bool debug_Attacking;

    [Header("Enemies Info")]
    public bool canAttack;
    public bool canFollow;
    public bool canDropItem;
    public float health = 2;

    public bool canAttackState;
    public bool canFollowState;

    [Header("Player")]
    public GameObject player;

    public float playerStopDistance = 1.2f;

    [Header("Attacking")]
    public float attackTime = 2f;
    public float attackAmount = 1.5f;
    bool onAttackCooldown;

    [Header("Following")]
    public Transform playerTransform;
    public float followSpeed = 0.5f;
    public float stoppingDistance = 1f; //The distance from the player where the enemy will stop moving
    public LayerMask obstacleMask;

    [Header("Death")]
    public GameObject itemToDrop;
    public string droppedItemName;

    public GameObject dropParent;

    [Header("World")]
    public GameObject worldController;

    // Start is called before the first frame update
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

        //Set the playertransform
        playerTransform = player.gameObject.transform;

        //Set the states for pausing and unpausing the game;
        canAttackState = canAttack;
        canFollowState = canFollow;
    }

    // Update is called once per frame
    void Update()
    {
        //Following
        if (canFollow) Follow();

        //Pausing/Unpausing the game
        if (worldController.GetComponent<PauseMenuController>().isGamePaused)
        {
            //If the game pauses
            canAttack = false;
            canFollow = false;
        }
        else if(!worldController.GetComponent<PauseMenuController>().isGamePaused && (canAttack != canAttackState || canFollow != canFollowState))
        {
            //If the game unpauses
            canFollow = canFollowState;
            canAttack = canAttackState;
        }
    }

    //----------CONTROLLING----------
    void Death()
    {
        //Debug
        if (debug_Death) { Debug.Log(this.gameObject.name + " Died"); }

        if (canDropItem)
        {
            Debug.Log("Dropped Item");
            //Drop item from prefab
            GameObject droppedItem = Instantiate(itemToDrop, this.transform);
            droppedItem.name = droppedItemName; //Rename the item

            droppedItem.transform.parent = dropParent.transform; //Change the parent before it is destroyed
        }

        //This should be switched for an animation with a fade coming after
        Destroy(this.gameObject);
    }

    void Follow()
    {
        //Check distance to the player
        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

        //If the enemy is far away from the player compared to the stopping distance and closer than 0.5 units, move towards the player
        if (distanceToPlayer > stoppingDistance && distanceToPlayer > playerStopDistance)
        {
            //Set direction
            Vector2 directionToPlayer = (playerTransform.position - transform.position).normalized;

            //Check for obstacles
            RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToPlayer, distanceToPlayer, obstacleMask);
            if (hit.collider == null) // No obstacle detected
            {
                //Actually move
                transform.position = Vector2.MoveTowards(transform.position, playerTransform.position, followSpeed * Time.deltaTime);
            }
            // If an obstacle is detected, you can implement additional logic to handle it, such as waiting or moving around
        }
    }


    IEnumerator Attack()
    {
        onAttackCooldown = true;

        if (debug_Attacking) { Debug.Log("Attack"); }

        //Remove health
        player.GetComponent<CharacterController>().LoseHealth(attackAmount);

        yield return new WaitForSeconds(attackTime);

        onAttackCooldown = false;

        if (debug_Attacking) { Debug.Log("End attack"); }
    }





    //----------COLLISIONS + TRIGGERS----------

    void OnCollisionEnter2D(Collision2D other)
    {
        //Attacking
        if (other.gameObject == player && canAttack)
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
            //melee
            if (other.gameObject.GetComponent<AttackInfo>().type == "melee")
            {
                health = health - other.gameObject.GetComponent<AttackInfo>().attackAmount;
            }

            //Checking if killed
            if (health < 0.1f)
            {
                Death();
            }

            //Debug
            if (debug_Health) { Debug.Log(this.gameObject.name + " Health: " + health); }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationSpots : MonoBehaviour
{
    public GameObject player;
    public Vector2 playerStopDirection = Vector2.down; //Y-1 = down, Y1 = up, X-1 = left, X1 = right

    private void OnTriggerEnter2D(Collider2D other)
    {
        //Health
        if (other.gameObject.tag == "NPC")
        {
            Debug.Log("NPC " + other.gameObject.name + " in " + this.gameObject.name);

            player.GetComponent<CharacterController>().playerStopDirection = playerStopDirection;
        }
    }
}

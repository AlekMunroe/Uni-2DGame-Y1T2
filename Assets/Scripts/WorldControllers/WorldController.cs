using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorldController : MonoBehaviour
{
    [Header("Debugging")]
    public bool debug_Doors;
    [Header("World")]
    public GameObject Player;

    [Header("Task information + Reusable stuff")]
    public string TaskName = "DefaultTask";
    public SpriteRenderer thisSprite;

    [Header("Door")]
    public bool isDoorLocked_Door = true;
    public bool isKeyRequired_Door = true;
    public bool isUsingKeyLink = false; //Used if the key is within another object e.g. an enemy
    public int keyLinkIdentifier;
    public string KeyRequired_Door;
    public Sprite doorClosedSprite_Door;
    public Sprite doorOpenSprite_Door;

    public int Identifier;

    [Header("Save State")]
    public float camRoomLocationX;
    public float camRoomLocationY;
    public int thisCameraNextLocation;

    private void Start()
    {
        //Doors
        if (TaskName == "LockedDoor" || TaskName == "UnlockedDoor" || TaskName == "Door")
        {
            if (Identifier < 1000)
            {
                Debug.LogError("Set Identifier to an int greater than 1000.");
            }

            if (PlayerPrefs.GetInt("SaveState_" + Identifier + "_Door") == 1) //If the door has previously been unlocked
            {
                //Open the door
                isKeyRequired_Door = false;
                isDoorLocked_Door = false;
                RunTask();

                //Destroy the key
                GameObject[] allObjects = FindObjectsOfType<GameObject>();

                foreach (GameObject obj in allObjects)
                {
                    if (obj.name == KeyRequired_Door)
                    {
                        Destroy(obj);
                        break; // Exit the loop if the object is found
                    }

                    if (isUsingKeyLink)
                    {
                        Identifiers identifiersComponent = obj.GetComponent<Identifiers>();
                        if (identifiersComponent == null)
                        {
                            continue; // Skip this iteration if the GameObject does not have an Identifiers component
                        }

                        if (identifiersComponent.Identifier == keyLinkIdentifier) // Check if the Identifier matches the keyLinkIdentifier
                        {
                            Debug.Log("Identified enemy");
                            Destroy(obj);
                            break; // Exit the loop once the identified enemy is found and destroyed
                        }
                    }
                }
            }
        }

        //Save states
    }

    public void RunTask()
    {
        if (TaskName == "LockedDoor" || TaskName == "UnlockedDoor" || TaskName == "Door")
        {
            RunTask_Door();
        }
        else if(TaskName == "SaveLocation")
        {
            RunTask_SaveLocation();
        }
        else
        {
            RunTask_Error();
        }
    }

    //----------ALL TASKS----------
    void RunTask_Error()
    {
        Debug.LogError(this.gameObject.name + " Does not have a TaskName set.");
    }

    //----------DOORS----------
    void RunTask_Door()
    {
        //Error correction
        if (isKeyRequired_Door)
        {
            isDoorLocked_Door = true;
        }

        //Unlocking the door
        if (isDoorLocked_Door && isKeyRequired_Door) //Locked + Door requires key
        {
            //Find index of the key
            int keyIndex = Player.GetComponent<CharacterController>().heldItems.IndexOf(KeyRequired_Door);

            if (keyIndex != -1) //If there is a key
            {
                if (debug_Doors) Debug.Log("Got Key for this door");

                //Change the sprite to the open door sprite
                thisSprite.sprite = doorOpenSprite_Door;

                //Disable the doors collider to allow passage
                thisSprite.gameObject.GetComponent<BoxCollider2D>().enabled = false;

                //Replace the used key item with null, making the slot empty
                Player.GetComponent<CharacterController>().heldItems[keyIndex] = null;

                //Save the unlock state
                PlayerPrefs.SetInt("SaveState_" + Identifier + "_Door", 1);
            }
            else
            {
                if (debug_Doors) Debug.Log(thisSprite.name + " Requires " + KeyRequired_Door + " to unlock this door.");
            }
        }
        else if(!isKeyRequired_Door) //Door does not require key
        {
            isDoorLocked_Door = false; //Since no key is required, automatically set to false

            if (debug_Doors) Debug.Log("No key required, opening " + thisSprite.name);

            //Change the sprite
            thisSprite.sprite = doorOpenSprite_Door;

            //Change the colliders
            thisSprite.gameObject.GetComponent<BoxCollider2D>().enabled = false;
        }
    }



    //----------SAVE LOCATION----------
    public void RunTask_SaveLocation()
    {
        //Get the position of the save location
        Vector3 currentPosition = this.transform.position;

        //Save the X and Y location to be used later
        PlayerPrefs.SetFloat("Player_PositionX", currentPosition.x);
        PlayerPrefs.SetFloat("Player_PositionY", currentPosition.y);

        //Save the camera's location
        PlayerPrefs.SetFloat("Camera_PositionX", camRoomLocationX);
        PlayerPrefs.SetFloat("Camera_PositionY", camRoomLocationY);
        PlayerPrefs.SetInt("Camera_CameraNextLocation", thisCameraNextLocation);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableItemSlot : MonoBehaviour
{
    public Identifiers identifier;

    public GameObject player;
    public GameObject inventoryController;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        inventoryController = GameObject.FindGameObjectWithTag("InventoryController");
    }

    public void ButtonPress()
    {
        if (identifier.IdentifierName == "Heart")
        {
            if (player.GetComponent<CharacterController>().health < player.GetComponent<CharacterController>().defaultHealth)
            {
                //Remove the heart from the heldItems list
                player.GetComponent<CharacterController>().heldItems.Remove("Heart");

                //Update the inventory
                inventoryController.GetComponent<InventoryController>().UpdateInventory(player.GetComponent<CharacterController>().heldItems);

                //Add the health to the player
                player.GetComponent<CharacterController>().addHealth(null);
            }
        }
        else if (identifier.IdentifierName == "Heart_Extra")
        {
            if (player.GetComponent<CharacterController>().health != player.GetComponent<CharacterController>().maxHealth)
            {
                //Remove the heart from the heldItems list
                player.GetComponent<CharacterController>().heldItems.Remove("Heart_Extra");

                //Update the inventory
                inventoryController.GetComponent<InventoryController>().UpdateInventory(player.GetComponent<CharacterController>().heldItems);

                //Add the health to the player
                player.GetComponent<CharacterController>().addExtraHealth(null);
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public GameObject inventorySlotPrefab;
    private List<GameObject> instantiatedSlots = new List<GameObject>();

    public void UpdateInventoryUI(List<string> heldItems)
    {
        //Make sure there are enough slots for the items
        for (int i = instantiatedSlots.Count; i < heldItems.Count; i++)
        {
            GameObject slot = Instantiate(inventorySlotPrefab, transform); //Create a slot
            instantiatedSlots.Add(slot); //Add slot
        }

        //Update the slots using heldItems
        for (int i = 0; i < instantiatedSlots.Count; i++)
        {
            Image iconImage = instantiatedSlots[i].transform.GetChild(0).GetComponent<Image>();
            if (i < heldItems.Count && !string.IsNullOrEmpty(heldItems[i]))
            {
                iconImage.sprite = Resources.Load<Sprite>(heldItems[i]);
                iconImage.enabled = true;
            }
            else
            {
                iconImage.enabled = false; //Hide icon iff null
            }
        }
    }

}

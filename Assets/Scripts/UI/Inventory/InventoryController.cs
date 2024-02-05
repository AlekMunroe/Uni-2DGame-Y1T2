using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class ItemToSprite
{
    public string itemName;
    public Sprite itemSprite;
}

public class InventoryController : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject itemSlotPrefab;
    public List<ItemToSprite> itemsToSprites;
    public Transform slotsParent;

    [Header("Inventory Information")]
    public int maxSlotCount = 10;
    public Vector2 cellSize = new Vector2(200, 200);
    public Vector2 externalCellSize = new Vector2(200, 200); //This will be used within BoxInteraction to change the cell size when necessary

    public List<string> heldItems = new List<string>();

    public void UpdateInventory(List<string> newHeldItems)
    {
        heldItems = new List<string>(newHeldItems);

        //Clear the inventory when it first opens
        foreach (Transform child in slotsParent)
        {
            Destroy(child.gameObject);
        }

        if (externalCellSize != cellSize) //If I want to use an external cell size (if it is different)
        {
            this.GetComponent<GridLayoutGroup>().cellSize = externalCellSize; //Use the external cell size

        }
        else
        {
            this.GetComponent<GridLayoutGroup>().cellSize = cellSize; //Use the default cell size
        }

        int slotsToCreate = Mathf.Max(maxSlotCount, heldItems.Count);

        for (int i = 0; i < slotsToCreate; i++)
        {
            GameObject itemSlot = Instantiate(itemSlotPrefab, slotsParent); //Instantiate the empty item slot
            itemSlot.name = $"ItemSlot_{i}"; //Name the item slot "ItemSlot_NUMBER"
            Image itemImage = itemSlot.transform.Find("ItemImage").GetComponent<Image>(); //Find the image for the item slot

            if (i < heldItems.Count)
            {
                var match = itemsToSprites.Find(item => item.itemName == heldItems[i]); //Check if the name of the item matches the image
                if (match != null)
                {
                    itemImage.sprite = match.itemSprite; //Add the image based on the name (ItemToSprite)
                }
                else
                {
                    itemImage.sprite = null;
                }
            }
            else
            {
                itemImage.sprite = null;
            }
        }
    }

    public void SwapItems(int indexOne, int indexTwo)
    {
        if (indexOne < heldItems.Count && indexTwo < heldItems.Count)
        {
            var temp = heldItems[indexOne];
            heldItems[indexOne] = heldItems[indexTwo];
            heldItems[indexTwo] = temp;

            UpdateInventory(heldItems); //Refresh the UI
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

[System.Serializable]
public class ItemToSprite_Box
{
    public string itemName;
    public Sprite itemSprite;
}

public class BoxInteraction : MonoBehaviour
{

    [Header("Generic")]
    public GameObject player;

    [Header("Check radius")]
    public bool waitForInput = true;
    bool isPlayerNearby;

    [Header("Interaction")]
    public GameObject interactionUI;
    public TMP_Text interactionText;

    bool isBoxOpen;
    bool closeBoxDelay;

    [Header("UI")]
    public GameObject inventoryUI;
    public GameObject boxUI;
    public GameObject boxItemsUI;

    public InventoryController inventoryController;

    [Header("Prefabs")]
    public GameObject itemSlotPrefab;
    public List<ItemToSprite_Box> itemsToSprites;
    public Transform slotsParent;

    [Header("Box information")]
    public int maxSlotCount = 10;
    public Vector2 cellSize = new Vector2(100, 100);

    public List<string> boxItems = new List<string>();

    private void Update()
    {
        //If the player is nearby
        if (isPlayerNearby)
        {
            if (Input.GetButtonDown("Interact") && !isBoxOpen) //If you press interact and the box is not open, open the box
            {
                Debug.Log("Open Box");
                isBoxOpen = true;

                //Hide the interaction UI
                this.GetComponent<BoxCollider2D>().enabled = false;
                interactionUI.SetActive(false);

                //Freeze the player
                player.GetComponent<CharacterController>().FreezeControls();

                inventoryUI.SetActive(true);
                boxUI.SetActive(true);

                slotsParent.GetComponent<GridLayoutGroup>().cellSize = cellSize;
                boxItemsUI.GetComponent<GridLayoutGroup>().cellSize = cellSize;

                inventoryController.externalCellSize = cellSize; //Set the size of the inventory to the cell size set in cellSize of this script
                inventoryController.UpdateInventory(player.GetComponent<CharacterController>().heldItems); //Update the inventory

                StartCoroutine(delayCloseBox());

                OpenBox(boxItems);
            }
        }

        if (!closeBoxDelay && Input.GetButtonDown("Interact") && isBoxOpen) //If you press interact and the box is open, close the box
        {
            Debug.Log("Close Box");

            isBoxOpen = false;

            CloseBox();
        }
    }




    //Talk
    void OpenBox(List<string> newboxItems)
    {
        boxItems = new List<string>(newboxItems);

        //Clear the inventory when it first opens
        foreach (Transform child in slotsParent)
        {
            Destroy(child.gameObject);
        }

        slotsParent.GetComponent<GridLayoutGroup>().cellSize = cellSize;

        int slotsToCreate = Mathf.Max(maxSlotCount, boxItems.Count);

        for (int i = 0; i < slotsToCreate; i++)
        {
            GameObject itemSlot = Instantiate(itemSlotPrefab, slotsParent); //Instantiate the empty item slot
            itemSlot.name = $"ItemSlot_{i}"; //Name the item slot "ItemSlot_NUMBER"
            Image itemImage = itemSlot.transform.Find("ItemImage").GetComponent<Image>(); //Find the image for the item slot

            if (i < boxItems.Count)
            {
                var match = itemsToSprites.Find(item => item.itemName == boxItems[i]); //Check if the name of the item matches the image
                if (match != null)
                {
                    itemImage.sprite = match.itemSprite; //Add the image based on the name (ItemToSprite_Box)
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

    void CloseBox()
    {
        //Hide the interaction UI
        this.GetComponent<BoxCollider2D>().enabled = true;
        interactionUI.SetActive(true);

        //Unfreeze the player
        player.GetComponent<CharacterController>().UnfreezeControls();

        inventoryUI.SetActive(false);
        boxUI.SetActive(false);
    }

    IEnumerator delayCloseBox()
    {
        closeBoxDelay = true;

        yield return new WaitForSeconds(0.5f);

        closeBoxDelay = false;
    }

    private void UpdateBoxUI()
    {
        Debug.Log("Transfer");

        //Clear the existing UI items in the box
        foreach (Transform child in slotsParent)
        {
            Destroy(child.gameObject);
        }

        //Loop items in the box and create aj itemSlot for each
        for (int i = 0; i < boxItems.Count; i++)
        {
            var item = boxItems[i];
            if (!string.IsNullOrEmpty(item))
            {
                //Instantiate an item slot for the box item
                GameObject itemSlot = Instantiate(itemSlotPrefab, slotsParent);
                itemSlot.name = $"BoxItemSlot_{i}"; //Rename the item slot to "BoxItemSlot_NUMBER"

                //Add an item image in the slot
                Image itemImage = itemSlot.transform.Find("ItemImage").GetComponent<Image>();
                var match = itemsToSprites.Find(sprite => sprite.itemName == item);
                if (match != null)
                {
                    itemImage.sprite = match.itemSprite; //Set the sprite to the matched item in ItemToSprite_Box
                }
                else //If there is no sprite in ItemToSprite_Box
                {
                    itemImage.sprite = null; //Do not add a sprite
                }
            }
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

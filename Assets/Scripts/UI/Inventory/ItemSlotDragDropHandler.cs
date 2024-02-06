using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemSlotDragDropHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    private Canvas canvas;
    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;
    private Vector2 originalPosition;
    private bool isDroppedOnTarget = false;
    private CharacterController characterController;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        canvasGroup = gameObject.AddComponent<CanvasGroup>();
        characterController = FindObjectOfType<CharacterController>(); //Find the CharacterController in the scene
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalPosition = rectTransform.anchoredPosition;
        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;
        isDroppedOnTarget = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
        if (!isDroppedOnTarget)
        {
            rectTransform.anchoredPosition = originalPosition;
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("Dropped");
        var itemBeingDropped = eventData.pointerDrag.GetComponent<ItemSlotDragDropHandler>();
        if (itemBeingDropped != null && itemBeingDropped != this)
        {
            //Swap the position of this item and the currently holding item
            Vector2 targetPosition = rectTransform.anchoredPosition;
            rectTransform.anchoredPosition = itemBeingDropped.originalPosition;
            itemBeingDropped.rectTransform.anchoredPosition = targetPosition;

            itemBeingDropped.isDroppedOnTarget = true;

            //Get the int from the index
            int thisSlotIndex = ParseSlotIndex(gameObject.name);
            int droppedSlotIndex = ParseSlotIndex(itemBeingDropped.gameObject.name);

            //Update heldItems in the CharacterController
            if (thisSlotIndex != -1 && droppedSlotIndex != -1)
            {
                characterController.SwapHeldItems(thisSlotIndex, droppedSlotIndex);
            }
        }
    }

    //Get the slots index from the slots name
    private int ParseSlotIndex(string slotName)
    {
        string[] splitName = slotName.Split('_');
        if (splitName.Length > 1 && int.TryParse(splitName[1], out int index))
        {
            return index;
        }
        return -1; //Error correction
    }
}

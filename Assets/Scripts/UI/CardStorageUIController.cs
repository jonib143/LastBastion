using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class CardStorageUIController : MonoBehaviour
{
    [Header("controllers")]
    [SerializeField] private CardStorageController storageController;
    [SerializeField] private TowerPlacementController placementController;
    [SerializeField] private Camera mainCamera;

    [Header("ui slots (size 5)")]
    [SerializeField] private Image[] slotIcons = new Image[CardStorageController.MaxSlots];
    [SerializeField] private RectTransform sellIconRect;
    [SerializeField] private Image dragPreviewIcon;
    [SerializeField] private float uiSlotSnapRadius = 60f;

    private int draggedSlotIndex = -1;
    private bool isDraggingSell = false;

    void Awake()
    {
        if (mainCamera == null) mainCamera = Camera.main;
        if (dragPreviewIcon != null) dragPreviewIcon.gameObject.SetActive(false);
    }

    void OnEnable()
    {
        if (storageController != null)
        {
            storageController.OnStorageUpdated += RefreshUI;
        }
        RefreshUI();
    }

    void OnDisable()
    {
        if (storageController != null)
        {
            storageController.OnStorageUpdated -= RefreshUI;
        }
    }

    void Update()
    {
        if (Mouse.current == null) return;

        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            TryStartDrag(mouseScreenPos);
        }
        else if (Mouse.current.leftButton.isPressed && (draggedSlotIndex != -1 || isDraggingSell))
        {
            UpdateDrag(mouseScreenPos);
        }
        else if (Mouse.current.leftButton.wasReleasedThisFrame && (draggedSlotIndex != -1 || isDraggingSell))
        {
            EndDrag(mouseScreenPos);
        }
    }

    private void TryStartDrag(Vector2 mouseScreenPos)
    {
        // check if player clicked the sell icon
        if (sellIconRect != null)
        {
            float sellDist = ((Vector2)sellIconRect.position - mouseScreenPos).magnitude;
            if (sellDist <= uiSlotSnapRadius)
            {
                isDraggingSell = true;
                ShowDragIcon(sellIconRect.GetComponent<Image>()?.sprite, mouseScreenPos);
                return;
            }
        }

        // check if player clicked one of the 5 storage slots
        int slotIndex = FindClosestSlotIndex(mouseScreenPos);
        if (slotIndex != -1 && storageController.GetCard(slotIndex) != null)
        {
            draggedSlotIndex = slotIndex;
            ShowDragIcon(storageController.GetCard(slotIndex).cardArt, mouseScreenPos);
        }
    }

    private void UpdateDrag(Vector2 mouseScreenPos)
    {
        if (dragPreviewIcon != null)
        {
            dragPreviewIcon.transform.position = mouseScreenPos;
        }

        Vector3 worldPos = ScreenToWorld2D(mouseScreenPos);

        if (isDraggingSell)
        {
            placementController.UpdateSellDrag(worldPos);
        }
        else if (draggedSlotIndex != -1)
        {
            TowerCardSO card = storageController.GetCard(draggedSlotIndex);
            placementController.UpdateCardDrag(card, worldPos);
        }
    }

    private void EndDrag(Vector2 mouseScreenPos)
    {
        if (dragPreviewIcon != null)
        {
            dragPreviewIcon.gameObject.SetActive(false);
        }

        Vector3 worldPos = ScreenToWorld2D(mouseScreenPos);

        if (isDraggingSell)
        {
            placementController.TryDropSell(worldPos);
            isDraggingSell = false;
            return;
        }

        if (draggedSlotIndex != -1)
        {
            // 1. check if dropped onto another storage slot to merge or move
            int targetSlot = FindClosestSlotIndex(mouseScreenPos);
            if (targetSlot != -1 && targetSlot != draggedSlotIndex)
            {
                storageController.TryCombineOrMoveSlot(draggedSlotIndex, targetSlot);
                placementController.CancelDrag();
                draggedSlotIndex = -1;
                return;
            }

            // 2. otherwise try placing or upgrading on a world tower node
            TowerCardSO card = storageController.GetCard(draggedSlotIndex);
            if (placementController.TryDropCard(card, worldPos))
            {
                storageController.RemoveCardAt(draggedSlotIndex);
            }

            draggedSlotIndex = -1;
        }
    }

    private int FindClosestSlotIndex(Vector2 screenPos)
    {
        int bestIndex = -1;
        float bestDist = uiSlotSnapRadius;

        for (int i = 0; i < slotIcons.Length; i++)
        {
            if (slotIcons[i] == null) continue;

            float dist = ((Vector2)slotIcons[i].rectTransform.position - screenPos).magnitude;
            if (dist <= bestDist)
            {
                bestDist = dist;
                bestIndex = i;
            }
        }

        return bestIndex;
    }

    private void ShowDragIcon(Sprite sprite, Vector2 pos)
    {
        if (dragPreviewIcon == null || sprite == null) return;
        dragPreviewIcon.sprite = sprite;
        dragPreviewIcon.transform.position = pos;
        dragPreviewIcon.gameObject.SetActive(true);
    }

    private Vector3 ScreenToWorld2D(Vector2 screenPos)
    {
        Vector3 world = mainCamera.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, -mainCamera.transform.position.z));
        world.z = 0f;
        return world;
    }

    private void RefreshUI()
    {
        if (storageController == null) return;

        for (int i = 0; i < CardStorageController.MaxSlots; i++)
        {
            if (slotIcons[i] == null) continue;

            TowerCardSO card = storageController.GetCard(i);
            if (card != null)
            {
                slotIcons[i].sprite = card.cardArt;
                slotIcons[i].enabled = true;
            }
            else
            {
                slotIcons[i].sprite = null;
                slotIcons[i].enabled = false;
            }
        }
    }
}
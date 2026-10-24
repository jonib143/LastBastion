using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardDraftUIController : MonoBehaviour
{
    [Serializable]
    public struct DraftCardSlotUI
    {
        public GameObject rootObject;
        public Image artImage;
        public TMP_Text nameText;
        public TMP_Text descriptionText;
        public TMP_Text costText;
        public TMP_Text damageText;
        public TMP_Text attackSpeedText;
        public TMP_Text rangeText;
        public Button buyButton;
    }

    [Header("controllers")]
    [SerializeField] private CardDraftController draftController;
    [SerializeField] private CardStorageController storageController;

    [Header("ui elements")]
    [SerializeField] private GameObject draftPanel;
    [SerializeField] private DraftCardSlotUI[] cardSlots = new DraftCardSlotUI[CardDraftController.DraftSlotCount];
    [SerializeField] private Button refreshButton;
    [SerializeField] private TMP_Text refreshCostText;
    [SerializeField] private Button closeButton;

    void Awake()
    {
        for (int i = 0; i < cardSlots.Length; i++)
        {
            int index = i; 
            if (cardSlots[i].buyButton != null)
            {
                cardSlots[i].buyButton.onClick.AddListener(() => OnBuyClicked(index));
            }
        }

        if (refreshButton != null)
        {
            refreshButton.onClick.AddListener(OnRefreshClicked);
        }

        if (closeButton != null)
        {
            closeButton.onClick.AddListener(OnCloseClicked);
        }
    }

    void OnEnable()
    {
        if (draftController != null)
        {
            draftController.OnDraftOpened += ShowPanel;
            draftController.OnDraftUpdated += RefreshUI;
            draftController.OnDraftClosed += HidePanel;
        }

        if (storageController != null)
        {
            storageController.OnStorageUpdated += RefreshUI;
        }

        if (GoldManager.Instance != null)
        {
            GoldManager.Instance.OnGoldChanged += HandleGoldChanged;
        }
    }

    void Start()
    {
        if (GoldManager.Instance != null)
        {
            GoldManager.Instance.OnGoldChanged -= HandleGoldChanged;
            GoldManager.Instance.OnGoldChanged += HandleGoldChanged;
        }

        RefreshUI();
    }

    void OnDisable()
    {
        if (draftController != null)
        {
            draftController.OnDraftOpened -= ShowPanel;
            draftController.OnDraftUpdated -= RefreshUI;
            draftController.OnDraftClosed -= HidePanel;
        }

        if (storageController != null)
        {
            storageController.OnStorageUpdated -= RefreshUI;
        }

        if (GoldManager.Instance != null)
        {
            GoldManager.Instance.OnGoldChanged -= HandleGoldChanged;
        }
    }

    private void OnBuyClicked(int slotIndex)
    {
        if (draftController != null)
        {
            draftController.TryBuyCard(slotIndex);
        }
    }

    private void OnRefreshClicked()
    {
        if (draftController != null)
        {
            draftController.TryRefreshDraft();
        }
    }

    private void OnCloseClicked()
    {
        if (draftController != null)
        {
            draftController.CloseDraft();
        }
    }

    private void HandleGoldChanged(int _)
    {
        RefreshUI();
    }

    private void ShowPanel()
    {
        if (draftPanel != null)
        {
            draftPanel.SetActive(true);
        }
        RefreshUI();
    }

    private void HidePanel()
    {
        if (draftPanel != null)
        {
            draftPanel.SetActive(false);
        }
    }

    private void RefreshUI()
    {
        if (draftController == null) return;

        bool hasSpace = storageController != null && storageController.HasEmptySlot();

        for (int i = 0; i < CardDraftController.DraftSlotCount; i++)
        {
            if (i >= cardSlots.Length) break;

            TowerCardSO card = draftController.GetOffer(i);
            var slot = cardSlots[i];

            if (card == null)
            {
                if (slot.rootObject != null) slot.rootObject.SetActive(false);
                continue;
            }

            if (slot.rootObject != null) slot.rootObject.SetActive(true);
            if (slot.artImage != null) slot.artImage.sprite = card.cardArt;
            if (slot.nameText != null) slot.nameText.text = card.cardName;
            if (slot.descriptionText != null) slot.descriptionText.text = card.description;
            if (slot.costText != null) slot.costText.text = $"{card.GoldCost}g";

            if (card.towerData != null)
            {
                if (slot.damageText != null) slot.damageText.text = $"{card.towerData.damage}";
                if (slot.attackSpeedText != null) slot.attackSpeedText.text = $"{card.towerData.attackSpeed}/s";
                if (slot.rangeText != null) slot.rangeText.text = $"{card.towerData.range}";
            }

            if (slot.buyButton != null)
            {
                bool canAfford = GoldManager.Instance != null && GoldManager.Instance.CanAfford(card.GoldCost);
                slot.buyButton.interactable = hasSpace && canAfford;
            }
        }

        if (refreshCostText != null)
        {
            refreshCostText.text = $"Refresh ({draftController.CurrentRefreshCost}g)";
        }

        if (refreshButton != null)
        {
            bool canAffordRefresh = GoldManager.Instance != null && GoldManager.Instance.CanAfford(draftController.CurrentRefreshCost);
            refreshButton.interactable = canAffordRefresh;
        }
    }
}
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class CardDraftUIController : MonoBehaviour
{
    [Serializable]
    public struct DraftCardSlotUI
    {
        public Image cardArtImage;
        public TMP_Text nameText;
        public TMP_Text descriptionText;
        public TMP_Text costText;
        public Button buyButton;
    }

    [Header("controllers")]
    [SerializeField] private CardDraftController draftController;
    [SerializeField] private CardStorageController storageController;
    [SerializeField] private GoldManager goldManager;

    [Header("ui elements")]
    [SerializeField] private GameObject draftPanel;
    [SerializeField] private DraftCardSlotUI[] cardSlots = new DraftCardSlotUI[CardDraftController.DraftCardCount];
    [SerializeField] private Button refreshButton;
    [SerializeField] private TMP_Text refreshCostText;
    [SerializeField] private Button closeButton;

    private UnityAction[] buyButtonActions;

    void Awake()
    {
        buyButtonActions = new UnityAction[cardSlots.Length];
        for (int i = 0; i < cardSlots.Length; i++)
        {
            int slotIndex = i;
            buyButtonActions[i] = () => OnBuyClicked(slotIndex);
        }
    }

    void OnEnable()
    {
        if (goldManager == null)
        {
            goldManager = GoldManager.Instance;
        }

        if (draftController != null)
        {
            draftController.OnDraftOpened += HandleDraftOpened;
            draftController.OnDraftStateChanged += RefreshUI;
            draftController.OnDraftClosed += HandleDraftClosed;
        }

        if (storageController != null)
        {
            storageController.OnStorageUpdated += RefreshUI;
        }

        if (goldManager != null)
        {
            goldManager.OnGoldChanged += HandleGoldChanged;
        }

        for (int i = 0; i < cardSlots.Length; i++)
        {
            if (cardSlots[i].buyButton != null && i < buyButtonActions.Length)
            {
                cardSlots[i].buyButton.onClick.AddListener(buyButtonActions[i]);
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

    void Start()
    {
        if (goldManager == null && GoldManager.Instance != null)
        {
            goldManager = GoldManager.Instance;
            goldManager.OnGoldChanged += HandleGoldChanged;
        }

        if (draftController != null)
        {
            if (draftPanel != null)
            {
                draftPanel.SetActive(draftController.IsDraftOpen);
            }

            if (draftController.IsDraftOpen)
            {
                RefreshUI();
            }
        }
    }

    void OnDisable()
    {
        if (draftController != null)
        {
            draftController.OnDraftOpened -= HandleDraftOpened;
            draftController.OnDraftStateChanged -= RefreshUI;
            draftController.OnDraftClosed -= HandleDraftClosed;
        }

        if (storageController != null)
        {
            storageController.OnStorageUpdated -= RefreshUI;
        }

        if (goldManager != null)
        {
            goldManager.OnGoldChanged -= HandleGoldChanged;
        }

        for (int i = 0; i < cardSlots.Length; i++)
        {
            if (cardSlots[i].buyButton != null && i < buyButtonActions.Length)
            {
                cardSlots[i].buyButton.onClick.RemoveListener(buyButtonActions[i]);
            }
        }

        if (refreshButton != null)
        {
            refreshButton.onClick.RemoveListener(OnRefreshClicked);
        }

        if (closeButton != null)
        {
            closeButton.onClick.RemoveListener(OnCloseClicked);
        }
    }

    private void HandleDraftOpened()
    {
        if (draftPanel != null)
        {
            draftPanel.SetActive(true);
        }

        RefreshUI();
    }

    private void HandleDraftClosed()
    {
        if (draftPanel != null)
        {
            draftPanel.SetActive(false);
        }
    }

    private void HandleGoldChanged(int currentGold)
    {
        RefreshUI();
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

    private void RefreshUI()
    {
        if (draftController == null || !draftController.IsDraftOpen) return;

        for (int i = 0; i < cardSlots.Length; i++)
        {
            DraftCardSlotUI slotUI = cardSlots[i];
            TowerCardSO card = draftController.GetCardChoice(i);
            bool isPurchased = draftController.IsSlotPurchased(i);

            if (card != null)
            {
                if (slotUI.cardArtImage != null)
                {
                    slotUI.cardArtImage.sprite = card.cardArt;
                    slotUI.cardArtImage.enabled = card.cardArt != null;
                }

                if (slotUI.nameText != null)
                {
                    slotUI.nameText.text = card.cardName;
                }

                if (slotUI.descriptionText != null)
                {
                    slotUI.descriptionText.text = card.description;
                }

                if (slotUI.costText != null)
                {
                    slotUI.costText.text = isPurchased ? "Bought" : $"{card.GoldCost}g";
                }
            }
            else
            {
                if (slotUI.cardArtImage != null)
                {
                    slotUI.cardArtImage.sprite = null;
                    slotUI.cardArtImage.enabled = false;
                }

                if (slotUI.nameText != null) slotUI.nameText.text = string.Empty;
                if (slotUI.descriptionText != null) slotUI.descriptionText.text = string.Empty;
                if (slotUI.costText != null) slotUI.costText.text = string.Empty;
            }

            if (slotUI.buyButton != null)
            {
                slotUI.buyButton.interactable = draftController.CanBuyCard(i);
            }
        }

        if (refreshCostText != null)
        {
            refreshCostText.text = $"Refresh ({draftController.CurrentRefreshCost}g)";
        }

        if (refreshButton != null)
        {
            refreshButton.interactable = draftController.CanRefreshDraft();
        }
    }
}

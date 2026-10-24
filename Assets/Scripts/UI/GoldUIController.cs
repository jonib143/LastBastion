using UnityEngine;
using TMPro;

public class GoldUIController : MonoBehaviour
{
    [Header("references")]
    [SerializeField] private GoldManager goldManager;

    [Header("ui elements")]
    [SerializeField] private TMP_Text goldText;

    void OnEnable()
    {
        if (goldManager == null)
        {
            goldManager = GoldManager.Instance;
        }

        if (goldManager != null)
        {
            goldManager.OnGoldChanged += UpdateGoldLabel;
            UpdateGoldLabel(goldManager.CurrentGold);
        }
    }

    void Start()
    {
        if (goldManager == null && GoldManager.Instance != null)
        {
            goldManager = GoldManager.Instance;
            goldManager.OnGoldChanged += UpdateGoldLabel;
        }

        if (goldManager != null)
        {
            UpdateGoldLabel(goldManager.CurrentGold);
        }
    }

    void OnDisable()
    {
        if (goldManager != null)
        {
            goldManager.OnGoldChanged -= UpdateGoldLabel;
        }
    }

    private void UpdateGoldLabel(int currentGold)
    {
        if (goldText != null)
        {
            goldText.text = $"{currentGold}g";
        }
    }
}

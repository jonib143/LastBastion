using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHealthUIController : MonoBehaviour
{
    [Header("references")]
    [SerializeField] private PlayerHealthManager healthManager;

    [Header("ui bars")]
    [SerializeField] private Image realHpBar;
    [SerializeField] private Image ghostHpBar;
    [SerializeField] private TMP_Text hpText;
    [SerializeField] private GameObject loseUI;

    [Header("ghost animation")]
    [SerializeField] private float delayBeforeEase = 0.25f;
    [SerializeField] private float easeDuration = 0.4f;

    private Coroutine ghostRoutine;

    void OnEnable()
    {
        if (healthManager == null)
        {
            healthManager = PlayerHealthManager.Instance;
        }

        if (healthManager != null)
        {
            healthManager.OnHealthChanged += HandleHealthChanged;
            healthManager.OnPlayerDefeated += HandlePlayerDefeated;
        }
    }

    void Start()
    {
        if (loseUI != null)
        {
            loseUI.SetActive(false);
        }

        // fallback if player health manager awake ran after this component enabled
        if (healthManager == null && PlayerHealthManager.Instance != null)
        {
            healthManager = PlayerHealthManager.Instance;
            healthManager.OnHealthChanged += HandleHealthChanged;
            healthManager.OnPlayerDefeated += HandlePlayerDefeated;
        }

        if (healthManager != null)
        {
            InitializeBars(healthManager.CurrentHp, healthManager.MaxHp);
        }
    }

    void OnDisable()
    {
        if (healthManager != null)
        {
            healthManager.OnHealthChanged -= HandleHealthChanged;
            healthManager.OnPlayerDefeated -= HandlePlayerDefeated;
        }
    }

    private void InitializeBars(float currentHp, float maxHp)
    {
        float targetFill = maxHp > 0f ? Mathf.Clamp01(currentHp / maxHp) : 0f;

        if (realHpBar != null)
        {
            realHpBar.fillAmount = targetFill;
        }

        if (ghostHpBar != null)
        {
            ghostHpBar.fillAmount = targetFill;
        }

        UpdateHpText(currentHp, maxHp);
    }

    private void HandleHealthChanged(float currentHp, float maxHp)
    {
        float targetFill = maxHp > 0f ? Mathf.Clamp01(currentHp / maxHp) : 0f;

        // real hp bar snaps immediately
        if (realHpBar != null)
        {
            realHpBar.fillAmount = targetFill;
        }

        UpdateHpText(currentHp, maxHp);

        if (ghostHpBar == null) return;

        // if healed or initialized higher, snap ghost bar immediately
        if (targetFill >= ghostHpBar.fillAmount)
        {
            if (ghostRoutine != null)
            {
                StopCoroutine(ghostRoutine);
                ghostRoutine = null;
            }

            ghostHpBar.fillAmount = targetFill;
            return;
        }

        // restart ghost delay and ease from its current visual fill
        if (ghostRoutine != null)
        {
            StopCoroutine(ghostRoutine);
        }

        ghostRoutine = StartCoroutine(AnimateGhostBar(targetFill));
    }

    private IEnumerator AnimateGhostBar(float targetFill)
    {
        yield return new WaitForSeconds(delayBeforeEase);

        float initialFill = ghostHpBar.fillAmount;
        float elapsedTime = 0f;

        while (elapsedTime < easeDuration)
        {
            elapsedTime += Time.deltaTime;
            float rawT = Mathf.Clamp01(elapsedTime / easeDuration);
            float easedT = EaseOutQuad(rawT);

            ghostHpBar.fillAmount = Mathf.Lerp(initialFill, targetFill, easedT);
            yield return null;
        }

        ghostHpBar.fillAmount = targetFill;
        ghostRoutine = null;
    }

    private float EaseOutQuad(float t)
    {
        return 1f - (1f - t) * (1f - t);
    }

    private void UpdateHpText(float currentHp, float maxHp)
    {
        if (hpText != null)
        {
            hpText.text = $"{Mathf.CeilToInt(currentHp)} / {Mathf.CeilToInt(maxHp)}";
        }
    }

    private void HandlePlayerDefeated()
    {
        if (loseUI != null)
        {
            loseUI.SetActive(true);
        }
    }
}

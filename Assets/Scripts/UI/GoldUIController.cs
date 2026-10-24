using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GoldUIController : MonoBehaviour
{
    private struct FlyingCoin
    {
        public Transform coinTransform;
        public int goldAmount;
        public bool isUiElement;
    }

    [Header("references")]
    [SerializeField] private GoldManager goldManager;
    [SerializeField] private Camera mainCamera;

    [Header("ui elements")]
    [SerializeField] private TMP_Text goldText;
    [SerializeField] private Transform coinTarget;
    [SerializeField] private Transform punchTarget;

    [Header("flying coin")]
    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private Transform coinSpawnParent;
    [SerializeField] private float coinFlySpeed = 900f;
    [SerializeField] private float coinHitRadius = 25f;

    [Header("bank animation")]
    [SerializeField] private float punchScaleMultiplier = 1.25f;
    [SerializeField] private float punchDuration = 0.2f;
    [SerializeField] private float countEaseDuration = 0.4f;

    private readonly List<FlyingCoin> activeCoins = new List<FlyingCoin>();
    private Coroutine countRoutine;
    private Coroutine punchRoutine;
    private Vector3 basePunchScale = Vector3.one;
    private float displayedGold;
    private int targetGold;
    private bool isInitialized;

    void Awake()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        if (coinTarget == null && goldText != null)
        {
            coinTarget = goldText.transform;
        }

        if (punchTarget == null)
        {
            punchTarget = coinTarget != null ? coinTarget : transform;
        }

        if (punchTarget != null)
        {
            basePunchScale = punchTarget.localScale;
        }
    }

    void OnEnable()
    {
        EnemyController.OnEnemyKilledAtPosition += HandleEnemyKilledAtPosition;

        if (goldManager == null)
        {
            goldManager = GoldManager.Instance;
        }

        if (goldManager != null)
        {
            goldManager.OnGoldChanged += HandleGoldChanged;
        }
    }

    void Start()
    {
        if (goldManager == null && GoldManager.Instance != null)
        {
            goldManager = GoldManager.Instance;
            goldManager.OnGoldChanged += HandleGoldChanged;
        }

        if (goldManager != null)
        {
            InitializeGoldDisplay(goldManager.CurrentGold);
        }
    }

    void OnDisable()
    {
        EnemyController.OnEnemyKilledAtPosition -= HandleEnemyKilledAtPosition;

        if (goldManager != null)
        {
            goldManager.OnGoldChanged -= HandleGoldChanged;
        }
    }

    void Update()
    {
        for (int i = activeCoins.Count - 1; i >= 0; i--)
        {
            FlyingCoin coin = activeCoins[i];
            if (coin.coinTransform == null)
            {
                activeCoins.RemoveAt(i);
                continue;
            }

            Vector3 targetPos = GetTargetPosition(coin.isUiElement);
            Vector3 currentPos = coin.coinTransform.position;
            Vector3 toTarget = targetPos - currentPos;

            float step = coinFlySpeed * Time.deltaTime;
            if (toTarget.magnitude <= coinHitRadius || toTarget.magnitude <= step)
            {
                Destroy(coin.coinTransform.gameObject);
                activeCoins.RemoveAt(i);

                if (goldManager != null)
                {
                    goldManager.AddGold(coin.goldAmount);
                }
                continue;
            }

            Vector3 dir = toTarget.normalized;
            coin.coinTransform.position += dir * step;
        }
    }

    private void InitializeGoldDisplay(int initialGold)
    {
        displayedGold = initialGold;
        targetGold = initialGold;
        isInitialized = true;
        SetGoldLabel(initialGold);
    }

    private void HandleEnemyKilledAtPosition(Vector3 worldPos, int goldAmount)
    {
        if (goldAmount <= 0) return;

        // if no coin prefab is assigned, add gold directly
        if (coinPrefab == null || coinTarget == null || mainCamera == null)
        {
            if (goldManager != null)
            {
                goldManager.AddGold(goldAmount);
            }
            return;
        }

        Transform parent = coinSpawnParent != null ? coinSpawnParent : coinTarget.root;
        GameObject coinObj = Instantiate(coinPrefab, parent);
        bool isUiElement = coinObj.GetComponent<RectTransform>() != null;

        if (isUiElement)
        {
            coinObj.transform.position = mainCamera.WorldToScreenPoint(worldPos);
        }
        else
        {
            coinObj.transform.position = new Vector3(worldPos.x, worldPos.y, 0f);
        }

        activeCoins.Add(new FlyingCoin
        {
            coinTransform = coinObj.transform,
            goldAmount = goldAmount,
            isUiElement = isUiElement
        });
    }

    private void HandleGoldChanged(int newGold)
    {
        if (!isInitialized)
        {
            InitializeGoldDisplay(newGold);
            return;
        }

        bool gainedGold = newGold > targetGold;
        targetGold = newGold;

        if (gainedGold)
        {
            if (punchRoutine != null)
            {
                StopCoroutine(punchRoutine);
            }
            punchRoutine = StartCoroutine(AnimatePunch());
        }

        if (countRoutine != null)
        {
            StopCoroutine(countRoutine);
        }
        countRoutine = StartCoroutine(AnimateGoldCount(targetGold));
    }

    private IEnumerator AnimatePunch()
    {
        if (punchTarget == null) yield break;

        Vector3 peakScale = basePunchScale * punchScaleMultiplier;
        float halfDuration = punchDuration * 0.5f;
        float elapsed = 0f;

        // punch up quickly
        while (elapsed < halfDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / halfDuration);
            punchTarget.localScale = Vector3.Lerp(basePunchScale, peakScale, EaseOutQuad(t));
            yield return null;
        }

        // ease back to original scale
        elapsed = 0f;
        while (elapsed < halfDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / halfDuration);
            punchTarget.localScale = Vector3.Lerp(peakScale, basePunchScale, EaseOutQuad(t));
            yield return null;
        }

        punchTarget.localScale = basePunchScale;
        punchRoutine = null;
    }

    private IEnumerator AnimateGoldCount(int targetValue)
    {
        float startGold = displayedGold;
        float elapsed = 0f;

        while (elapsed < countEaseDuration)
        {
            elapsed += Time.deltaTime;
            float rawT = Mathf.Clamp01(elapsed / countEaseDuration);
            float easedT = EaseOutQuad(rawT);

            displayedGold = Mathf.Lerp(startGold, targetValue, easedT);
            SetGoldLabel(Mathf.RoundToInt(displayedGold));
            yield return null;
        }

        displayedGold = targetValue;
        SetGoldLabel(targetValue);
        countRoutine = null;
    }

    private float EaseOutQuad(float t)
    {
        return 1f - (1f - t) * (1f - t);
    }

    private Vector3 GetTargetPosition(bool isUiElement)
    {
        if (isUiElement)
        {
            return coinTarget.position;
        }

        Vector3 screenPos = coinTarget.position;
        Vector3 world = mainCamera.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, -mainCamera.transform.position.z));
        world.z = 0f;
        return world;
    }

    private void SetGoldLabel(int value)
    {
        if (goldText != null)
        {
            goldText.text = $"{value}";
        }
    }
}

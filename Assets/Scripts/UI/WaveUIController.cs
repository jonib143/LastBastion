using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WaveUIController : MonoBehaviour
{
    [Header("references")]
    [SerializeField] private WaveSpawner waveSpawner;

    [Header("ui elements")]
    [SerializeField] private Button startWaveButton;
    [SerializeField] private TMP_Text waveCounterText;
    [SerializeField] private GameObject winUI;

    [Header("counter animation")]
    [SerializeField] private float punchScaleMultiplier = 1.2f;
    [SerializeField] private float punchDuration = 0.2f;

    private Coroutine punchRoutine;
    private Vector3 baseTextScale = Vector3.one;

    void Awake()
    {
        if (waveCounterText != null)
        {
            baseTextScale = waveCounterText.transform.localScale;
        }
    }

    void OnEnable()
    {
        if (waveSpawner != null)
        {
            waveSpawner.OnWaveStarted += HandleWaveStarted;
            waveSpawner.OnWaveCleared += HandleWaveCleared;
            waveSpawner.OnAllWavesCleared += HandleAllWavesCleared;
        }

        if (startWaveButton != null)
        {
            startWaveButton.onClick.AddListener(OnStartWaveClicked);
        }
    }

    void OnDisable()
    {
        if (waveSpawner != null)
        {
            waveSpawner.OnWaveStarted -= HandleWaveStarted;
            waveSpawner.OnWaveCleared -= HandleWaveCleared;
            waveSpawner.OnAllWavesCleared -= HandleAllWavesCleared;
        }

        if (startWaveButton != null)
        {
            startWaveButton.onClick.RemoveListener(OnStartWaveClicked);
        }
    }

    void Start()
    {
        if (winUI != null)
        {
            winUI.SetActive(false);
        }

        if (waveCounterText != null && waveSpawner != null)
        {
            waveCounterText.text = $"{waveSpawner.CurrentWave}/{waveSpawner.TotalWaves}";
        }
    }

    private void OnStartWaveClicked()
    {
        if (startWaveButton != null)
        {
            startWaveButton.gameObject.SetActive(false);
        }

        if (waveSpawner != null)
        {
            waveSpawner.StartNextWave();
        }
    }

    private void HandleWaveStarted(int currentWave, int totalWaves)
    {
        if (waveCounterText != null)
        {
            waveCounterText.text = $"{currentWave}/{totalWaves}";

            if (punchRoutine != null)
            {
                StopCoroutine(punchRoutine);
            }
            punchRoutine = StartCoroutine(AnimateCounterPunch());
        }
    }

    private void HandleWaveCleared()
    {
        if (startWaveButton != null)
        {
            startWaveButton.gameObject.SetActive(true);
        }
    }

    private void HandleAllWavesCleared()
    {
        if (winUI != null)
        {
            winUI.SetActive(true);
        }
    }

    private IEnumerator AnimateCounterPunch()
    {
        if (waveCounterText == null) yield break;

        Transform target = waveCounterText.transform;
        Vector3 peakScale = baseTextScale * punchScaleMultiplier;
        float halfDuration = punchDuration * 0.5f;
        float elapsed = 0f;

        while (elapsed < halfDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / halfDuration);
            target.localScale = Vector3.Lerp(baseTextScale, peakScale, EaseOutQuad(t));
            yield return null;
        }

        elapsed = 0f;
        while (elapsed < halfDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / halfDuration);
            target.localScale = Vector3.Lerp(peakScale, baseTextScale, EaseOutQuad(t));
            yield return null;
        }

        target.localScale = baseTextScale;
        punchRoutine = null;
    }

    private float EaseOutQuad(float t)
    {
        return 1f - (1f - t) * (1f - t);
    }
}

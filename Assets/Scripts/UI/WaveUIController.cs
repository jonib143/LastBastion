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
        if (winUI != null) winUI.SetActive(false);

        if (waveCounterText != null && waveSpawner != null)
        {
            waveCounterText.text = $"Wave 0 / {waveSpawner.TotalWaves}";
        }
    }

    private void OnStartWaveClicked()
    {
        if (startWaveButton != null)
        {
            startWaveButton.gameObject.SetActive(false);
        }

        waveSpawner.StartNextWave();
    }

    private void HandleWaveStarted(int currentWave, int totalWaves)
    {
        if (waveCounterText != null)
        {
            waveCounterText.text = $"Wave {currentWave} / {totalWaves}";
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
}
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class SettingsUIController : MonoBehaviour
{
    [Header("ui panel")]
    [SerializeField] private GameObject settingsPanel;

    [Header("buttons")]
    [SerializeField] private Button openSettingsButton;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button quitButton;

    private bool isOpen = false;

    void OnEnable()
    {
        if (openSettingsButton != null)
        {
            openSettingsButton.onClick.AddListener(ToggleSettings);
        }

        if (resumeButton != null)
        {
            resumeButton.onClick.AddListener(Resume);
        }

        if (restartButton != null)
        {
            restartButton.onClick.AddListener(Restart);
        }

        if (quitButton != null)
        {
            quitButton.onClick.AddListener(Quit);
        }
    }

    void OnDisable()
    {
        if (openSettingsButton != null)
        {
            openSettingsButton.onClick.RemoveListener(ToggleSettings);
        }

        if (resumeButton != null)
        {
            resumeButton.onClick.RemoveListener(Resume);
        }

        if (restartButton != null)
        {
            restartButton.onClick.RemoveListener(Restart);
        }

        if (quitButton != null)
        {
            quitButton.onClick.RemoveListener(Quit);
        }
    }

    void Start()
    {
        SetPanelState(false);
    }

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            ToggleSettings();
        }
    }

    public void ToggleSettings()
    {
        SetPanelState(!isOpen);
    }

    public void Resume()
    {
        SetPanelState(false);
    }

    public void Restart()
    {
        // reset time scale before reloading so the new scene isn't frozen
        Time.timeScale = 1f;
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.buildIndex);
    }

    public void Quit()
    {
        Time.timeScale = 1f;

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void SetPanelState(bool open)
    {
        isOpen = open;

        if (settingsPanel != null)
        {
            settingsPanel.SetActive(isOpen);
        }

        Time.timeScale = isOpen ? 0f : 1f;
    }

    void OnDestroy()
    {
        if (isOpen)
        {
            Time.timeScale = 1f;
        }
    }
}

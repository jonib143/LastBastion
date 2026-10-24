using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class StartScreenUIController : MonoBehaviour
{
    [Header("scene config")]
    [SerializeField] private string gameplaySceneName = "SampleScene";

    [Header("ui elements")]
    [SerializeField] private Image startScreenImage;
    [SerializeField] private Sprite startScreenSprite;

    private bool isLoading = false;

    void Awake()
    {
        Time.timeScale = 1f;

        if (startScreenImage != null && startScreenSprite != null)
        {
            startScreenImage.sprite = startScreenSprite;
        }
    }

    void Update()
    {
        if (isLoading) return;

        bool clicked = Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame;
        bool touched = Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame;

        if (clicked || touched)
        {
            LoadGameplayScene();
        }
    }

    public void LoadGameplayScene()
    {
        if (isLoading) return;
        isLoading = true;

        if (!string.IsNullOrEmpty(gameplaySceneName))
        {
            SceneManager.LoadScene(gameplaySceneName);
        }
        else
        {
            // fallback to next scene in build settings if name is left blank
            int nextIndex = SceneManager.GetActiveScene().buildIndex + 1;
            SceneManager.LoadScene(nextIndex);
        }
    }
}

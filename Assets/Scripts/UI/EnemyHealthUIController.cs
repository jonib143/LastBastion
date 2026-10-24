using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthUIController : MonoBehaviour
{
    [Header("references")]
    [SerializeField] private EnemyHealth enemyHealth;
    [SerializeField] private Transform barRoot;

    [Header("ui elements")]
    [SerializeField] private Image fillImage;
    [SerializeField] private Slider healthSlider;

    [Header("positioning")]
    [SerializeField] private Vector3 worldOffset = new Vector3(0f, 0.6f, 0f);

    void Awake()
    {
        if (enemyHealth == null)
        {
            enemyHealth = GetComponentInParent<EnemyHealth>();
        }

        if (barRoot == null)
        {
            barRoot = transform;
        }
    }

    void OnEnable()
    {
        if (enemyHealth != null)
        {
            enemyHealth.OnHealthChanged += UpdateHealthBar;

            if (enemyHealth.MaxHp > 0f)
            {
                UpdateHealthBar(enemyHealth.CurrentHp, enemyHealth.MaxHp);
            }
        }
    }

    void OnDisable()
    {
        if (enemyHealth != null)
        {
            enemyHealth.OnHealthChanged -= UpdateHealthBar;
        }
    }

    void LateUpdate()
    {
        if (enemyHealth == null || barRoot == null) return;

        // lock rotation and world offset so the bar stays upright above the rotating enemy
        barRoot.rotation = Quaternion.identity;
        barRoot.position = enemyHealth.transform.position + worldOffset;
    }

    private void UpdateHealthBar(float currentHp, float maxHp)
    {
        float ratio = maxHp > 0f ? Mathf.Clamp01(currentHp / maxHp) : 0f;

        if (fillImage != null)
        {
            fillImage.fillAmount = ratio;
        }

        if (healthSlider != null)
        {
            healthSlider.value = ratio;
        }
    }
}

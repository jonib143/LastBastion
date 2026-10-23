using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class TowerPlacementVisualizer : MonoBehaviour
{
    [Header("references")]
    [SerializeField] private TowerPlacementController placementController;
    [SerializeField] private SpriteRenderer ghostSpriteRenderer;

    [Header("visual settings")]
    [SerializeField] private int rangeSegments = 36;
    [SerializeField] private Color validColor = new Color(0f, 1f, 0f, 0.5f);
    [SerializeField] private Color invalidColor = new Color(1f, 0f, 0f, 0.5f);

    private LineRenderer lineRenderer;

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.useWorldSpace = true;
        lineRenderer.loop = true;
        HidePreview();
    }

    void OnEnable()
    {
        if (placementController != null)
        {
            placementController.OnCardPreviewUpdated += HandleCardPreview;
            placementController.OnSellPreviewUpdated += HandleSellPreview;
            placementController.OnPreviewHidden += HidePreview;
        }
    }

    void OnDisable()
    {
        if (placementController != null)
        {
            placementController.OnCardPreviewUpdated -= HandleCardPreview;
            placementController.OnSellPreviewUpdated -= HandleSellPreview;
            placementController.OnPreviewHidden -= HidePreview;
        }
    }

    private void HandleCardPreview(Vector3 pos, bool isValid, float range, Sprite sprite)
    {
        if (ghostSpriteRenderer != null)
        {
            ghostSpriteRenderer.gameObject.SetActive(true);
            ghostSpriteRenderer.transform.position = pos;
            ghostSpriteRenderer.sprite = sprite;
            ghostSpriteRenderer.color = isValid ? validColor : invalidColor;
        }

        lineRenderer.enabled = true;
        lineRenderer.startColor = isValid ? validColor : invalidColor;
        lineRenderer.endColor = isValid ? validColor : invalidColor;
        DrawCircle(pos, range);
    }

    private void HandleSellPreview(Vector3 pos, bool isValid)
    {
        if (ghostSpriteRenderer != null)
        {
            ghostSpriteRenderer.gameObject.SetActive(false);
        }

        lineRenderer.enabled = true;
        lineRenderer.startColor = isValid ? validColor : invalidColor;
        lineRenderer.endColor = isValid ? validColor : invalidColor;
        DrawCircle(pos, 0.8f);
    }

    private void HidePreview()
    {
        if (ghostSpriteRenderer != null)
        {
            ghostSpriteRenderer.gameObject.SetActive(false);
        }

        if (lineRenderer != null)
        {
            lineRenderer.enabled = false;
        }
    }

    private void DrawCircle(Vector3 center, float radius)
    {
        lineRenderer.positionCount = rangeSegments;
        float step = 360f / rangeSegments;

        for (int i = 0; i < rangeSegments; i++)
        {
            float rad = (i * step) * Mathf.Deg2Rad;
            Vector3 offset = new Vector3(Mathf.Cos(rad) * radius, Mathf.Sin(rad) * radius, 0f);
            lineRenderer.SetPosition(i, center + offset);
        }
    }
}
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class TowerRangeVisualizer : MonoBehaviour
{
    [SerializeField] private TowerController towerController;
    [SerializeField] private int segments = 36;

    private LineRenderer lineRenderer;

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.useWorldSpace = true;
        lineRenderer.loop = true;
    }

    void Start()
    {
        DrawRangeRing();
    }

    public void DrawRangeRing()
    {
        if (towerController == null || towerController.Data == null) return;

        float range = towerController.Data.range;
        lineRenderer.positionCount = segments;

        for (int i = 0; i < segments; i++)
        {
            float rad = (i * (360f / segments)) * Mathf.Deg2Rad;
            Vector3 offset = new Vector3(Mathf.Cos(rad) * range, Mathf.Sin(rad) * range, 0f);
            lineRenderer.SetPosition(i, transform.position + offset);
        }
    }

    public void SetVisible(bool visible)
    {
        if (lineRenderer != null)
        {
            lineRenderer.enabled = visible;
        }
    }
}
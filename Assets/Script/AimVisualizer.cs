using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(LineRenderer))]
public class AimVisualizer : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private Transform leftCap, rightCap, arrowHead;
    [SerializeField] private Tilemap grid;
    [SerializeField] private float arrowOffset = 0.2f;

    public float minAngle = 30f;
    public float maxAngle = 120f;
    public float maxDistance = 5f;
    public int segments = 20;

    private LineRenderer lineRenderer;
    private float currentConeAngle;


    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = segments + 1;
        lineRenderer.useWorldSpace = false;
    }

    void Update()
    {

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;

        Vector3 dir = mousePos - player.position;
        float distance = dir.magnitude;

        Vector3Int mouseCell = grid.WorldToCell(mousePos);
        Vector3Int playerCell = grid.WorldToCell(player.position);

        bool mouseInGrid = grid.HasTile(mouseCell);
        bool tooClose = Mathf.Abs(playerCell.x - mouseCell.x) <= 1 && Mathf.Abs(playerCell.y - mouseCell.y) <= 1;

        if (!mouseInGrid || tooClose)
        {
            SetVisible(false);
            return;
        }

        SetVisible(true);

        // Cập nhật hướng và vị trí
        Vector3 direction = dir.normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.position = player.position;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);

        // Góc mở cung
        float t = Mathf.Clamp01(distance / maxDistance);
        float coneAngle = Mathf.Lerp(minAngle, maxAngle, t);
        DrawArc(-coneAngle / 2f, coneAngle / 2f, distance);
        currentConeAngle = Mathf.Lerp(minAngle, maxAngle, t); // thêm dòng này
        DrawArc(-currentConeAngle / 2f, currentConeAngle / 2f, distance);
    }

    void SetVisible(bool visible)
    {
        lineRenderer.enabled = visible;
        if (leftCap) leftCap.gameObject.SetActive(visible);
        if (rightCap) rightCap.gameObject.SetActive(visible);
        if (arrowHead) arrowHead.gameObject.SetActive(visible);
    }

    void DrawArc(float startAngle, float endAngle, float radius)
    {
        Vector3[] points = new Vector3[segments + 1];

        for (int i = 0; i <= segments; i++)
        {
            float angle = Mathf.Lerp(startAngle, endAngle, (float)i / segments) * Mathf.Deg2Rad;
            points[i] = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f) * radius;
            lineRenderer.SetPosition(i, points[i]);
        }

        if (segments > 1)
        {
            UpdateCap(leftCap, points[0], points[1]);
            UpdateCap(rightCap, points[segments], points[segments - 1]);
            UpdateArrow(points);
        }
    }

    void UpdateCap(Transform cap, Vector3 from, Vector3 to)
    {
        if (!cap) return;
        Vector3 tangent = (to - from).normalized;
        Vector3 normal = new Vector3(-tangent.y, tangent.x, 0f);
        cap.localPosition = from;
        cap.localRotation = Quaternion.FromToRotation(Vector3.right, normal);
    }

    void UpdateArrow(Vector3[] points)
    {
        if (!arrowHead) return;

        int mid = segments / 2;
        Vector3 prev = points[mid - 1], curr = points[mid], next = points[mid + 1];
        Vector3 tangent = (next - prev).normalized;
        Vector3 normal = -new Vector3(-tangent.y, tangent.x, 0f); // đảo chiều

        arrowHead.localPosition = curr + normal * arrowOffset;
        arrowHead.localRotation = Quaternion.FromToRotation(Vector3.up, normal);
    }
    public float GetCurrentConeAngle()
    {
        return currentConeAngle;
    }
    public bool IsVisible()
    {
        return lineRenderer != null && lineRenderer.enabled;
    }
}

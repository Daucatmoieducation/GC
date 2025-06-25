using UnityEngine;
using UnityEngine.Tilemaps;

public class ArrowHighlighterByRotation : MonoBehaviour
{
    public Tilemap tilemap;
    public GameObject arrowPrefab;

    private GameObject arrowInstance;

    // 8 hướng gần kề
    private Vector3Int[] directions = new Vector3Int[]
    {
        new Vector3Int(0, 1, 0),    // ↑
        new Vector3Int(1, 1, 0),    // ↗
        new Vector3Int(1, 0, 0),    // →
        new Vector3Int(1, -1, 0),   // ↘
        new Vector3Int(0, -1, 0),   // ↓
        new Vector3Int(-1, -1, 0),  // ↙
        new Vector3Int(-1, 0, 0),   // ←
        new Vector3Int(-1, 1, 0),   // ↖
    };

    void Start()
    {
        arrowInstance = Instantiate(arrowPrefab);
        arrowInstance.SetActive(false);
    }

    void Update()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0;

        Vector3Int mouseCell = tilemap.WorldToCell(mouseWorldPos);
        Vector3Int centerCell = tilemap.WorldToCell(transform.position);
        Vector3Int offset = mouseCell - centerCell;

        if (IsValidOffset(offset) && tilemap.HasTile(mouseCell))
        {
            arrowInstance.SetActive(true);
            arrowInstance.transform.position = tilemap.GetCellCenterWorld(mouseCell);

            // Tính góc xoay dựa trên hướng offset
            Vector2 dir = new Vector2(offset.x, offset.y).normalized;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

            // Nếu sprite mặc định là hướng lên ↑ thì trừ 90°
            arrowInstance.transform.rotation = Quaternion.Euler(0, 0, angle - 90);
        }
        else
        {
            arrowInstance.SetActive(false);
        }
    }

    bool IsValidOffset(Vector3Int offset)
    {
        foreach (var dir in directions)
        {
            if (offset == dir)
                return true;
        }
        return false;
    }
}

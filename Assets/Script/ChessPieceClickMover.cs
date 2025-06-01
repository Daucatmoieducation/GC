using UnityEngine;
using UnityEngine.Tilemaps;

public class ChessPieceClickMover : MonoBehaviour
{
    public Tilemap tilemap; // Gán Tilemap chứa bàn cờ chính
    public Camera mainCamera;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 worldClickPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int targetCell = tilemap.WorldToCell(worldClickPos);
            Vector3Int currentCell = tilemap.WorldToCell(transform.position);

            Debug.Log($"Clicked cell: {targetCell}, Current cell: {currentCell}");

            // ✅ Kiểm tra nếu có tile
            if (tilemap.HasTile(targetCell))
            {
                int dx = Mathf.Abs(targetCell.x - currentCell.x);
                int dy = Mathf.Abs(targetCell.y - currentCell.y);

                // ✅ Chỉ cho di chuyển nếu là ô xung quanh (kể cả chéo), khác vị trí hiện tại
                if ((dx <= 1 && dy <= 1) && !(dx == 0 && dy == 0))
                {
                    transform.position = tilemap.GetCellCenterWorld(targetCell);
                }
            }
        }
    }
}

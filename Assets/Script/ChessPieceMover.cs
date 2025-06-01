using UnityEngine;
using UnityEngine.Tilemaps;

public class ChessPieceMover : MonoBehaviour
{
    public float tileSize = 1f; // Kích thước mỗi ô (ví dụ 1)
    public Tilemap tilemap;     // Gán Tilemap bàn cờ trong Inspector

    void Update()
    {
        Vector3 move = Vector3.zero;

        if (Input.GetKeyDown(KeyCode.UpArrow))
            move = new Vector3(0, tileSize, 0);
        else if (Input.GetKeyDown(KeyCode.DownArrow))
            move = new Vector3(0, -tileSize, 0);
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
            move = new Vector3(-tileSize, 0, 0);
        else if (Input.GetKeyDown(KeyCode.RightArrow))
            move = new Vector3(tileSize, 0, 0);

        if (move != Vector3.zero)
        {
            Vector3 nextPosition = transform.position + move;
            Vector3Int nextCell = tilemap.WorldToCell(nextPosition);

            // ✅ Chỉ di chuyển nếu ô tiếp theo có tile (hợp lệ)
            if (tilemap.HasTile(nextCell))
            {
                transform.position = tilemap.GetCellCenterWorld(nextCell);
                Debug.Log("Moved to: " + nextCell);
            }
        }
    }
}
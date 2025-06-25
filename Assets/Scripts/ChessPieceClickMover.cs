using UnityEngine;
using UnityEngine.Tilemaps;

public class ChessPieceClickMover : MonoBehaviour
{
    public Tilemap tilemap;
    public Camera mainCamera;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 worldClickPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int targetCell = tilemap.WorldToCell(worldClickPos);
            Vector3Int currentCell = tilemap.WorldToCell(transform.position);

            if (tilemap.HasTile(targetCell))
            {
                int dx = Mathf.Abs(targetCell.x - currentCell.x);
                int dy = Mathf.Abs(targetCell.y - currentCell.y);

                if ((dx <= 1 && dy <= 1) && !(dx == 0 && dy == 0))
                {
                    TurnManager.Instance?.AddTurn();
                    transform.position = tilemap.GetCellCenterWorld(targetCell);

                    Gun gun = FindObjectOfType<Gun>();
                    if (gun != null)
                    {
                        gun.HandleReloadOnMove();
                    }
                }

            }
        }
    }
}

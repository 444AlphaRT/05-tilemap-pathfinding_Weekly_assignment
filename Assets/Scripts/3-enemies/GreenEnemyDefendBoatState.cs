using UnityEngine;
using UnityEngine.Tilemaps;

public class GreenEnemyDefendBoatState : MonoBehaviour
{
    public Transform player;
    public Transform boatCenter;
    public float moveSpeed = 2f;
    public float stopDistance = 0.1f;
    [Header("Tilemap + Forbidden Tiles")]
    public Tilemap worldTilemap;
    public TileBase[] forbiddenWaterTiles;   
    void OnEnable()
    {
        Debug.Log("Green Enemy: DEFEND BOAT");
    }
    void Update()
    {
        if (player == null || boatCenter == null)
            return;
        Vector3 fromBoatToPlayer = (player.position - boatCenter.position).normalized;
        Vector3 targetPos = boatCenter.position + fromBoatToPlayer * 1.0f;
        Vector3 current = transform.position;
        Vector3 dir = (targetPos - current);
        if (dir.magnitude <= stopDistance)
            return;
        dir = dir.normalized;
        float step = moveSpeed * Time.deltaTime;
        Vector3 nextPos = current + dir * step;
        if (IsForbiddenTile(nextPos))
        {
            Debug.Log("Green enemy blocked by WATER – staying on land");
            return;
        }
        transform.position = nextPos;
    }

    private bool IsForbiddenTile(Vector3 worldPosition)
    {
        if (worldTilemap == null || forbiddenWaterTiles == null || forbiddenWaterTiles.Length == 0)
            return false;

        Vector3Int cell = worldTilemap.WorldToCell(worldPosition);
        TileBase tile = worldTilemap.GetTile(cell);

        if (tile == null)
            return false;
        foreach (TileBase t in forbiddenWaterTiles)
        {
            if (t == tile)
                return true;
        }
        return false;
    }
}

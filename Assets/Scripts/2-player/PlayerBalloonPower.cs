using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerBalloonPower : MonoBehaviour
{
    [Header("Tilemap + Tiles")]
    public Tilemap worldTilemap;    
    public TileBase balloonTile;  
    public TileBase replaceWithTile;

    [Header("State")]
    [SerializeField] private bool hasBalloon = false;

    public bool HasBalloon => hasBalloon;

    public void ClearBalloon()
    {
        if (hasBalloon)
        {
            hasBalloon = false;
            Debug.Log("Balloon power cleared – cannot pass mountains anymore.");
        }
    }

    void Update()
    {
        Vector3Int cellPos = worldTilemap.WorldToCell(transform.position);
        TileBase currentTile = worldTilemap.GetTile(cellPos);

        if (!hasBalloon && currentTile == balloonTile)
        {
            hasBalloon = true;
            Debug.Log("Picked up BALLOON – can now walk over mountains!");
            if (replaceWithTile != null)
                worldTilemap.SetTile(cellPos, replaceWithTile);
            else
                worldTilemap.SetTile(cellPos, null);  
        }
    }
}

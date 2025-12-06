using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

/**
 * This component allows the player to move by clicking the arrow keys,
 * but only if the new position is on an allowed tile.
 */
public class KeyboardMoverByTile : KeyboardMover
{
    [SerializeField] private Tilemap tilemap = null;
    [SerializeField] private AllowedTiles allowedTiles = null;
    [Header("Balloon power")]
    [SerializeField] private PlayerBalloonPower balloonPower; 
    [SerializeField] private TileBase balloonTile;         
    [SerializeField] private TileBase[] mountainTiles;      
    [Header("Boat & Castle")]
    [SerializeField] private TileBase boatTile;       
    [SerializeField] private TileBase boatReplaceTile;   
    [SerializeField] private TileBase[] waterTiles;    
    [SerializeField] private TileBase boatPathTile;      
    [SerializeField] private TileBase castleTile;      
    [SerializeField] private bool isSailing = false;   
    [SerializeField] private bool reachedCastle = false;
    public bool ReachedCastle => reachedCastle;
    private TileBase TileOnPosition(Vector3 worldPosition)
    {
        Vector3Int cellPosition = tilemap.WorldToCell(worldPosition);
        return tilemap.GetTile(cellPosition);
    }

    void Update()
    {
        if (reachedCastle)
            return;

        Vector3 newPosition = NewPosition();
        TileBase tileOnNewPosition = TileOnPosition(newPosition);
        bool baseAllowed = allowedTiles != null && allowedTiles.Contains(tileOnNewPosition);
        bool isAllowed = baseAllowed;
        bool hasBalloon = (balloonPower != null && balloonPower.HasBalloon);
        bool isBalloon = (tileOnNewPosition == balloonTile);
        bool isMountain = (mountainTiles != null &&
                           mountainTiles.Length > 0 &&
                           mountainTiles.Contains(tileOnNewPosition));
        bool isBoat = (tileOnNewPosition == boatTile);
        bool isWater = (waterTiles != null && waterTiles.Length > 0 &&
                         waterTiles.Contains(tileOnNewPosition)) ||
                        (tileOnNewPosition == boatPathTile);
        bool isCastle = (tileOnNewPosition == castleTile);
        if (isBalloon)
        {
            isAllowed = true;
        }
        if (isMountain)
        {
            if (!hasBalloon)
                isAllowed = false;
            else
                isAllowed = true;
        }
        if (isBoat)
        {
            isAllowed = true;
        }
        if (isWater)
        {
            if (isSailing)
                isAllowed = true;
            else
                isAllowed = false;   
        }
        if (isCastle)
        {
            if (isSailing)
                isAllowed = true;
            else
                isAllowed = false;
        }
        if (isAllowed)
        {
            bool hadBalloonBeforeMove = hasBalloon;
            bool wasSailingBeforeMove = isSailing;
            transform.position = newPosition;
            TileBase currentTile = TileOnPosition(transform.position);
            bool nowOnMountain = mountainTiles != null &&
                                 mountainTiles.Length > 0 &&
                                 mountainTiles.Contains(currentTile);
            bool nowOnBalloon = (currentTile == balloonTile);
            bool nowOnBoat = (currentTile == boatTile);
            bool nowOnWater = (waterTiles != null && waterTiles.Length > 0 &&
                                 waterTiles.Contains(currentTile)) ||
                                (currentTile == boatPathTile);
            bool nowOnCastle = (currentTile == castleTile);

            if (hadBalloonBeforeMove && !nowOnMountain && !nowOnBalloon && balloonPower != null)
            {
                balloonPower.ClearBalloon();
            }
            if (!wasSailingBeforeMove && nowOnBoat)
            {
                isSailing = true;
                Debug.Log("Boarded the BOAT – you can now sail on water!");

                if (boatReplaceTile != null)
                {
                    Vector3Int cellPos = tilemap.WorldToCell(transform.position);
                    tilemap.SetTile(cellPos, boatReplaceTile);
                }
            }
            if (isSailing && nowOnWater && boatPathTile != null)
            {
                Vector3Int cellPos = tilemap.WorldToCell(transform.position);
                tilemap.SetTile(cellPos, boatPathTile);
            }
            if (!reachedCastle && nowOnCastle)
            {
                reachedCastle = true;
                Debug.Log("You reached the CASTLE! Goal completed 🎉");
                this.enabled = false;
            }
        }
        else
        {
            Debug.LogWarning("You cannot walk on " + tileOnNewPosition + "!");
        }
    }
}

using System.Collections;
using System.Linq; 
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;


/**
 * This class demonstrates the CaveGenerator on a Tilemap.
 * 
 * By: Erel Segal-Halevi
 * Since: 2020-12
 */

public class TilemapCaveGenerator: MonoBehaviour {
    [SerializeField] Tilemap tilemap = null;

    [Tooltip("The tile that represents a wall (an impassable block)")]
    [SerializeField] TileBase wallTile = null;

    [Tooltip("The tile that represents a floor (a passable block)")]
    [SerializeField] TileBase floorTile = null;

    [Tooltip("The percent of walls in the initial random map")]
    [Range(0, 1)]
    [SerializeField] float randomFillPercent = 0.5f;

    [Tooltip("Length and height of the grid")]
    [SerializeField] int gridSize = 100;

    [Tooltip("How many steps do we want to simulate?")]
    [SerializeField] int simulationSteps = 20;

    [Tooltip("For how long will we pause between each simulation step so we can look at the result?")]
    [SerializeField] float pauseTime = 1f;

    [Tooltip("The Transform component of the Player/object to place")]
    [SerializeField] Transform playerTransform = null;

    [Tooltip("The minimum required reachable tiles for a valid spawn point")]
    [SerializeField] int minReachableTiles = 100;

    // כדי להימנע מחיפוש חוזר של AllowedTiles ו-TilemapGraph
    private TilemapGraph tilemapGraph = null;
    private TileBase[] allowedTiles = null;

    private CaveGenerator caveGenerator;

    void Start()  {
        //To get the same random numbers each time we run the script
        Random.InitState(100);

        // מניחים ש-AllowedTiles נמצא במקום אחר ומכיל את האריחים המותרים (floorTile)
        this.allowedTiles = FindObjectOfType<AllowedTiles>()?.Get();
        if (this.allowedTiles == null || this.allowedTiles.Length == 0)
        {
            this.allowedTiles = new TileBase[] { floorTile }; // Fallback
        }
        this.tilemapGraph = new TilemapGraph(tilemap, this.allowedTiles);

        caveGenerator = new CaveGenerator(randomFillPercent, gridSize);
        caveGenerator.RandomizeMap();

        //For testing that init is working
        ShowPatternOnTileMap(caveGenerator.GetMap());

        //Start the simulation
        SimulateCavePattern();
    }


    //Do the simulation in an async function, so we can pause and see what's going on
    async void SimulateCavePattern()  {
        for (int i = 0; i < simulationSteps; i++)   {
            await Awaitable.WaitForSecondsAsync(pauseTime);

            //Calculate the new values
            caveGenerator.SmoothMap();

            //Generate texture and display it on the plane
            ShowPatternOnTileMap(caveGenerator.GetMap());
        }
        Debug.Log("Simulation completed!");
        ValidateAndPlacePlayer();
    }


    //Generate a black or white texture depending on if the pixel is cave or wall
    //Display the texture on a plane
    private void ShowPatternOnTileMap(int[,] data) {
        for (int y = 0; y < gridSize; y++) {
            for (int x = 0; x < gridSize; x++) {
                var position = new Vector3Int(x, y, 0);
                var tile = data[x, y] == 1 ? wallTile: floorTile;
                tilemap.SetTile(position, tile);
            }
        }
    }

    /**
     * Finds a valid spawn position on the map and places the player there.
     * A valid position is one that connects to at least minReachableTiles (default 100)
     */
    private void ValidateAndPlacePlayer()
    {
        if (playerTransform == null)
        {
            Debug.LogError("Player Transform is not assigned in the inspector! Cannot validate spawn.");
            return;
        }

        // 1. קבלת כל האריחים המותרים למיקום שחקן
        List<Vector3Int> floorTiles = new List<Vector3Int>();
        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                Vector3Int pos = new Vector3Int(x, y, 0);
                if (allowedTiles.Contains(tilemap.GetTile(pos)))
                {
                    floorTiles.Add(pos);
                }
            }
        }

        if (floorTiles.Count == 0)
        {
            Debug.LogError("No passable tiles found on the map! Cannot place player.");
            return;
        }

        // 2. לולאה לבדיקת נקודות אקראיות עד שנמצא אחת תקפה
        int maxAttempts = floorTiles.Count * 2; // נותנים מספיק ניסיונות

        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            // בחירת נקודת רצפה אקראית
            int randomIndex = Random.Range(0, floorTiles.Count);
            Vector3Int candidatePos = floorTiles[randomIndex];

            // ספירת אריחים נגישים באמצעות BFS
            int reachableCount = BFS.CountReachableNodes(this.tilemapGraph, candidatePos);

            if (reachableCount >= minReachableTiles)
            {
                Vector3Int spawnPosition = candidatePos;
                Debug.Log($"Found valid spawn point at {spawnPosition}. Reachable tiles: {reachableCount} after {attempt + 1} attempts.");

                // מיקום השחקן
                // CellToWorld ממיר קואורדינטות אריח למיקום עולם (בפינה), לכן מוסיפים חצי גודל אריח למרכז.
                playerTransform.position = tilemap.CellToWorld(spawnPosition) + tilemap.cellSize / 2;

                // אם קיים רכיב PlayerSpawn, כדאי לעדכן את נקודת ההתחלה שלו
                // אם את לא מצליחה לעשות זאת דרך קריאה לפונקציה ציבורית, אפשר לוותר על זה כרגע.

                return;
            }
        }

        // Fallback: אם לא נמצאה נקודה תקפה, מציבים באופן אקראי בכל זאת.
        Debug.LogWarning($"Could not find a spawn point with >= {minReachableTiles} reachable tiles after {maxAttempts} attempts. Placing player randomly anyway.");
        playerTransform.position = tilemap.CellToWorld(floorTiles[Random.Range(0, floorTiles.Count)]) + tilemap.cellSize / 2;
    }
}

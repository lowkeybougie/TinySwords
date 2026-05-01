using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class MapPopulator : MonoBehaviour
{
    public Tilemap floorTilemap; 

    void Start()
    {
        PopulateSpawners();
    }

    void PopulateSpawners()
    {
        List<Vector3> availableTiles = new List<Vector3>();

       
        BoundsInt bounds = floorTilemap.cellBounds;

        foreach (var pos in bounds.allPositionsWithin)
        {
            if (floorTilemap.HasTile(pos))
            {
                
                Vector3 worldPos = floorTilemap.CellToWorld(pos) + new Vector3(0.5f, 0.5f, 0);
                availableTiles.Add(worldPos);
            }
        }

        
    }
}

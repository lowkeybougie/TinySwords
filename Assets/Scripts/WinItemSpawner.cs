using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class FilteredWinItemSpawner : MonoBehaviour
{
    public GameObject winItemPrefab;

    [Header("Tilemap Setup")]
    public Tilemap groundTilemap; 
    public List<Tilemap> avoidTilemaps; 

    void Start()
    {
        SpawnWinItem();
    }

    void SpawnWinItem()
    {
        if (groundTilemap == null) return;

        List<Vector3> validPositions = new List<Vector3>();

        
        foreach (var pos in groundTilemap.cellBounds.allPositionsWithin)
        {
            if (groundTilemap.HasTile(pos))
            {
               
                if (!IsPositionOccupied(pos))
                {
                    validPositions.Add(groundTilemap.GetCellCenterWorld(pos));
                }
            }
        }

        
        if (validPositions.Count > 0)
        {
            int randomIndex = Random.Range(0, validPositions.Count);
            Instantiate(winItemPrefab, validPositions[randomIndex], Quaternion.identity);
        }
    }

    
    private bool IsPositionOccupied(Vector3Int cellPos)
    {
        foreach (Tilemap map in avoidTilemaps)
        {
            if (map != null && map.HasTile(cellPos))
            {
                return true; 
            }
        }
        return false; 
    }
}

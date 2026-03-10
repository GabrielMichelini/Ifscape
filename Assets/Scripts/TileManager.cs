using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    public GameObject[] tilePrefabs; 
    public float zSpawn = 0; 
    public float tileLength = 30; 
    public int numberOfTiles = 5; 
    public Transform playerTransform; 

    private List<GameObject> activeTiles = new List<GameObject>();

    void Start()
    {
        for (int i = 0; i < numberOfTiles; i++)
        {
            SpawnTile(0);
        }
    }

    void Update()
    {
        if (playerTransform != null && playerTransform.position.z - 35 > zSpawn - (numberOfTiles * tileLength))
        {
            SpawnTile(0);
            DeleteTile();
        }
    }

    public void SpawnTile(int tileIndex)
    {
        GameObject go = Instantiate(tilePrefabs[tileIndex], transform.forward * zSpawn, transform.rotation);
        activeTiles.Add(go);
        zSpawn += tileLength;
    }

    private void DeleteTile()
    {
        Destroy(activeTiles[0]);
        activeTiles.RemoveAt(0);
    }
}
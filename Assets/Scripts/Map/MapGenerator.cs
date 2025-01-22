using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public int width = 4;
    public int height = 4;

    public string seed;
    public bool useRandomSeed;

    [Range(0,100)]
    public int randomFillPercent;

    private bool[,] worldGrid;

    public List<GameObject> startPrefab;
    public GameObject emptyPrefab;

    // Start is called before the first frame update
    void Start()
    {
        GenerateWorldGrid();
        CheckConnected();
        BuildGrid();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void GenerateWorldGrid()
    {
        
        if(useRandomSeed || seed == null)
        {
            seed = System.DateTime.Today.ToString();
        }
        System.Random rnd = new System.Random(seed.GetHashCode());
        worldGrid = new bool[width, height];
        for(int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                Debug.Log(rnd.GetHashCode().ToString());
                worldGrid[x, y] = true;
            }
        }
    }

    void CheckConnected()
    {
        HashSet<Vector2Int> visited = new HashSet<Vector2Int>();
        Vector2Int start = Vector2Int.zero;

        FloodFill(start, visited);

        //Ensure all rooms are reachable
        for(int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                if(worldGrid[x,y] && !visited.Contains(new Vector2Int(x,y)))
                {
                    ConnectRoomToVisited(new Vector2Int(x, y), visited);
                }
            }
        }
    }

    void FloodFill(Vector2Int position, HashSet<Vector2Int> visited)
    {
        Stack<Vector2Int> stack = new Stack<Vector2Int>();
        stack.Push(position);

        while(stack.Count > 0)
        {
            Vector2Int current = stack.Pop();
            if(visited.Contains(current)) continue;

            visited.Add(current);

            //Check neighbors (up, down, left, right)
            foreach(Vector2Int neighbor in GetNeighbors(current))
            {
                if(worldGrid[neighbor.x, neighbor.y] && !visited.Contains(neighbor))
                {
                    stack.Push(neighbor);
                }
            }
        }
    }

    void ConnectRoomToVisited(Vector2Int position, HashSet<Vector2Int> visited)
    {
        //Find a visited neighbor
        foreach (Vector2Int neighbor in GetNeighbors(position))
        {
            if(visited.Contains(neighbor))
            {
                worldGrid[position.x, position.y] = true;
                visited.Add(position);
                return;
            }
        }
    }

    List<Vector2Int> GetNeighbors(Vector2Int position)
    {
        List<Vector2Int> neighbors = new List<Vector2Int>();

        Vector2Int[] directions = {
            Vector2Int.up,
            Vector2Int.down,
            Vector2Int.left,
            Vector2Int.right
        };

        foreach(Vector2Int direction in directions)
        {
            Vector2Int neighbor = position + direction;
            if(neighbor.x >=0 && neighbor.x < width && neighbor.y >= 0 && neighbor.y < height)
            {
                neighbors.Add(neighbor);
            }
        }

        return neighbors;
    }

    void BuildGrid()
    {
        for(int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                Vector2 worldPosition = new Vector2(x * 80, y * 80);
                if(worldPosition == Vector2.zero)
                {
                    Instantiate(startPrefab[0], worldPosition, quaternion.identity);
                }
                else if(worldGrid[x,y])
                {
                    Instantiate(startPrefab[1], worldPosition, quaternion.identity);
                }
                else
                {
                    Instantiate(emptyPrefab, worldPosition, quaternion.identity);
                }
            }
        }
    }
}

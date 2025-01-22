using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public int width;
    public int height;

    public string seed;
    public bool useRandomSeed;

    [Range(0,100)]
    public int randomFillPercent;

    int[,] map;

    // Start is called before the first frame update
    void Start()
    {
        GenerateMap();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void GenerateMap()
    {
        map = new int[width, height];
    }

    void RandomFillMap()
    {
        if(useRandomSeed)
        {
            seed = Time.time.ToString();
        }

        System.Random rand = new System.Random(seed.GetHashCode());

        for(int x = 0; x < width; x++)
        {
            for(int y = 0; x < height; y++)
            {
                map[x,y] = (rand.Next(0,100) < randomFillPercent) ? 1 : 0;
            }
        }
    } 

    void OnDrawGizmos()
    {
        if (map != null)
        {
            for(int x= 0; x < width; x++) 
            {
                for(int y = 0; y < height; y++)
                {
                    Gizmos.color = (map[x,y] == 1) ? Color.black : Color.white;
                    Vector3 pos = new Vector3(-width/2 + x + .5f, -height/2 + y+.5f, 0);
                    Gizmos.DrawCube(pos, Vector3.one);
                }
            }
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using Unity.Services.Analytics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class MapGenerator : MonoBehaviour
{
    public int width = 20;
    public int height = 20;

    public string seed;
    public bool useRandomSeed;

    [Range(0,1)]
    public float randomFillPercent;

    public int roomWidth = 40;
    public int roomHeight = 20;

    public List<GameObject> roomPrefabList;
    public List<GameObject> endRoomPrefabList;
    public GameObject emptyPrefab;

    public int roomCount = 40;
    private Dictionary<Vector2Int, bool> dungeonMap = new Dictionary<Vector2Int, bool>();
    public Transform dungeonParent;

    // Start is called before the first frame update
    void Start()
    {
        dungeonParent = GameObject.Find("World").transform;
        GenerateDungeon();
        EmptyRooms();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ResetMap()
    {
        Transform ShadowParent = GameObject.Find("Shadows").transform;
        Transform CasingParent = GameObject.Find("Casings").transform;
        Transform HealthBarParent = GameObject.Find("HealthBars").transform;

        foreach(Transform child in CasingParent)
        {
            Destroy(child.gameObject);
        }
        foreach(Transform child in ShadowParent)
        {
            if(child.gameObject.name != "Shadow_Gun")
                Destroy(child.gameObject);
        }
        
        foreach(Transform child in dungeonParent)
        {                
            Destroy(child.gameObject);
        }
        GenerateDungeon();
        EmptyRooms();
        FindAnyObjectByType<PlayerController>().Reset();
    }

    void GenerateDungeon()
    {
        dungeonMap.Clear();
        Vector2Int startRoom = Vector2Int.zero;
        dungeonMap[startRoom] = true;

        // List for room place for connectivity
        List<Vector2Int> roomList = new List<Vector2Int>();
        roomList.Add(startRoom);

        //Directions for room placement
        Vector2Int[] directions = {Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right};

        //Generate Rooms
        int roomsGenerated = 1;
        while(roomsGenerated < roomCount && roomList.Count > 0)
        {
            //Randomly pick a room from the queue to expand
            int randomIndex = Random.Range(0, roomList.Count);
            Vector2Int currentRoom = roomList[randomIndex];

            //Randomize the number of directions to explore (1 to all directions)
            int directionsToExplore = Random.Range(1, directions.Length + 1);

            //Shuffle directions to randomize generation
            directions = ShuffleArray(directions);

            for(int x = 0; x < directionsToExplore; x++)
            {
                Vector2Int newRoom = currentRoom + directions[x];

                //Check if the room is within bounds and hasn't been placed yet
                if(!dungeonMap.ContainsKey(newRoom) && Mathf.Abs(newRoom.x) < width / 2 &&
                    Mathf.Abs(newRoom.y) < height / 2)
                {
                    //Place the room
                    dungeonMap[newRoom] = true;
                    roomList.Add(newRoom);
                    roomsGenerated++;

                    //Break if we reach the desired room count
                    if(roomsGenerated >= roomCount)
                        break;
                }
                
            }

            //Remove the current room from the queue if it has no more valid neighbors
            if(roomList.Count > 1 && Random.value > 0.5f)
                roomList.RemoveAt(randomIndex); // Randomly remove rooms to increase random
        }

        //Instantiate rooms in the scene
        InstantiateDungeon();
    }

    void InstantiateDungeon()
    {
        foreach(KeyValuePair<Vector2Int, bool> entry in dungeonMap)
        {
            int rnd = Random.Range(3, 5);
            Vector3 roomPosition = new Vector3(entry.Key.x * roomWidth, entry.Key.y * roomHeight, 0);
            GameObject roomPrefabToUse = entry.Key == Vector2Int.zero ? roomPrefabList[0] : roomPrefabList[rnd];
            Instantiate(roomPrefabToUse, roomPosition, quaternion.identity, dungeonParent);
        }
    }

    void EmptyRooms()
    {
        Vector2Int[] directions = {Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right, 
            new Vector2Int(-1, 1), new Vector2Int(1, 1), 
            new Vector2Int(-1, -1), new Vector2Int(1, -1)};

        List<Vector2Int> occupiedPositions = new List<Vector2Int>(dungeonMap.Keys);

        foreach(Vector2Int roomPosition in occupiedPositions)
        {
            foreach(Vector2Int direction in directions)
            {
                Vector2Int adjacentPosition = roomPosition + direction;

                if(!dungeonMap.ContainsKey(adjacentPosition))
                {
                    dungeonMap[adjacentPosition] = false;

                    Vector3 position = new Vector3(adjacentPosition.x * roomWidth, adjacentPosition.y * roomHeight, 0);
                    Instantiate(emptyPrefab, position, quaternion.identity, dungeonParent);
                }
            }
        }
    }

    Vector2Int[] ShuffleArray(Vector2Int[] array)
    {
        for(int x = 0; x < array.Length; x++)
        {
            int randomIndex = Random.Range(0, array.Length);
            Vector2Int temp = array[x];
            array[x] = array[randomIndex];
            array[randomIndex] = temp;
        }
        return array;
    }
}

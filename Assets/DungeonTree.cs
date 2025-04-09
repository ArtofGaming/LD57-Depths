using System;
using System.ComponentModel.Design.Serialization;
using JetBrains.Annotations;
using Mono.Cecil.Cil;
using UnityEditor.Rendering.CustomRenderTexture.ShaderGraph;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using System.Linq;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.Experimental.GraphView;

public class DungeonTree : MonoBehaviour
{
    [SerializeField]
    Tilemap floor;
    [SerializeField]
    TileBase floorTile;
    [SerializeField]
    TileBase altFloorTile;

    [SerializeField]
    GameObject[] enemies;
    Queue<Room> tree = new Queue<Room>(); 
    System.Random rand = new System.Random();

    int trimTiles = 1;

    int minWidth = 3;
    int minHeight = 3;
    Room root;
    Room currentRoom;
    int cutLocation = 0;
    List<Room> roomList = new List<Room>();

    double chance = 0.0;


    void Start()
    {
        BinarySpatialPartition();
    }
    
    public void BinarySpatialPartition()
    {
        
        root = new Room();
        
        currentRoom = root;
        currentRoom.height = 30;
        currentRoom.width = 30;
        currentRoom.x = 0;
        currentRoom.y = 0;
        
        DivideRoom(root);
    }
                /*Debug.Log(currentRoom.leftChild.topLeftCornerPosition + " " + currentRoom.leftChild.bottomRightCornerPosition);
                currentRoom.InsertRightChild();
                currentRoom.rightChild.topLeftCornerPosition = new Vector2(cutLocation.x,currentRoom.topLeftCornerPosition.y);
                currentRoom.rightChild.bottomRightCornerPosition = new Vector2(currentRoom.bottomRightCornerPosition.x ,currentRoom.bottomRightCornerPosition.y);
                currentRoom.rightChild.parent = currentRoom;
                currentRoom.rightChild.width = (int)currentRoom.rightChild.bottomRightCornerPosition.x - (int)currentRoom.rightChild.topLeftCornerPosition.x;
                currentRoom.rightChild.height = (int)currentRoom.rightChild.bottomRightCornerPosition.y - (int)currentRoom.rightChild.topLeftCornerPosition.y;
                Debug.Log(currentRoom.rightChild.topLeftCornerPosition + " " + currentRoom.leftChild.bottomRightCornerPosition);*/

        
        //PaintTiles()
        //FindUndividedRooms();
        //Debug.Log(x);
    public void DivideRoom(Room currentRoom) 
    {

        /*
        if (bigger than min width and min height)
        {
            if (width > height)
            {
                cut location = min width < random int < width - minwidth
                CreateLeftChild(parent,startCorner, width height) -> CreateLeftChild(parent, new Vector2Int(parent.x,parent.y), cutLocation, height)
            }
        }
        */
        if (currentRoom.width / 2 < minWidth && currentRoom.height / 2 < minHeight)
        {
            Trim(currentRoom);
            PaintTiles(currentRoom, floor, floorTile, altFloorTile);
            PaintEnemies(currentRoom, floor, enemies);
        }
        else 
        {
            if (currentRoom.width > minWidth && currentRoom.height > minHeight)
            {
                if (currentRoom.width > currentRoom.height)
                {
                    cutLocation = rand.Next(2, currentRoom.width - 2);
                    currentRoom.InsertLeftChild();
                    CreateLeftChild(currentRoom.leftChild, cutLocation, true);
                    currentRoom.InsertRightChild();
                    CreateRightChild(currentRoom.rightChild, cutLocation, true);
                }
                else
                {
                    cutLocation = rand.Next(2, currentRoom.height - 2);
                    currentRoom.InsertLeftChild();
                    CreateLeftChild(currentRoom.leftChild, cutLocation, false);
                    currentRoom.InsertRightChild();
                    CreateRightChild(currentRoom.rightChild, cutLocation, false);
                }
                DivideRoom(currentRoom.leftChild);
                DivideRoom(currentRoom.rightChild);
            }
        }
        

    }
    public void CreateLeftChild(Room currentRoom, int cutLocation, bool onXAxis)
    {
        if (onXAxis)
        {
            currentRoom.x = currentRoom.parent.x;
            currentRoom.y = currentRoom.parent.y;
            currentRoom.width = cutLocation;
            currentRoom.height = currentRoom.parent.height;
        }
        else 
        {
            currentRoom.x = currentRoom.parent.x;
            currentRoom.y = currentRoom.parent.y;
            currentRoom.width = currentRoom.parent.width;
            currentRoom.height = cutLocation;
        }
        //Trim(currentRoom);
        //Debug.Log(currentRoom.leftChild.topLeftCornerPosition + " " + currentRoom.leftChild.bottomRightCornerPosition);
    }
    public void CreateRightChild(Room currentRoom, int cutLocation, bool onXAxis)
    {
        if (onXAxis) 
        {
            currentRoom.x = currentRoom.parent.x + cutLocation + 1;
            currentRoom.y = currentRoom.parent.y;
            currentRoom.width = currentRoom.parent.width - cutLocation;
            currentRoom.height = currentRoom.parent.height;
        }
        else
        {
            currentRoom.x = currentRoom.parent.x;
            currentRoom.y = currentRoom.parent.y + cutLocation + 1;
            currentRoom.width = currentRoom.parent.width;
            currentRoom.height = currentRoom.parent.height - cutLocation;
        }
        //Trim(currentRoom);
        //Debug.Log(currentRoom.rightChild.topLeftCornerPosition + " " + currentRoom.rightChild.bottomRightCornerPosition);
    }

    public void Trim(Room room)
    {
        room.x += trimTiles;
        room.width -= trimTiles;
        room.y += trimTiles;
        room.height -= trimTiles;
    }

    void PaintTiles(Room room, Tilemap tilemap, TileBase tile, TileBase altTile)
    {
        for (int j = 0; j <= room.height; j++)
        {
            for (int i = 0; i <= room.width; i++)
            {
                var tilePosition = new Vector3Int(room.x + i,room.y + j,0);
                chance = rand.NextDouble();
                if (chance < .25)
                {
                    tilemap.SetTile(tilePosition, altTile);
                }
                else
                {
                    tilemap.SetTile(tilePosition, tile);
                }
                
            }
        }
    }
    void PaintEnemies(Room room, Tilemap tilemap, GameObject[] enemies)
    {
        double adjustment = 0;
        for (int j = 0; j <= room.height; j++)
        {
            for (int i = 0; i <= room.width; i++)
            {
                var tilePosition = new Vector3Int(room.x + i,room.y + j,0);
                chance = rand.NextDouble() + adjustment;
                if (chance < .1)
                {
                    chance = 0;

                     GameObject.Instantiate(enemies[(int)chance], tilePosition, Quaternion.identity);
                     
                    adjustment = rand.NextDouble();
                }
                else
                {
                    adjustment = 0;
                }
                
            }
        }
    }
}

public class Room 
{
    public Vector2Int topLeftCornerPosition = Vector2Int.zero;
    public Vector2Int bottomRightCornerPosition = Vector2Int.zero;
    public Room sibling = null;
    public Room parent = null;
    public Room leftChild = null;
    public Room rightChild = null;
    public bool tooSmall = false;
    public int width = 0;
    public int height = 0;
    public int x = 0;
    public int y = 0;


    public void InsertLeftChild()
    {
        if (leftChild == null)
        {
            leftChild = new Room();
            leftChild.parent = this;
        }
        else 
        {
            Debug.LogError(this + " already has a left child.");
        }
    }
    public void InsertRightChild()
    {
        if (rightChild == null)
        {
            rightChild = new Room();
            rightChild.parent = this;
            rightChild.sibling = leftChild;
            leftChild.sibling = rightChild;
        }
        else 
        {
            Debug.LogError(this + " already has a right child.");
        }
    }

     public List<int> GetRConnections()
    {
        List <int> connections = new List<int>();
        if (rightChild != null && leftChild != null) 
        {
            if (rightChild != null)
            {
                connections.AddRange(rightChild.GetRConnections());
            }
            if (leftChild != null)
            {
                connections.AddRange(leftChild.GetRConnections());
            }
        }
        else 
        {
            for (int l = y + height + 1; l <= y - 1; l++)
            {
                connections.Add(l);
            }
        }
        return connections;
        
    }
}

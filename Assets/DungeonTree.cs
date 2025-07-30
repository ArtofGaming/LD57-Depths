using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using System.Linq;
using UnityEngine.Analytics;

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

    [SerializeField]
    int desiredNumberOfRooms = 8;

    [SerializeField]
    TileBase pathFloorTile;
    Queue<Room> tree = new Queue<Room>();
    System.Random rand = new System.Random();

    int trimTiles = 1;

    int offset = 1;
    int minWidth = 5;
    int minHeight = 5;
    Room root;
    Room currentRoom;
    int cutLocation = 0;
    List<Room> roomList = new List<Room>();
    int endNodes = 0;
    double chance = 0.0;



    void Start()
    {
        BinarySpatialPartition();
    }

    public void BinarySpatialPartition()
    {
        floor.ClearAllTiles();
        tree.Clear();
        roomList.Clear();
        if (GameObject.FindGameObjectsWithTag("Enemy") != null)
        {
            foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
            {
                Destroy(enemy);
            }
        }
        root = new Room();
        roomList.Clear();
        //walkable.Clear();
        currentRoom = root;
        currentRoom.height = 30;
        currentRoom.width = 30;
        currentRoom.bottomRightCornerPosition = new Vector2Int(currentRoom.width, currentRoom.height);
        currentRoom.x = 0;
        currentRoom.y = 0;
        roomList.Add(root);
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
        for (int i = 0; i < 5; i++)
        {
            if (roomList.Count() >= desiredNumberOfRooms)
            {
                break;
            }
            else if (currentRoom.width <= 5)
            {
                break;
            }
            else
            {
                cutLocation = rand.Next(currentRoom.topLeftCornerPosition.x + 2, currentRoom.bottomRightCornerPosition.x - 2);
                currentRoom.InsertLeftChild();
                CreateLeftChild(currentRoom.leftChild, cutLocation, true);
                roomList.Add(currentRoom.leftChild);
                tree.Enqueue(currentRoom.leftChild);
                currentRoom.InsertRightChild();
                CreateRightChild(currentRoom.rightChild, cutLocation, true);
                roomList.Add(currentRoom.rightChild);
                tree.Enqueue(currentRoom.rightChild);
                for (int j = 0; j < Math.Pow(2, i); j++)
                {
                    if (roomList.Count() >= desiredNumberOfRooms)
                    {
                        break;
                    }
                    else
                    {
                        DivideRoom(tree.Dequeue());
                    }
                }
            }
            
        }
        
        PaintTiles(currentRoom, floor, floorTile, altFloorTile);
        /*if (roomList.Count() >= desiredNumberOfRooms)
        {
            PaintTiles(currentRoom, floor, floorTile, altFloorTile);
            //ConnectRooms(floor, pathFloorTile);
        }
        else
        {
            if (currentRoom.width / 2 < minWidth && currentRoom.height / 2 < minHeight)
            {
                roomList.Remove(currentRoom);
                if (currentRoom.width > minWidth && currentRoom.height > minHeight)
                {
                    if (currentRoom.width > currentRoom.height)
                    {
                        cutLocation = rand.Next(2, currentRoom.width - 2);
                        currentRoom.InsertLeftChild();
                        CreateLeftChild(currentRoom.leftChild, cutLocation, true);
                        roomList.Add(currentRoom.leftChild);
                        tree.Enqueue(currentRoom.leftChild);
                        currentRoom.InsertRightChild();
                        CreateRightChild(currentRoom.rightChild, cutLocation, true);
                        roomList.Add(currentRoom.rightChild);
                        tree.Enqueue(currentRoom.rightChild);
                    }
                    else
                    {
                        cutLocation = rand.Next(2, currentRoom.height - 2);
                        currentRoom.InsertLeftChild();
                        CreateLeftChild(currentRoom.leftChild, cutLocation, false);
                        roomList.Add(currentRoom.leftChild);
                        tree.Enqueue(currentRoom.leftChild);
                        currentRoom.InsertRightChild();
                        CreateRightChild(currentRoom.rightChild, cutLocation, false);
                        roomList.Add(currentRoom.rightChild);
                        tree.Enqueue(currentRoom.rightChild);
                    }

                    DivideRoom(currentRoom.leftChild);
                    DivideRoom(currentRoom.rightChild);
                }
            }
        }*/
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
    }

    public void CreateLeftChild(Room currentRoom, int cutLocation, bool onXAxis)
    {
        if (onXAxis)
        {
            currentRoom.x = currentRoom.parent.x;
            currentRoom.y = currentRoom.parent.y;
            currentRoom.width = currentRoom.parent.width - (currentRoom.parent.width - cutLocation);
            currentRoom.topLeftCornerPosition = new Vector2Int(currentRoom.x, currentRoom.y);
            currentRoom.height = currentRoom.parent.height;
            currentRoom.bottomRightCornerPosition = new Vector2Int(currentRoom.x + cutLocation, currentRoom.y + currentRoom.height);
        }
        else
        {
            currentRoom.x = currentRoom.parent.x;
            currentRoom.y = currentRoom.parent.y;
            currentRoom.width = currentRoom.parent.width;
            currentRoom.topLeftCornerPosition = new Vector2Int(currentRoom.x, currentRoom.y);
            currentRoom.height = currentRoom.parent.height - (currentRoom.parent.height - cutLocation);
            currentRoom.bottomRightCornerPosition = new Vector2Int(currentRoom.x + currentRoom.width, currentRoom.y + cutLocation);
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
            currentRoom.width = currentRoom.parent.width - (cutLocation + 1);
            currentRoom.topLeftCornerPosition = new Vector2Int(cutLocation + 1, currentRoom.y);
            currentRoom.height = currentRoom.parent.height;
            currentRoom.bottomRightCornerPosition = new Vector2Int(currentRoom.parent.bottomRightCornerPosition.x, currentRoom.parent.bottomRightCornerPosition.y);
        }
        else
        {
            currentRoom.x = currentRoom.parent.x;
            currentRoom.y = currentRoom.parent.y + cutLocation + 1;
            currentRoom.width = currentRoom.parent.width;
            currentRoom.topLeftCornerPosition = new Vector2Int(currentRoom.x, currentRoom.y + cutLocation + 1);
            currentRoom.height = currentRoom.parent.height - (cutLocation + 1);
            currentRoom.bottomRightCornerPosition = new Vector2Int(currentRoom.parent.bottomRightCornerPosition.x, currentRoom.parent.bottomRightCornerPosition.y);
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
                var tilePosition = new Vector3Int(room.x + i, room.y + j, 0);
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

    void PaintConnection(Room room1, Room room2, int minX, int maxX, Tilemap tilemap, TileBase tile, TileBase altTile)
    {
        for (int j = room1.bottomRightCornerPosition.y; j < room2.topLeftCornerPosition.y; j++)
        {
            for (int i = minX; i <= maxX; i++)
            {
                var tilePosition = new Vector3Int(i,j, 0);
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

    void ConnectRooms(Tilemap tilemap, TileBase tile)
    {
        int minX = 100;
        int maxX = 0;
        int pathBLCornerX = 0;
        int pathBRCornerX = 0;
        Room room1 = tree.Dequeue();
        Room room2 = tree.Peek();
        if (room1.topLeftCornerPosition.y > room2.topLeftCornerPosition.y)
        {
                Room forSwitching = room1;
                room1 = room2;
                room2 = forSwitching;
        }
            if (room1.topLeftCornerPosition.x < room2.topLeftCornerPosition.x)
            {
                minX = room2.topLeftCornerPosition.x;
            }
            else
            {
                minX = room1.topLeftCornerPosition.x;
            }

            if (room1.bottomRightCornerPosition.x < room2.bottomRightCornerPosition.x)
            {
                maxX = room1.bottomRightCornerPosition.x;
            }
            else
            {
                maxX = room2.bottomRightCornerPosition.x;
            }

        PaintConnection(room1, room2, minX, maxX, tilemap, tile, altFloorTile);

            //rooms have overlap
            /*if (Math.Abs(room2.bottomRightCornerPosition.x - room1.topLeftCornerPosition.x) < room2.width)
            {
                int overlap = room2.bottomRightCornerPosition.x - room1.topLeftCornerPosition.x;
                int excessL = room2.topLeftCornerPosition.x - room1.topLeftCornerPosition.x;
                int excessR = room2.bottomRightCornerPosition.x - room1.bottomRightCornerPosition.x;
                if (excessL > 0)
                {
                    pathBLCornerX = overlap + excessL;
                }
                pathBRCornerX = overlap + (overlap - room2.bottomRightCornerPosition.x);
                if (excessR > 0)
                {
                    pathBRCornerX -= excessR;
                }
                if (pathBRCornerX - pathBLCornerX > 1)
                {
                    int chance = UnityEngine.Random.Range(1, 3);
                    if (chance == 1)
                    {
                        pathBLCornerX = pathBRCornerX - 1;
                    }
                    else if (chance == 2)
                    {
                        pathBRCornerX = pathBLCornerX + 1;
                    }
                }
                if (room1.bottomRightCornerPosition.y - room2.topLeftCornerPosition.y > 0)
                {
                    for (int j = room2.topLeftCornerPosition.y; j <= room1.bottomRightCornerPosition.y; j++)
                    {
                        for (int i = pathBLCornerX; i <= pathBRCornerX; i++)
                        {
                            var tilePosition = new Vector3Int(i, j, 0);
                            tilemap.SetTile(tilePosition, tile);
                        }
                    }
                }
                else
                {
                    for (int j = room1.topLeftCornerPosition.y; j <= room2.bottomRightCornerPosition.y; j++)
                    {
                        for (int i = pathBLCornerX; i <= pathBRCornerX; i++)
                        {
                            var tilePosition = new Vector3Int(i, j, 0);
                            tilemap.SetTile(tilePosition, tile);
                        }
                    }
                }
            }
            else if (Math.Abs(room2.bottomRightCornerPosition.y - room1.topLeftCornerPosition.y) < room2.height)
            {
                //rooms have overlap
                if (Math.Abs(room2.bottomRightCornerPosition.x - room1.topLeftCornerPosition.x) < room2.width)
                {
                    int overlap = room2.bottomRightCornerPosition.x - room1.topLeftCornerPosition.x;
                    int excessL = room2.topLeftCornerPosition.x - room1.topLeftCornerPosition.x;
                    int excessR = room2.bottomRightCornerPosition.x - room1.bottomRightCornerPosition.x;
                    if (excessL > 0)
                    {
                        pathBLCornerX = overlap + excessL;
                    }
                    pathBRCornerX = overlap + (overlap - room2.bottomRightCornerPosition.x);
                    if (excessR > 0)
                    {
                        pathBRCornerX -= excessR;
                    }
                    if (pathBRCornerX - pathBLCornerX > 1)
                    {
                        int chance = UnityEngine.Random.Range(1, 3);
                        if (chance == 1)
                        {
                            pathBLCornerX = pathBRCornerX - 1;
                        }
                        else if (chance == 2)
                        {
                            pathBRCornerX = pathBLCornerX + 1;
                        }
                    }
                    if (room1.bottomRightCornerPosition.y - room2.topLeftCornerPosition.y > 0)
                    {
                        for (int j = room2.topLeftCornerPosition.y; j <= room1.bottomRightCornerPosition.y; j++)
                        {
                            for (int i = pathBLCornerX; i <= pathBRCornerX; i++)
                            {
                                var tilePosition = new Vector3Int(i, j, 0);
                                tilemap.SetTile(tilePosition, tile);
                            }
                        }
                    }
                    else
                    {
                        for (int j = room1.topLeftCornerPosition.y; j <= room2.bottomRightCornerPosition.y; j++)
                        {
                            for (int i = pathBLCornerX; i <= pathBRCornerX; i++)
                            {
                                var tilePosition = new Vector3Int(i, j, 0);
                                tilemap.SetTile(tilePosition, tile);
                            }
                        }
                    }
                }
            }
        }

        /*void PaintEnemies(Room room, Tilemap tilemap, GameObject[] enemies)
        {
            double adjustment = 0;
            for (int j = 0; j <= room.height; j++)
            {
                for (int i = 0; i <= room.width; i++)
                {
                    var tilePosition = new Vector3Int(room.x + i,room.y + j,0);
                    chance = rand.NextDouble() + adjustment;
                    if (chance < .08)
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
        }*/
        
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


}

using System;
using System.ComponentModel.Design.Serialization;
using JetBrains.Annotations;
using Mono.Cecil.Cil;
using UnityEditor.Rendering.CustomRenderTexture.ShaderGraph;
using UnityEngine;

public class DungeonTree : MonoBehaviour
{
    System.Random rand = new System.Random();
    Vector2 dungeonSize = new Vector2(40,40);
    Room root;
    int depth = 4;
    Room currentRoom;
    Vector2 cutLocation = new Vector2(0,0);

    void Start()
    {
        BinarySpatialPartition();
    }
    
    public void BinarySpatialPartition()
    {
        
        root = new Room();
        
        root.bottomRightCornerPosition = dungeonSize;
        currentRoom = root;
        
        
        /*if (root.leftChild.leftChild != null)
        {
            depth = 2;
            DivideRoom(root.leftChild);
        }
        else 
        {
            depth = 2;
            DivideRoom(root.rightChild);
        }*/



    }
    public void DivideRoom()
    {
        for (int x = 1; x <= depth; x++)
        {
            Debug.Log(x);
            if (rand.Next(0,1) == 0)
            {
                Debug.Log(currentRoom);
                cutLocation.x = rand.Next(1,(int)dungeonSize.x - 1);
                //Debug.Log(currentRoom.leftChild);
                currentRoom.InsertLeftChild();
                currentRoom.leftChild.topLeftCornerPosition = new Vector2(0,0);
                currentRoom.leftChild.bottomRightCornerPosition = new Vector2(cutLocation.x - 1,40);
                currentRoom.InsertRightChild(new Room());
                currentRoom.rightChild.topLeftCornerPosition = new Vector2(cutLocation.x,0);
                currentRoom.rightChild.bottomRightCornerPosition = new Vector2(cutLocation.x,40);
            }
            else
            {
                cutLocation.y = rand.Next(1,(int)dungeonSize.x - 1);
                currentRoom.InsertLeftChild(new Room());
                currentRoom.leftChild.topLeftCornerPosition = new Vector2(0,0);
                currentRoom.leftChild.bottomRightCornerPosition = new Vector2(0,cutLocation.y - 1);
                currentRoom.InsertRightChild(new Room());
                currentRoom.rightChild.topLeftCornerPosition = new Vector2(0,cutLocation.y);
                currentRoom.rightChild.bottomRightCornerPosition = new Vector2(0,40);
            }
            
            if (rand.Next(0,1) == 0) 
            {
                currentRoom.leftChild = currentRoom;
            }
            else 
            {
                currentRoom.rightChild = currentRoom;
            }
            x++;
        }
        //Debug.Log(x);
    }
}

public class Room 
{
    public Vector2 topLeftCornerPosition = Vector2.zero;
    public Vector2 bottomRightCornerPosition = Vector2.zero;
    public Room sibling = null;
    public Room parent = null;
    public Room leftChild = null;
    public Room rightChild = null;
    public void InsertLeftChild()
    {
        if (leftChild == null)
        {
            leftChild = new Room();
        }
        else {
            Debug.LogError(this + " already has a left child.");
        }
    }
    public void InsertLeftChild(Room child)
    {
        if (leftChild == null)
        {
            leftChild = child;
        }
        else {
            Debug.LogError(this + " already has a left child.");
        }
    }
    public void InsertRightChild(Room child)
    {
        if (rightChild == null)
        {
            rightChild = child;
        }
        else {
            Debug.LogError(this + " already has a right child.");
        }
    }
}

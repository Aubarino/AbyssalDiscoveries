using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Item : IComparable<Item>
{
    public string Path;
    public Mesh Mesh;
    public string Name;
    public Sprite Sprite;
    public string Tag;
    public string Behaviour;

    public Item(string ItemPath, Mesh ItemMesh, string ItemName, Sprite ItemSprite, string ItemTag, string ItemBehaviour)
    {
        Path = ItemPath;
        Mesh = ItemMesh;
        Name = ItemName;
        Tag = ItemTag;
        Sprite = ItemSprite;
        Behaviour = ItemBehaviour;
    }

    //This method is required by IComparable
    public int CompareTo(Item other)
    {
        if(other == null)
        {
            return 1;
        }
        return 0;
    }
}


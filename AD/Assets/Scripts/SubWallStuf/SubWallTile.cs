using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class SubWallTile : IComparable<SubWallTile>
{

    public string TileID = "surface"; //used to identify the tile, this is the default
    public string PrefabBase = "Fabs/surface_single"; //used to load all the materials 'n etc, as a prefab cause Bruh it's easier.
    public float Damage = 0f;
    public float DamageMax = 3f;
    public Mesh[] DamageVaris = new Mesh[]{ //normalized to the damage value and damage value max, using a division of the amount of damage options provided here.
        Resources.Load<Mesh>("Models/tiles/surfacehole0_single"),
        Resources.Load<Mesh>("Models/tiles/surfacehole1_single"),
        Resources.Load<Mesh>("Models/tiles/surfacehole2_single")
        };
    public float WaterLetIn = 0f;
    public Mesh Mesh = Resources.Load<Mesh>("Models/tiles/surface_single");

    public SubWallTile()
    {
        
    }

    public string GetPrefabBase()
    {return(PrefabBase);}

    public int CompareTo(SubWallTile other)
    {
        if(other == null)
        {
            return 1;
        }
        return 0;
    }
}
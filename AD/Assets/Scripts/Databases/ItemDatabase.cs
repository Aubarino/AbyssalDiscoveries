using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ItemDatabase
{
    public static List<Item> Items;

    public static void Startup() //runs on awake via an object somewhere
    {
        Items = new List<Item>();
        Items.Clear();
        RegisterDevItems();
        Debug.Log("Item Database Startup");
    }

    private static void RegisterDevItems()
    {
        Items.Add(new Item("null", Resources.Load<Mesh>("Models/NodeStuff/air"), "error", null, "error", null));
        Debug.Log(Items[0].Name);
    }
}

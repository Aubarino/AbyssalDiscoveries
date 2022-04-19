using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
public static class AfflictionDatabase
{
    public static List<AfflictionPrefab> Afflictions;

    public static void Startup() //runs on awake via an object somewhere
    {
        Afflictions = new List<AfflictionPrefab>();
        Afflictions.Clear();
        RegisterDevItems();
        Debug.Log("Item Database Startup");
    }

    private static void RegisterDevItems()
    {
        Afflictions.Add(new AfflictionPrefab("Brain damage", "The patient has odd behaviour, with no visible signs of damage.", "neurotrauma", true, new Vector2(0f,2f), 100f, "", "", 100f));
        Debug.Log(Afflictions[0].name);
    }
}

public class AfflictionPrefab : IComparable<AfflictionPrefab>
{
    public string name;
    public string description;
    public string identifier;
    public bool scaleVitality;
    public Vector2 vitalityReduction;
    public float visibilityTreshold;
    public string script;
    public string onDamageScript;
    public float maxStrength;
    public AfflictionPrefab(string nm, string dsc, string id, bool scv, Vector2 red, float vistresh, string scr, string ondmg, float str)
    {
        name = nm;
        description = dsc;
        identifier = id;
        scaleVitality = scv;
        vitalityReduction = red;
        visibilityTreshold = vistresh;
        script = scr;
        onDamageScript = ondmg;
        maxStrength = str;
    }
    public int CompareTo(AfflictionPrefab other)
    {
        if (other == null)
        {
            return 1;
        }
        return 0;
    }
}
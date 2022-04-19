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
        RegisterDevAffs();
        Debug.Log("Affliction Database Startup");
    }

    private static void RegisterDevAffs()
    {
        Afflictions.Add(new AfflictionPrefab(
            "Brain damage", 
            "The patient has odd behaviour, with no visible signs of damage.",
            "neurotrauma",
            true,
            new Vector2(0f,2f), 
            100f,
            "", 
            "",
            100f,
            Color.white, 
            Color.red,
            null
            ));
        Afflictions.Add(new AfflictionPrefab(
            "Barotrauma",
            "The patient has red, veiny eyes, is in pain, and severe organ damage is suspected.",
            "barotrauma",
            true,
            new Vector2(0f,2f),
            0f,
            "",
            "",
            100f,
            Color.white,
            Color.red,
            Resources.Load<Sprite>("barotrauma")
            ));
        Afflictions.Add(new AfflictionPrefab(
            "Hypoxemia",
            "Hypo, meaning low, xemia, meaning oxygen presence in blood.",
            "hypoxemia",
            true,
            new Vector2(0f, 2f),
            10f,
            "",
            "",
            100f,
            Color.cyan,
            Color.magenta,
            Resources.Load<Sprite>("oxygen")
            ));
        Debug.Log(Afflictions[0].name);
    }
    public static AfflictionPrefab GetAffliction(string identifier)
    {
        foreach(AfflictionPrefab prefab in Afflictions)
        {
            if(prefab.identifier == identifier) return prefab;
        }
        Debug.Log("Error getting affliction with identifier: " + identifier);
        return null;
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
    public Color mincolor, maxcolor;
    public Sprite icon;
    public AfflictionPrefab(string nm, string dsc, string id, bool scv, Vector2 red, float vistresh, string scr, string ondmg, float str, Color min, Color max, Sprite ico)
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
        mincolor = min;
        maxcolor = max;
        icon = ico;
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

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

    private static void RegisterDevAffs() //AFFLICTION TRIGGERS: drown, lowoxygen, barotrauma
    {
        Afflictions.Add(new AfflictionPrefab(
            "Brain damage",
            "The patient has odd behaviour, with no visible signs of damage.",
            "neurotrauma",
            true,
            new Vector2(0f, 2f),
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
            new Vector2(0f, 2f),
            0f,
            "",
            "",
            100f,
            Color.white,
            Color.red,
            Resources.Load<Sprite>("Afflictions/barotrauma2")
            ));
        Afflictions.Add(new AfflictionPrefab( //Add the new affliction using this
            "Hypoxemia", //The ingame affliction name
            "Hypo, meaning low, xemia, meaning oxygen presence in blood.", //The ingame affliction description, eg. The patient has red, veiny eyes, is in pain, and severe organ damage is suspected.
            "hypoxemia", //the identifier, used only in code
            false, //whether the affliction scales with vitality (if false, it will reduce the vitality a flat amount. if true, it will reduce vitality from 0 to 200, killing you)
            new Vector2(0f, 100f), //the amount of vitality that the affliction removes
            10f, //the strength treshold where the vitality becomes visible onscreen
            "", //per second script (unused)
            "", //on damage script (unused)
            100f, //the max affliction strength
            Color.cyan, //the beginning affliction color in the ui (at 0 strength)
            Color.red, //the ending affliction color in the ui (at max strength)
            Resources.Load<Sprite>("Afflictions/lowoxygen"), //the ingame affliction icon
            new AffTrigger("neurotrauma", 1f, 3.4f, 70f) //the affliction trigger. first is the wanted affliction identifier, second is the chance per second, third is the strength of the given affliction, fourth is the min current afflictions strength
            ));
        Debug.Log(Afflictions[0].name);
    }
    public static AfflictionPrefab GetAffliction(string identifier)
    {
        foreach (AfflictionPrefab prefab in Afflictions)
        {
            if (prefab.identifier == identifier) return prefab;
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
    public AffTrigger afftrig;
    public AfflictionPrefab(string nm, string dsc, string id, bool scv, Vector2 red, float vistresh, string scr, string ondmg, float str, Color min, Color max, Sprite ico, AffTrigger aff = null)
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
        afftrig = aff;
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
public class AffTrigger
{
    public string affliction;
    public float chance;
    public float strength;
    public float minStrength;
    public AffTrigger(string aff, float ch, float str, float minstr)
    {
        affliction = aff;
        chance = ch;
        strength = str;
        minStrength = minstr;
    }
}
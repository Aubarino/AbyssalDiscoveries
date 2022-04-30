using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Electronics : MonoBehaviour
{
    public bool On;
    public bool Broken;
    public bool isOnFire;
    [Space]
    public float Durability = 100f; //i changed this to Durability because DamageAmount was stupid
    public float DurabilityDecrease = -0.04f;

    void Update()
    {
        if (On && !Broken)
        {
            Durability += DurabilityDecrease * Time.deltaTime;
        }

        if (Durability <= 0f)
        {
            Broken = true;
        }
        else
        {
            Broken = false;
        }

        if (isOnFire)
        {
            DurabilityDecrease = -5.5f;
        } else
        {
            DurabilityDecrease = -0.04f;
        }
    }

    void OnCollisionEnter(Collision col) //or OnTriggerEnter, depends on what you do
    {
        if (col.gameObject.tag == "FireExtinguisherPuff") { //rename this when you get to the point where you add a fire extinguisher
            isOnFire = false;
        } else if (col.gameObject.tag == "Water") {
            isOnFire = false;
        }
    }

    public void ResetConfigs() { 
        On = false;
        Broken = false;
        isOnFire = false;
        Durability = 100f;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reactor : MonoBehaviour
{   
    public int Fuel = 1; //goes from 0-4 like in baro
    [Space]
    public bool On; //determines if the generator is on
    public bool Broken; //if true, the generator cannot be used
    public bool Overload; //power overload
    public bool isOnFire; //explains itself
    [Space]
    public float PowerOutput = 0f; //how much power is being generated
    public float DamagedAmount = 0f; //how much its damaged
    float Timer;
    float FireTime;

    public void ResetReactor() { //call this when you wanna reset the reactor
        Fuel = 0;
        Broken = false;
        On = false;
        Overload = false;
        isOnFire = false;
        PowerOutput = 0f;
        DamagedAmount = 0f;
        Timer = 0f;
        FireTime = Random.Range(Random.Range(1f, 4f), Random.Range(7f, 10f));
    }

    void Start() {
        FireTime = Random.Range(Random.Range(1f, 4f), Random.Range(7f, 10f));
    }

    void Update() {
        Timer += Time.deltaTime;

        if (On && !Broken)
        {
            switch (Fuel) //my first time ever using switch()
            {
                default:
                    break;
                case 0:
                    if (PowerOutput < 0f || PowerOutput == 0f)
                    {
                        PowerOutput = 0f;
                        return;
                    }
                    PowerOutput -= 2f * Time.deltaTime;
                    Overload = false;
                    break;
                case 1:
                    DamagedAmount += 0.04f * Time.deltaTime;
                    PowerOutput += 2f * Time.deltaTime;
                    Overload = false;
                    break;
                case 2:
                    DamagedAmount += 0.5f * Time.deltaTime;
                    PowerOutput += 4f * Time.deltaTime;
                    Overload = true;
                    break;
                case 3:
                    DamagedAmount += 1f * Time.deltaTime;
                    PowerOutput += 6f * Time.deltaTime;
                    Overload = true;
                    break;
                case 4:
                    DamagedAmount += 1.5f * Time.deltaTime;
                    PowerOutput += 8f * Time.deltaTime;
                    Overload = true;
                    break;
            }

            if (DamagedAmount >= 100f) {
                Broken = true;
            } else {
                Broken = false;
            }

            if (isOnFire) {
                DamagedAmount += 9.5f * Time.deltaTime;
            }

            if (PowerOutput > 100f) {
                PowerOutput = 100f;
            }

            if (Overload) {
                if (Timer >= FireTime) {
                    this.isOnFire = true;
                }
            }
        }
        if (!On || Broken) {
            if (PowerOutput < 0f || PowerOutput == 0f) {
                PowerOutput = 0f;
                return;
            }
            PowerOutput -= 2f * Time.deltaTime;
            Overload = false;
        }
    }

    void OnCollisionEnter(Collision col) //or OnTriggerEnter, depends on what you do
    {
        if (col.gameObject.tag == "FireExtinguisherPuff") { //rename this when you get to the point where you add a fire extinguisher
            isOnFire = false;
        }
    }
}

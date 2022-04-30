using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reactor : MonoBehaviour
{
    public Electronics ReactorElectronics;
    public Light OnIndicator;
    [Space]
    public Color OnColor;
    public Color BrokenColor;
    public Color NoFuelColor;
    [Space]
    public int Fuel = 1; //goes from 0-4 like in baro
    [Space]
    public bool Overload; //power overload
    [Space]
    public float PowerOutput = 0f; //how much power is being generated
    float Timer;
    float FireTime;

    public void ResetReactor() { //call this when you wanna reset the reactor
        Fuel = 0;
        Overload = false;
        PowerOutput = 0f;
        Timer = 0f;
        FireTime = Random.Range(Random.Range(1f, 4f), Random.Range(7f, 10f));
    }

    void Start() {
        FireTime = Random.Range(Random.Range(1f, 4f), Random.Range(7f, 10f));
    }

    void Update() {
        Timer += Time.deltaTime;
        OnIndicator.enabled = ReactorElectronics.On;
        if (ReactorElectronics.On && !ReactorElectronics.Broken)
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
                    ReactorElectronics.DurabilityDecrease = 0f;
                    PowerOutput -= 2f * Time.deltaTime;
                    Overload = false;

                    OnIndicator.color = NoFuelColor;
                    break;
                case 1:
                    ReactorElectronics.DurabilityDecrease = -0.04f;
                    PowerOutput += 2f * Time.deltaTime;
                    Overload = false;

                    OnIndicator.color = OnColor;
                    break;
                case 2:
                    ReactorElectronics.DurabilityDecrease = -0.5f;
                    PowerOutput += 4f * Time.deltaTime;
                    Overload = true;

                    OnIndicator.color = BrokenColor;
                    break;
                case 3:
                    ReactorElectronics.DurabilityDecrease = -1f;
                    PowerOutput += 6f * Time.deltaTime;
                    Overload = true;

                    OnIndicator.color = BrokenColor;
                    break;
                case 4:
                    ReactorElectronics.DurabilityDecrease = -1.5f;
                    PowerOutput += 8f * Time.deltaTime;
                    Overload = true;
                    break;
            }



            if (PowerOutput > 100f) {
                PowerOutput = 100f;
            }

            if (Overload) {
                if (Timer >= FireTime) {
                    ReactorElectronics.isOnFire = true;
                }
            }
        }
        if (!ReactorElectronics.On || ReactorElectronics.Broken) {
            if (PowerOutput < 0f || PowerOutput == 0f) {
                PowerOutput = 0f;
                return;
            }
            OnIndicator.color = BrokenColor;
            PowerOutput -= 2f * Time.deltaTime;
            Overload = false;
        }
    }
}

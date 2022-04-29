using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmergencyLightBehavior : MonoBehaviour
{
    public bool On;
    public Reactor ConnectedReactor; //put the reactor script here in the inspector
    public Light elight;
    void Update() {
        elight.intensity = ConnectedReactor.PowerOutput / 6.5f;
        if (On) {
            elight.enabled = true;
        } else {
            elight.enabled = false;
        }

        if (ConnectedReactor.ReactorElectronics.isOnFire == true || ConnectedReactor.Overload == true || ConnectedReactor.Fuel > 1) {
            On = true;
        }
    }
}

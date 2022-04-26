using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmergencyLightBehavior : MonoBehaviour
{
    public bool On;
    public Reactor ConnectedReactor; //put the reactor script here in the inspector
    public Light light;
    void Update() {
        if (On) {
            light.enabled = true;
        } else {
            light.enabled = false;
        }

        if (ConnectedReactor.isOnFire == true || ConnectedReactor.Overload == true) {
            On = true;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirAreaManager : MonoBehaviour
{
    public GameObject AirNodeObj;
    private Material mat;
    public float WaterVolume = 0;
    public float WaterVolumeMax
    { get; private set; }
    private float SingleUnit = 3;
    public float WaterLevelY = 0; //the predicted y level
    public float AirAreaYBounds
    { get; private set; }
    public BoxCollider col;

    private void Start()
    {
        CreateWaterstffff();
        col = GetComponent<BoxCollider>();
    }

    private void CreateWaterstffff() //professional B)))))))
    {
        var rend = AirNodeObj.GetComponent<MeshRenderer>();
        rend.material = new Material(Resources.Load<Material>("Materials/AirMaterial"));
        //Debug.Log(rend.material.shader.name);
        rend.material.SetFloat("WatLevel", 0.2f);
    }

    private void OnDrawGizmosSelected() //water level preview for the unity editor alone, not used in normal gameplay i guess
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube((AirNodeObj.transform.position + new Vector3(0,WaterLevelY,0)), new Vector3(AirNodeObj.transform.localScale.x * 2, 0, AirNodeObj.transform.localScale.z * 2));
    }

    private void Update()
    {
        var rend = AirNodeObj.GetComponent<MeshRenderer>();
        float WatLevel = Mathf.Clamp(((1 / WaterVolumeMax) * WaterVolume), 0, 1); //set the water level rendering stuff to a devision of the volume max accordingly, 1 is always max.
        rend.material.SetFloat("WatLevel", WatLevel);

        WaterVolumeMax =( //total max water volume- caps at this.
            (AirNodeObj.transform.localScale.x * SingleUnit) *
            (AirNodeObj.transform.localScale.y * SingleUnit) *
            (AirNodeObj.transform.localScale.z * SingleUnit)
        );

        WaterVolume ++;
        WaterVolume = Mathf.Clamp(WaterVolume, 0, WaterVolumeMax); //water deletes past a certain range, sorry. :)

        //Debug.Log(WaterVolume + "Water Volume... Water MAX :" + WaterVolumeMax);
        Vector3 WaterPredictLev = new Vector3(
            (Mathf.Abs(AirNodeObj.transform.forward.y) * AirNodeObj.transform.localScale.z),
            (Mathf.Abs(AirNodeObj.transform.up.y) * AirNodeObj.transform.localScale.y),
            (Mathf.Abs(AirNodeObj.transform.right.y) * AirNodeObj.transform.localScale.x)
            );
        float AirAreaYBounds = (((WaterPredictLev.x + WaterPredictLev.y + WaterPredictLev.z) * 1)); //water level y pos prediction stuff
        WaterLevelY = (((WatLevel * 2) - 1) * AirAreaYBounds); //calculate the water level in y 
    }
}

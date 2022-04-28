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
    public Vector3 AirAreaBounds
    { get; private set; }
    public BoxCollider col;
    public float WatLevelCore
    { get; private set; }

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
        Gizmos.DrawWireCube((AirNodeObj.transform.position + new Vector3(0,WaterLevelY,0)), new Vector3(AirAreaBounds.x, 0, AirAreaBounds.z));
        Gizmos.DrawWireCube((AirNodeObj.transform.position), AirAreaBounds);
    }

    private void Update()
    {
        var rend = AirNodeObj.GetComponent<MeshRenderer>();
        WatLevelCore = Mathf.Clamp(((1 / WaterVolumeMax) * WaterVolume), 0, 1); //set the water level rendering stuff to a devision of the volume max accordingly, 1 is always max.
        rend.material.SetFloat("WatLevel", WatLevelCore);

        WaterVolumeMax =( //total max water volume- caps at this.
            (AirNodeObj.transform.localScale.x * SingleUnit) *
            (AirNodeObj.transform.localScale.y * SingleUnit) *
            (AirNodeObj.transform.localScale.z * SingleUnit)
        );

        //WaterVolume ++;
        WaterVolume = Mathf.Clamp(WaterVolume, 0, WaterVolumeMax); //water deletes past a certain range, sorry. :)

        //Debug.Log(WaterVolume + "Water Volume... Water MAX :" + WaterVolumeMax);
        Vector3 WaterPredictLevY = new Vector3(
            (Mathf.Abs(AirNodeObj.transform.forward.y) * AirNodeObj.transform.localScale.z),
            (Mathf.Abs(AirNodeObj.transform.up.y) * AirNodeObj.transform.localScale.y),
            (Mathf.Abs(AirNodeObj.transform.right.y) * AirNodeObj.transform.localScale.x)
            );
        Vector3 WaterPredictLevX = new Vector3(
            (Mathf.Abs(AirNodeObj.transform.forward.x) * AirNodeObj.transform.localScale.z),
            (Mathf.Abs(AirNodeObj.transform.up.x) * AirNodeObj.transform.localScale.y),
            (Mathf.Abs(AirNodeObj.transform.right.x) * AirNodeObj.transform.localScale.x)
            );
        Vector3 WaterPredictLevZ = new Vector3(
            (Mathf.Abs(AirNodeObj.transform.forward.z) * AirNodeObj.transform.localScale.z),
            (Mathf.Abs(AirNodeObj.transform.up.z) * AirNodeObj.transform.localScale.y),
            (Mathf.Abs(AirNodeObj.transform.right.z) * AirNodeObj.transform.localScale.x)
            );
        AirAreaBounds = new Vector3( //water area pos prediction stuff
            ((WaterPredictLevX.x + WaterPredictLevX.y + WaterPredictLevX.z) * 2),
            ((WaterPredictLevY.x + WaterPredictLevY.y + WaterPredictLevY.z) * 2),
            ((WaterPredictLevZ.x + WaterPredictLevZ.y + WaterPredictLevZ.z) * 2)
        );
        WaterLevelY = (((WatLevelCore * 2) - 1) * (AirAreaBounds.y * 0.5f)); //calculate the water level in y 
    }
}

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
    private float WaterLevelY = 0;

    private void Start()
    {
        CreateWaterstffff();
    }

    private void CreateWaterstffff() //professional B)))))))
    {
        var rend = AirNodeObj.GetComponent<MeshRenderer>();
        rend.material = new Material(Resources.Load<Material>("Materials/AirMaterial"));
        //Debug.Log(rend.material.shader.name);
        rend.material.SetFloat("WatLevel", 0.2f);
        WaterVolumeMax =( //predict water volume off the sale of the air area / node thing.. and the unit scale bullshit
            (AirNodeObj.transform.localScale.x * SingleUnit) *
            (AirNodeObj.transform.localScale.y * SingleUnit) *
            (AirNodeObj.transform.localScale.z * SingleUnit)
        );
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube((AirNodeObj.transform.position + new Vector3(0,WaterLevelY,0)), new Vector3(AirNodeObj.transform.localScale.x * 2, 0, AirNodeObj.transform.localScale.z * 2));
    }

    private void Update()
    {
        var rend = AirNodeObj.GetComponent<MeshRenderer>();
        float WatLevel = Mathf.Clamp(((1 / WaterVolumeMax) * WaterVolume), 0, 1); //set the water level rendering stuff to a devision of the volume max accordingly, 1 is always max.
        rend.material.SetFloat("WatLevel", WatLevel);
        WaterVolume ++;
        WaterVolume = Mathf.Clamp(WaterVolume, 0, WaterVolumeMax); //water deletes past a certain range, sorry. :)
        Debug.Log(WaterVolume + "Water Volume... Water MAX :" + WaterVolumeMax);
        float WaterLevelYTemp = ((((WatLevel * 2) * 1.05f) - 1.05f) * Mathf.Abs(AirNodeObj.transform.localScale.y)); //water level y pos prediction stuff
        WaterLevelY = ((WaterLevelYTemp * 1));
    }
}

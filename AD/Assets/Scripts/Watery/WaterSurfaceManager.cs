using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterSurfaceManager : MonoBehaviour
{
    public GameObject AirNodeObj;
    public GameObject WaterSurfaceObj;
    private MeshRenderer WatMeshRend;
    private AirAreaManager AirNodeObjAirManager;
    Quaternion RotationFix;

    private void Start()
    {
        WaterSurfaceObj = this.gameObject;
        WatMeshRend = WaterSurfaceObj.GetComponent<MeshRenderer>();
        AirNodeObj = WaterSurfaceObj.transform.parent.gameObject;
        AirNodeObjAirManager = AirNodeObj.GetComponent<AirAreaManager>();
        CreateWaterSurfaceSt();
        RotationFix = WaterSurfaceObj.transform.rotation;
        WaterSurfaceObj.transform.parent = null;
    }

    private void CreateWaterSurfaceSt()
    {
        WatMeshRend.material = new Material(Resources.Load<Material>("Materials/WaterMaterial"));
    }

    void Update()
    { 
        SyncManagers();
        WaterSurfaceObj.transform.rotation = RotationFix;
        WaterSurfaceObj.transform.position = (AirNodeObj.transform.position + new Vector3(0,AirNodeObjAirManager.WaterLevelY,0));
        WaterSurfaceObj.transform.localScale = new Vector3(
        (AirNodeObjAirManager.AirAreaBounds.x * 0.5f),
        1,
        (AirNodeObjAirManager.AirAreaBounds.z * 0.5f)
        );
    }

    private void SyncManagers() //syncs stuff in the surface object with the air area.
    {
        var AirNodeMat = AirNodeObj.GetComponent<MeshRenderer>().material;
        WatMeshRend.material.SetFloat("WatFog", AirNodeMat.GetFloat("WatFog"));
        WatMeshRend.material.SetColor("WatFogColour", AirNodeMat.GetColor("WatFogColour"));
    }
}

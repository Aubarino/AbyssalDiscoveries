using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubWallManager : MonoBehaviour
{
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void BuildMesh(GameObject obj)
    {
        Vector3 position = obj.transform.position;
        obj.transform.position = Vector3.zero;

        MeshFilter[] meshFilters = obj.GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];
        int i = 1;
        while (1 < meshFilters.Length)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[1].transform = meshFilters[i].transform.localToWorldMatrix;
            meshFilters[i].gameObject.SetActive(false);
            i++;
        }

        // obj.GetComponent<MeshFilter>.mesh = new Mesh();
        // obj.GetComponent<MeshFilter>.mesh.CombineMeshes(combine, true, true);
        obj.transform.gameObject.SetActive(true);

        obj.transform.position = position;
    }
}

using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class SubWallManager : MonoBehaviour
{

    public Vector2 TileGridSize = new Vector2(4,4); //the dimensions of the grid
    public SubWallTile[,] TileGrid;
    private Vector2 TileVari; //internal, used for offset of tiles so they center on the obj
    public Vector2 ScalePredict //internal but readable, used for a total scale
    { get; private set; }
    private bool OutsideUnityEditor = false;

    private void Awake()
    {
        OutsideUnityEditor = true;
    }

    private void Start()
    {
        //move it to nothing so we can have accurate predictions local to the main transform without 4th dimensional bullshit happening X_X
        Vector3 position = transform.position;
        transform.position = Vector3.zero;
        Vector3 rotation = transform.eulerAngles;
        transform.eulerAngles = Vector3.zero;

        // startup stuff for the grid
        TileVari = new Vector2(
            (TileGridSize.x * -0.5f) + 0.5f,
            (TileGridSize.y * -0.5f) + 0.5f
            );
        ScalePredict = new Vector2(
            TileGridSize.x + TileVari.x,
            TileGridSize.y + TileVari.x
            );

        TileGrid = new SubWallTile[(int) TileGridSize.x, (int) TileGridSize.y];
        InitGenGrid();
        GenMeshObjects();
        BuildMesh();

        //move it back
        transform.position = position;
        transform.eulerAngles = rotation;
    }

    private void Update()
    {
        
    }

    private void OnDrawGizmos() //water level preview for the unity editor alone, not used in normal gameplay i guess
    {
        Gizmos.matrix = transform.localToWorldMatrix;
        if (OutsideUnityEditor){
            Gizmos.color = new Color32(238, 194, 49, 255);;
            Gizmos.DrawWireCube(new Vector3(0,0,0.375f), new Vector3(TileGridSize.x, TileGridSize.y, 0.25f));
        }else{
            Gizmos.color = new Color32(175, 190, 200, 255);;
            Gizmos.DrawWireCube(Vector3.zero, new Vector3(TileGridSize.x, TileGridSize.y, 1));
            Gizmos.color = new Color32(100, 100, 100, 255);
            Gizmos.DrawCube(new Vector3(0,0,0.375f), new Vector3(TileGridSize.x, TileGridSize.y, 0.25f));
        }
    }

    private void GenMeshObjects() //spawns all the game-objects that are then combined... don't judge me!
    {
        for(int TX = 0; TX < TileGridSize.x; TX++)
        {
            for(int TY = 0; TY < TileGridSize.y; TY++)
            {
                SubWallTile ThisTile = TileGrid[TX,TY]; //this current slot in the tile grid.
                GameObject Me = Instantiate(Resources.Load(ThisTile.PrefabBase) as GameObject);

                //syncs up all the position stuff so it doesn't appear at 0
                Me.transform.localPosition = new Vector3(TX + TileVari.x, TY + TileVari.y);
                Me.transform.eulerAngles = new Vector3(transform.eulerAngles.x,transform.eulerAngles.y,transform.eulerAngles.z);
                Me.transform.SetParent(transform);
            }
        }
    }

    public void InitGenGrid() //fills the tile grid with stuff
    {
        for(int TX = 0; TX < TileGridSize.x; TX++)
        {
            for(int TY = 0; TY < TileGridSize.y; TY++)
            {
                TileGrid[TX,TY] = new SubWallTile();
                SubWallTile Tile = TileGrid[TX, TY];
                
                if (UnityEngine.Random.value > 0.5f) {
                    int rng = UnityEngine.Random.Range(0, 3);
                    Tile.Mesh = Resources.Load<Mesh>("Models/tiles/surfacehole" + rng + "_single");
                    Tile.PrefabBase = "Fabs/surfacehole" + rng + "_single";
                    Tile.TileID = "surfacehole" + rng;
                    Debug.Log("Broken Tile " + rng + " added ");
                }else{
                    Tile.Mesh = Resources.Load<Mesh>("Models/tiles/surface_single");
                    Tile.PrefabBase = "Fabs/surface_single";
                    Tile.TileID = "surface";
                    Debug.Log("Tile 1 added ");
                }
            }
        }
    }

    private void BuildMesh() //generates the mesh of the tile grid 2d array using Game Object MeshFilters X_X there's better ways but this is the simplist, will rework in future maybe
    {
        MeshFilter[] thefilters = GetComponentsInChildren<MeshFilter>(false);
        MeshFilter MyMeshFilt = gameObject.GetComponent<MeshFilter>();

        List<Material> materials = new List<Material>();
        MeshRenderer[] theRenderers = GetComponentsInChildren<MeshRenderer>(false);

        foreach (MeshRenderer renderer in theRenderers)
        {
            if (renderer.transform == transform)
                continue;
            Material[] localMats = renderer.sharedMaterials;
            foreach (Material localMat in localMats){
                if (!materials.Contains (localMat)){
                    materials.Add (localMat);
                }
            }
        }

        List<Mesh> submeshes = new List<Mesh>();
        foreach (Material material in materials){ //makes an entry in an array of models for each part of the model that contains a certain material
            List<CombineInstance> combiners = new List<CombineInstance>();
            foreach(MeshFilter filter in thefilters){
                MeshRenderer renderer = filter.GetComponent<MeshRenderer>();
                if (renderer == null){
                    Debug.LogError (filter.name + "doesn't have a mesh renderer");
                    continue;
                }

                Material[] localMaterials = renderer.sharedMaterials;
                for (int i = 0; i < localMaterials.Length; i++)
                {
                    if (localMaterials[i] != material)
                        continue;
                    CombineInstance cominst = new CombineInstance();
                    cominst.mesh = filter.sharedMesh;
                    cominst.subMeshIndex = i;
                    cominst.transform = filter.gameObject.transform.localToWorldMatrix;
                    if (filter.sharedMesh != null) combiners.Add(cominst);
                }
                if (filter != MyMeshFilt)
                    Destroy(filter.gameObject);
            }

            Mesh mesh = new Mesh();
            mesh.CombineMeshes (combiners.ToArray(), true);
            submeshes.Add (mesh);
        }

        List<CombineInstance> EndCombiner = new List<CombineInstance>();
        foreach (Mesh mesh in submeshes) //combines all the meshes for each material into a single mesh and then sets the mesh to that.
        {
            CombineInstance cominst = new CombineInstance();
            cominst.mesh = mesh;
            cominst.subMeshIndex = 0;
            cominst.transform = Matrix4x4.identity;
            EndCombiner.Add(cominst);
        }
        
        Mesh finalMesh = new Mesh();
        finalMesh.CombineMeshes (EndCombiner.ToArray(), false);
        finalMesh.name = "SubWallGenMesh";
        MyMeshFilt.mesh = finalMesh;

        Debug.Log("Mesh has been combined. with " + submeshes.Count + " materials.");

        MeshCollider meshCollider = gameObject.GetComponent<MeshCollider>();
        meshCollider.sharedMesh = finalMesh;
    }
}

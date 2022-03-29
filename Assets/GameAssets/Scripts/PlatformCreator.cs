using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformCreator : MonoBehaviour
{
    // Start is called before the first frame update
    public int X_Size;
    public int Z_Size;
    public int Tile_Size = 5;
    public GameObject tile;
    public int TileVariants;
    private List<GameObject> platfroms = new List<GameObject>();
    public float coverProb;
    void Start()
    {

    }

    [ContextMenu("Create")]
    public void Create()
    {
        for (int x = 0; x < X_Size / Tile_Size; x++)
        {
            for (int z = 0; z < Z_Size / Tile_Size; z++)
            {
                var obj = GameObject.Instantiate(tile);
                obj.transform.position = new Vector3(x * Tile_Size, 0, z * Tile_Size) + this.transform.position;
                bool activated = obj.GetComponent<FloorPlane>().SetActiveTiles((int)(TileVariants));
                obj.GetComponent<FloorPlane>().activateRails(x, z, (X_Size / Tile_Size) - 1, (Z_Size / Tile_Size) - 1);
                obj.transform.parent = this.transform;
                if (!activated)
                {
                    obj.GetComponent<FloorPlane>().activateAll();
                }
                platfroms.Add(obj);
                if (coverProb > Random.value)
                {
                    obj.GetComponent<FloorPlane>().ActivateRandomCover();
                }
                obj.GetComponent<FloorPlane>().DeleteInavtive();

                DestroyImmediate(obj.GetComponent<FloorPlane>());
            }
        }
    }

    [ContextMenu("Delete")]
    public void Delete()
    {
        foreach(GameObject obj in platfroms)
        {
            DestroyImmediate(obj);
        }
    }
}

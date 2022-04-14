using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorPlane : MonoBehaviour
{
    // Start is called before the first frame update
    public List<GameObject> floorList;
    public List<GameObject> rails;
    public List<GameObject> covers;
    private List<int> activeObject = new List<int>();
    private int m_wall_varient = 0;

    public bool SetActiveTiles(int id,int wall_variant)
    {
        if(id > floorList.Count-1)
        {
            Debug.LogError("ID "+ id.ToString()+" is not available using " +  (floorList.Count - 1).ToString() +" instead");
            id = floorList.Count -1;
        }

        var wall_type_count = rails[0].GetComponentsInChildren<TempWall>().Length;
        if(wall_type_count == 0)
        {
            m_wall_varient = -1;
        }
        else if(wall_variant > wall_type_count-1)
        {
            Debug.LogError("ID "+ wall_variant.ToString()+" is not available using " +  (wall_type_count-1).ToString() +" instead");
            m_wall_varient =  wall_type_count-1;
        }
        else
        {
            m_wall_varient = wall_variant;
        }


        var f_id = 0;
        bool activated = false;
        foreach(GameObject floorTile in floorList)
        {
           if(f_id == id)
           {
                floorTile.SetActive(true);
                activated = true;
           }
           else
           {
                //floorTile.SetActive(false);
                DestroyImmediate(floorTile);
           }
            f_id += 1;
        }

        if(!activated)
        {
            Debug.Log("Problem");
        }
        return activated;
    }

    public void activateAll()
    {
        var l = 0;
        foreach (GameObject rail in rails)
        {
            rail.SetActive(true);
            activeObject.Add(l);
            l += 1;
        }
    }

    public void activateRails(int x, int z, int max_x , int max_z)
    {
        // Activate wall type
        foreach (GameObject rail in rails)
        {
            var wall_types = rail.GetComponentsInChildren<TempWall>();

            if(m_wall_varient != -1)
            {
                int wid =0;
                foreach(var wtype in wall_types)
                {
                    if(wid !=m_wall_varient)
                    {
                        DestroyImmediate(wtype.gameObject);
                    }
                    else
                    {
                        wtype.gameObject.SetActive(true);
                        DestroyImmediate(wtype.GetComponent<TempWall>());
                    }
                    
                    wid +=1;
                }
            }
            rail.SetActive(false);
        }

        if(x==0)
        {
            rails[1].SetActive(true);
            activeObject.Add(1);
        }

        if (z == 0)
        {
            rails[0].SetActive(true);
            activeObject.Add(0);
        }

        if (x == max_x)
        {
            rails[3].SetActive(true);
            activeObject.Add(3);
        }

        if (z == max_z)
        {
            rails[2].SetActive(true);
            activeObject.Add(2);
        }

    }

    private void activateWallID(int id)
    {

    }

    public void DeleteInavtive()
    {
        int l = 0;
        foreach(GameObject obj in rails)
        {
            if (!activeObject.Contains(l))
            {
                DestroyImmediate(obj);
            }
            l += 1;        
        }
    }

    public void ActivateRandomCover()
    {
        covers[Random.Range(0, covers.Count)].SetActive(true);
    }
}

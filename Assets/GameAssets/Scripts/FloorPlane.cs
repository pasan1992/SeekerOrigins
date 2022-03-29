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


    public bool SetActiveTiles(int id)
    {
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
        foreach (GameObject rail in rails)
        {
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

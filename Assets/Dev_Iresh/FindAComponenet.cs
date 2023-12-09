using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindAComponenet : MonoBehaviour
{
    [SerializeField] List<GameObject> _objectToFind;
    // Start is called before the first frame update
    void Start()
    {
        //_objectToFind = transform.GetChild(0).gameObject;
        _objectToFind.Add(FindObjectOfType<AudioSource>().gameObject);

        foreach(var obj in _objectToFind)
        {
            print(obj.name);
            //SM_Prop_Table_Small_01 (1)
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

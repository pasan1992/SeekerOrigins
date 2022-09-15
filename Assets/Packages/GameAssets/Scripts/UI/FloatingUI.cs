using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingUI : MonoBehaviour
{
    // Start is called before the first frame update
    private Transform camera_transfrom;
    void Awake()
    {
        camera_transfrom = Camera.main.transform; 
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.LookAt(this.transform.position + camera_transfrom.forward);
    }
}

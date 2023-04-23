using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIFloatingDialog : MonoBehaviour
{
    [SerializeField] GameObject _mainCam;
    public Transform target;

    public Vector3 offset;

    // Update is called once per frame
    void Update()
    {
        ///transform.rotation = Quaternion.LookRotation(_mainCam.transform.position);
        //transform.rotation = Quaternion.LookRotation(new Vector3(35,-25,0));
        this.transform.position = target.transform.position + offset;
    }
}

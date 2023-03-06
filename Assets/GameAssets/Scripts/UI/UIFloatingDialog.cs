using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIFloatingDialog : MonoBehaviour
{
    [SerializeField] GameObject _mainCam;

    // Update is called once per frame
    void Update()
    {
        //transform.rotation = Quaternion.LookRotation(_mainCam.transform.position);
        transform.rotation = Quaternion.LookRotation(new Vector3(35,-25,0));
    }
}

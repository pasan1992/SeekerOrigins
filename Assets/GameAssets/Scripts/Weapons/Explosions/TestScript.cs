using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    CharacterController charCtrl;
    [SerializeField] LayerMask _layerMask;

    void Start()
    {
        charCtrl = GetComponent<CharacterController>();
    }

    void Update()
    {
        InvokeRepeating("DrawSphere", 1, 10);
    }

    void DrawSphere()
    {
        //print("Working");

        RaycastHit hit;

        Vector3 p1 = transform.position + charCtrl.center;
        float distanceToObstacle = 0;

        // Cast a sphere wrapping character controller 10 meters forward
        // to see if it is about to hit anything.
        //if (Physics.SphereCast(p1, charCtrl.height / 2, transform.forward, out hit, 10, 14))
        if (Physics.SphereCast(p1, charCtrl.height / 2, transform.forward, out hit, 10, _layerMask))
        //if (Physics.SphereCast(transform.position, 4, transform.forward, out hit, 10))
        {
            if (hit.collider != null)
            {
                print("Working 2  " + hit.collider.name);
                //Debug.DrawRay()
            }
            distanceToObstacle = hit.distance;
            //print(distanceToObstacle);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheWorldScript : MonoBehaviour
{
    [SerializeField] int _speed;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.right * Time.deltaTime * _speed);
        //transform.rotation = Quaternion.Euler(1 * Time.deltaTime * _speed, 0, 0);
    }
}

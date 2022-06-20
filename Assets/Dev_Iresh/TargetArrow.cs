using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetArrow : MonoBehaviour
{
    [SerializeField] GameObject _player;
    [SerializeField] GameObject _target;
    [SerializeField] float _hideDistance;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var dir = _target.transform.position - _player.transform.position;

        if (dir.magnitude < _hideDistance)
        {
            gameObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(true);

            var angle = Mathf.Atan2(dir.y, dir.x);
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }


    }
}

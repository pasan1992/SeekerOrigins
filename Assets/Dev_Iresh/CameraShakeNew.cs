using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShakeNew : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(2))
        {
            Shake();
        }
    }

    void Shake()
    {
        Vector3 destination = transform.position + new Vector3(Random.RandomRange(-1, 1), Random.Range(-1, 1), Random.RandomRange(-1, 1));
        float time = Random.Range(0.5f, 1f);

        LeanTween.move(gameObject, destination, time)
            .setEasePunch()
            .setOnComplete(ShakeDone);

        //Vector3 destination = transform.position + new Vector3(Random.RandomRange(-3, 3), Random.Range(-3, 3), Random.RandomRange(-1, 1));
        //float time = Random.Range(0.5f, 1f);

        //LeanTween.move(gameObject, destination, time)
        //    .setEasePunch()
        //    .setOnComplete(Shake);
    }

    void ShakeDone()
    {
        print("shake done");
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class BombExplosion : MonoBehaviour
{
    [SerializeField] CameraShake _cameraShake;
    [SerializeField] VisualEffect _visualEffect;

    // Start is called before the first frame update
    void Awake()
    {
        _visualEffect.Stop();
    }

    // Update is called once per frame
    void Update()
    {
        _visualEffect.Play();
        _cameraShake.StartExplosion();
    }

    public void StartExplosion(){
        _cameraShake.StartExplosion();
    }
}

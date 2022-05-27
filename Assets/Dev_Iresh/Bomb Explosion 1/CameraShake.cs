using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [SerializeField] AnimationCurve _shakeCurve;

    Vector3 _targetPosition;
    Vector3 _offset;

    private void Awake()
    {
        _targetPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = _targetPosition + _offset;
    }

    public void StartExplosion()
    {
        StartCoroutine(ScreenShake());
    }

    IEnumerator ScreenShake()
    {
        for (float t = 0; t < 0.5f; t += Time.deltaTime)
        {
            float y = _shakeCurve.Evaluate(t * 2);
            _offset = new Vector3(0, y, 0);
            yield return null;
        }

        _offset = Vector3.zero;
    }
}

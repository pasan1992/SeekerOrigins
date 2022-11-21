using HutongGames.PlayMaker.Actions;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TheWorldScript : MonoBehaviour
{
    [SerializeField] float _alphaValue;
    [SerializeField] float _alphaValueSpeed;
    [SerializeField] int _speed;
    [SerializeField] int _camSpeed;
    [SerializeField] GameObject _cam;
    [SerializeField] CanvasGroup _canvasGroup;

    bool _isMenuActive = false;

    // Start is called before the first frame update
    void Start()
    {
        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            _isMenuActive = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_isMenuActive)
        {
            CamAnim();
        }

        transform.Rotate(Vector3.right * Time.deltaTime * _speed);
        //transform.rotation = Quaternion.Euler(1 * Time.deltaTime * _speed, 0, 0);
    }

    void CamAnim()
    {
        //_cam.LeanRotate(new Vector3(), 2);
        //print("WORK : " + _cam.transform.rotation.x);
        //print("WORK : " + _cam.transform.rotation);
        //print("WORK : " + _cam.transform.rotation.x);
        //print("WORK : " + _cam.transform.localEulerAngles);
        //print("WORK : " + _cam.transform.localEulerAngles.x);
        //print("WORK : " + _cam.transform.eulerAngles);
        //print("WORK : " + _cam.transform.eulerAngles.x);
        //print("WORK : " + _cam.transform.rotation);
        //print("WORK : " + _cam.transform.rotation.x);
        print("WORK : " + _cam.transform.localRotation);
        print("WORK : " + _cam.transform.localRotation.x);

        if (_cam.transform.localRotation.x < 0)
        {
            _cam.transform.Rotate(Vector3.right * Time.deltaTime * _camSpeed);
        }
        else
        {
            _alphaValue += _alphaValue * _alphaValueSpeed;

            if (_canvasGroup.alpha != 1)
            {
                _canvasGroup.alpha = _alphaValue;

            }
            else
            {
                _isMenuActive = false;
            }
        }
    }
}

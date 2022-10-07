using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIInstanceHUD : MonoBehaviour
{
    [SerializeField] CanvasGroup _mainHUD;
    [SerializeField] GameObject _instanceHUD;

    Vector3 _initialPosition;
    Vector3 _endPosition;

    //RectTransform _initialPosition;
    //RectTransform _endPosition;
    bool _isHUDOpen = false;

    private void Awake()
    {
        _initialPosition = _instanceHUD.transform.position;
        _endPosition = new Vector3(_initialPosition.x, 500f, 0f);

        //_initialPosition = _instanceHUD.GetComponent<RectTransform>().localPosition;
        //_endPosition = new Vector3(0, 600f, 1);

        //_initialPosition = _instanceHUD.GetComponent<RectTransform>();
        //_endPosition.localPosition = new Vector3(0, 600f,1);   
    }


    void Update()
    {
        if (Input.GetKey(KeyCode.Tab))
        {
             HudOpen();
        }
        if (Input.GetKeyUp(KeyCode.Tab))
        {
            HudClose();
        }
    }

    void HudOpen()
    {
        if (!_isHUDOpen)
        {
            _isHUDOpen = true;
            _mainHUD.alpha = 0;
            _instanceHUD.SetActive(true);
            LeanTween.cancel(_instanceHUD);
            Time.timeScale = 0.1f;
            LeanTween.move(_instanceHUD, _endPosition, 0.1f).setEase(LeanTweenType.easeOutBounce).setIgnoreTimeScale(true);
        }
    }

    void HudClose()
    {
        if (_isHUDOpen)
        {
            LeanTween.cancel(_instanceHUD);
            LeanTween.move(_instanceHUD, _initialPosition, 0.1f).setEase(LeanTweenType.easeInBounce)
                .setIgnoreTimeScale(true).setOnComplete((valu) =>{
                    _instanceHUD.SetActive(false);
                    _mainHUD.alpha = 1;
                    _isHUDOpen = false;
                    Time.timeScale = 1f;

                });
        }
    }
}

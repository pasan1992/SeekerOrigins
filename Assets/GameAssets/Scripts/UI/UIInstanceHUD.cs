using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIInstanceHUD : MonoBehaviour
{
    [SerializeField] CanvasGroup _mainHUD;
    [SerializeField] GameObject _instanceHUD;

    Vector3 _initialPosition;
    Vector3 _endPosition;

    private void Awake()
    {
        _initialPosition = _instanceHUD.transform.position;
        _endPosition = new Vector3(_initialPosition.x,600f, 0f);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            StartCoroutine(HudOpen());
        }
        if (Input.GetKeyUp(KeyCode.M))
        {
            StartCoroutine(HudClose());
        }
    }

    IEnumerator HudOpen()
    {
        _mainHUD.alpha = 0;
        _instanceHUD.SetActive(true);
        LeanTween.cancel(_instanceHUD);
        LeanTween.move(_instanceHUD, _endPosition, 0.5f).setEase(LeanTweenType.easeOutBounce);
        yield return new WaitForSeconds(0.5f);
        Time.timeScale = 0.1f;
    }

    IEnumerator HudClose()
    {
        Time.timeScale = 1f;

        LeanTween.cancel(_instanceHUD);
        LeanTween.move(_instanceHUD, _initialPosition, 0.5f).setEase(LeanTweenType.easeInBounce);

        yield return new WaitForSeconds(0.7f);
        _instanceHUD.SetActive(false);
        _mainHUD.alpha = 1;
    }
}

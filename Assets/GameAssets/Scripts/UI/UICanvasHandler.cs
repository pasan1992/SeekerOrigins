using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICanvasHandler : MonoBehaviour
{
    [SerializeField] CanvasGroup _gameHUDCanvas;
    [SerializeField] CanvasGroup _inGameMenuCanvas;
    [SerializeField] CanvasGroup _weponeMenuCanvas;

    bool _isAvailaleEscape = true;
    bool _isAvailaleTab = true;

    bool isTabMenuOpened = false;

    private static UICanvasHandler m_UIHandler;

    // Start is called before the first frame update

    public static UICanvasHandler getInstance()
    {
        if(m_UIHandler ==null)
        {
            m_UIHandler = GameObject.FindObjectOfType<UICanvasHandler>();
        }

        return m_UIHandler;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (_isAvailaleEscape)
            {
                _gameHUDCanvas.alpha = 0;
                _inGameMenuCanvas.gameObject.SetActive(true);
                _inGameMenuCanvas.alpha = 1;

                _inGameMenuCanvas.GetComponent<InGameMenuManager>().GamePaused();
                _isAvailaleEscape = false;
            }
            else
            {
                _inGameMenuCanvas.GetComponent<InGameMenuManager>().GamePaused();

                _inGameMenuCanvas.alpha = 0;
                _inGameMenuCanvas.gameObject.SetActive(false);
                _gameHUDCanvas.alpha = 1;

                _isAvailaleEscape = true;
            }
        }

        if (Input.GetKey(KeyCode.Tab))
        {
            _gameHUDCanvas.alpha = 0;
            _weponeMenuCanvas.gameObject.SetActive(true);
            _weponeMenuCanvas.alpha = 1;

            if (_weponeMenuCanvas.GetComponent<UIInstanceHUD>().isFreeToOpen)
            {
                _weponeMenuCanvas.GetComponent<UIInstanceHUD>().CallToOpen();
                isTabMenuOpened = true;
            }
        }

        if (Input.GetKeyUp(KeyCode.Tab))
        {
            _weponeMenuCanvas.GetComponent<UIInstanceHUD>().CallToClose();

            _weponeMenuCanvas.alpha = 0;
            _weponeMenuCanvas.gameObject.SetActive(false);

            _gameHUDCanvas.alpha = 1;
            isTabMenuOpened = false;
        }
    }

    public bool TabMenuOpened()
    {
        return isTabMenuOpened;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class UICanvasHandler : MonoBehaviour
{
    [SerializeField] CanvasGroup _gameHUDCanvas;
    [SerializeField] CanvasGroup _inGameMenuCanvas;
    [SerializeField] CanvasGroup _weponeMenuCanvas;
    [SerializeField] CanvasGroup _instantVideoPlayerCanvas;
    [SerializeField] CanvasGroup _videoPlayerCanvas;

    bool _isAvailaleEscape = true;
    bool _isAvailaleTab = true;
    bool _isOpenInstantVideo = false;

    public bool isTabMenuOn = false;

    public static UICanvasHandler this_instance;

    public static UICanvasHandler getInstance()
    {
        if(this_instance ==null)
        {
            this_instance = GameObject.FindObjectOfType<UICanvasHandler>();
        }

        return this_instance;
    }

    // Update is called once per frame
    void Update()
    {
        if (!_isOpenInstantVideo)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (_isAvailaleEscape && _isAvailaleTab)
                {
                    _gameHUDCanvas.alpha = 0;
                    _inGameMenuCanvas.gameObject.SetActive(true);
                    _inGameMenuCanvas.alpha = 1;

                    _inGameMenuCanvas.GetComponent<InGameMenuManager>().GamePaused();
                    _isAvailaleEscape = false;
                }
                else if (_isAvailaleTab)
                {
                    CloseMenu();
                }
            }

            if (Input.GetKey(KeyCode.Tab))
            {
                if (_isAvailaleEscape)
                {
                    _isAvailaleTab = false;
                    isTabMenuOn = true;
                    _gameHUDCanvas.alpha = 0;
                    _weponeMenuCanvas.gameObject.SetActive(true);
                    _weponeMenuCanvas.alpha = 1;

                    if (_weponeMenuCanvas.GetComponent<UIInstanceHUD>().isFreeToOpen)
                    {
                        _weponeMenuCanvas.GetComponent<UIInstanceHUD>().CallToOpen();
                    }
                }
            }

            if (Input.GetKeyUp(KeyCode.Tab))
            {
                if (_isAvailaleEscape)
                {
                    _weponeMenuCanvas.GetComponent<UIInstanceHUD>().CallToClose();

                    _weponeMenuCanvas.alpha = 0;
                    _weponeMenuCanvas.gameObject.SetActive(false);

                    _gameHUDCanvas.alpha = 1;
                    _isAvailaleTab = true;
                    isTabMenuOn = false;
                }
            }
        }
    }
    public void CloseMenu()
    {
        _inGameMenuCanvas.GetComponent<InGameMenuManager>().GamePaused();

        _inGameMenuCanvas.alpha = 0;
        _inGameMenuCanvas.gameObject.SetActive(false);
        _gameHUDCanvas.alpha = 1;

        _isAvailaleEscape = true;
    }

    public void OpenInstantVideoPlayerHome()
    {
        _isOpenInstantVideo = true;

        _gameHUDCanvas.alpha = 0;

        _instantVideoPlayerCanvas.gameObject.SetActive(true);
        _instantVideoPlayerCanvas.alpha = 1;

        _inGameMenuCanvas.GetComponent<InGameMenuManager>().GamePaused();
    }

    public void CloseInstantVideoPlayerHome()
    {

        _instantVideoPlayerCanvas.alpha = 0;
        _instantVideoPlayerCanvas.gameObject.SetActive(false);

        _gameHUDCanvas.alpha = 1;

        _isOpenInstantVideo = false;
        _inGameMenuCanvas.GetComponent<InGameMenuManager>().GamePaused();
    }

    public void OpenVideoPlayerHome()
    {
        _gameHUDCanvas.alpha = 0;

        _videoPlayerCanvas.gameObject.SetActive(true);
        _videoPlayerCanvas.alpha = 1;

        _isAvailaleEscape = false;
        _inGameMenuCanvas.GetComponent<InGameMenuManager>().GamePaused();

    }

    public void CloseVideoPlayerHome()
    {

        _videoPlayerCanvas.alpha = 0;
        _videoPlayerCanvas.gameObject.SetActive(false);

        _gameHUDCanvas.alpha = 1;

        _isAvailaleEscape = true;
        _inGameMenuCanvas.GetComponent<InGameMenuManager>().GamePaused();

    }
}

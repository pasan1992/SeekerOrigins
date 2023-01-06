using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class UICanvasHandler : MonoBehaviour
{
    [SerializeField] CanvasGroup _gameHUDCanvas;
    [SerializeField] CanvasGroup _inGameMenuCanvas;
    [SerializeField] CanvasGroup _weponeMenuCanvas;
    [SerializeField] CanvasGroup _instantVideoPlayerCanvas;
    [SerializeField] CanvasGroup _videoPlayerCanvas;

    [SerializeField] UIInstanceHUD _uIInstanceHUD;

    public GameObject ActionHud;

    //Testing
    [SerializeField] int _mainType = 0;
    [SerializeField] int _subType = 0;
    //-----

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

    public void setActionHudStatus(bool enabled)
    {
        ActionHud.SetActive(enabled);
    }

    // Update is called once per frame
    void Update()
    {
        if (!_isOpenInstantVideo)
        {
            //if (Input.GetKey(KeyCode.F))
            //{
            //    pressFKey();
            //}

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

    //void pressFKey()
    //{
    //    if (_mainType == 0)
    //    {
    //        _uIInstanceHUD.pistolTypeObj_IGF.GetComponent<Image>().sprite = _uIInstanceHUD.pistolImgList[_subType];
    //    }
    //    else if (_mainType == 1)
    //    {
    //        _uIInstanceHUD.rifleTypeObj_IGF.GetComponent<Image>().sprite = _uIInstanceHUD.rifleImgList[_subType];
    //    }
    //    //InstantChange(_mainType,_subType);
    //}

    //public void InstantChange(int mainType, int subType)
    //{
    //    if (mainType == 0)
    //    {
    //        _uIInstanceHUD.pistolTypeObj_IGF.GetComponent<Image>().sprite = _uIInstanceHUD.pistolImgList[subType];
    //    }
    //    else if (mainType == 1)
    //    {
    //        _uIInstanceHUD.rifleTypeObj_IGF.GetComponent<Image>().sprite = _uIInstanceHUD.rifleImgList[subType];
    //    }
    //}

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

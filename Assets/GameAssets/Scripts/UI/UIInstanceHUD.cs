using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIInstanceHUD : MonoBehaviour
{
    //[System.Serializable]
    //public struct AmmoPack
    //{
    //    public string AmmoType;
    //    public int AmmoCount;
    //}

    //List<AmmoPack> _weaponAmmoList = new List<AmmoPack>();


    [SerializeField] CanvasGroup _mainHUD;
    [SerializeField] GameObject _instanceHUD;

    [SerializeField] PlayerController _player;

    // In game Fields
    [Header("In Game Fields")]

    [SerializeField] GameObject _missileTypeObj_IGF;
    [SerializeField] TMP_Text _missileTypeCount_IGF;

    [SerializeField] GameObject _grenadeTypeObj_IGF;
    [SerializeField] TMP_Text _grenadeTypeCount_IGF;

    [SerializeField] GameObject _healthPackTypeObj_IGF;
    [SerializeField] TMP_Text _healthPackTypeCount_IGF;

    [SerializeField] GameObject _pistolTypeObj_IGF;
    [SerializeField] TMP_Text _pistolTypeCount_IGF;

    [SerializeField] GameObject _rifleTypeObj_IGF;
    [SerializeField] TMP_Text _rifleTypeCount_IGF;

    // Tab Menu Feilds
    [Header("Tab Menu Fields - Base objects")]
    [SerializeField] GameObject _missileTypeBaseObj_TMF;
    [SerializeField] GameObject _grenadeTypeBaseObj_TMF;
    [SerializeField] GameObject _healthPackTypeBaseObj_TMF;
    [SerializeField] GameObject _pistolTypeBaseObj_TMF;
    [SerializeField] GameObject _rifleTypeBaseObj_TMF;

    [Header("Tab Menu Fields - Sub objects")]
    [SerializeField] GameObject _missileTypeObj_TMF_1;
    [SerializeField] TMP_Text _missileTypeCount_TMF_1;
    [SerializeField] GameObject _missileTypeObj_TMF_2;
    [SerializeField] TMP_Text _missileTypeCount_TMF_2;

    [SerializeField] GameObject _grenadeTypeObj_TMF_1;
    [SerializeField] TMP_Text _grenadeTypeCount_TMF_1;
    [SerializeField] GameObject _grenadeTypeObj_TMF_2;
    [SerializeField] TMP_Text _grenadeTypeCount_TMF_2;

    [SerializeField] GameObject _healthPackTypeObj_TMF_1;
    [SerializeField] TMP_Text _healthPackTypeCount_TMF_1;

    [SerializeField] GameObject _pistolTypeObj_TMF_1;
    [SerializeField] TMP_Text _pistolTypeCount_TMF_1;
    [SerializeField] GameObject _pistolTypeObj_TMF_2;
    [SerializeField] TMP_Text _pistolTypeCount_TMF_2;

    [SerializeField] GameObject _rifleTypeObj_TMF_1;
    [SerializeField] TMP_Text _rifleTypeCount_TMF_1;
    [SerializeField] GameObject _rifleTypeObj_TMF_2;
    [SerializeField] TMP_Text _rifleTypeCount_TMF_2;

    [SerializeField] List<Sprite> _grenadesSpriteList = new List<Sprite>();

    [SerializeField] List<Sprite> _pistolsSpriteList = new List<Sprite>();

    List<AgentData.AmmoPack> _weaponAmmoList = new List<AgentData.AmmoPack>();

    AgentData _agentData;

    Vector3 _initialPosition;
    Vector3 _endPosition;

    //RectTransform _initialPosition;
    //RectTransform _endPosition;
    bool _isHUDOpen = false;

    void Awake()
    {
        _initialPosition = _instanceHUD.transform.position;
        _endPosition = new Vector3(_initialPosition.x, 500f, 0f);

        //_initialPosition = _instanceHUD.GetComponent<RectTransform>().localPosition;
        //_endPosition = new Vector3(0, 600f, _TMF_1);

        //_initialPosition = _instanceHUD.GetComponent<RectTransform>();
        //_endPosition.localPosition = new Vector3(0, 600f,_TMF_1);   
    }

    void Start()
    {
        _weaponAmmoList = _player.GetComponent<HumanoidMovingAgent>().AgentData.WeaponAmmo;
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Tab))
        {
            HudOpen();
            UpdateTabMenuUIData();
        }
        if (Input.GetKeyUp(KeyCode.Tab))
        {
            HudClose();
        }

        UpdateInGameUIData();
    }

    void UpdateInGameUIData()
    {
        if (_weaponAmmoList.Count() > 0)
        {
            foreach (var weaponAmmo in _weaponAmmoList)
            {
                print(weaponAmmo.AmmoType);

                if (weaponAmmo.AmmoType == AmmoTypeEnums.Grenade.Regular_Grenade.ToString())
                {
                    _grenadeTypeCount_IGF.text = weaponAmmo.AmmoCount.ToString();
                    _grenadeTypeObj_IGF.transform.GetChild(2).GetComponent<Image>().sprite = _grenadesSpriteList[(int)AmmoTypeEnums.Grenade.Regular_Grenade];
                    _grenadeTypeObj_IGF.SetActive(true);
                }

                else if(weaponAmmo.AmmoType == AmmoTypeEnums.Grenade.Stealth_Grenade.ToString())
                {
                    _grenadeTypeCount_IGF.text = weaponAmmo.AmmoCount.ToString();
                    _grenadeTypeObj_IGF.transform.GetChild(2).GetComponent<Image>().sprite = _grenadesSpriteList[(int)AmmoTypeEnums.Grenade.Stealth_Grenade];
                    _grenadeTypeObj_IGF.SetActive(true);
                }
                else
                {
                    //_grenadeTypeObj_IGF.transform.parent.gameObject.SetActive(false);
                    _grenadeTypeObj_IGF.SetActive(false);
                }

                if (weaponAmmo.AmmoType == AmmoTypeEnums.Ammo.Stun_Ammo.ToString())
                {
                    _pistolTypeCount_IGF.text = weaponAmmo.AmmoCount.ToString();
                    _pistolTypeObj_IGF.transform.GetChild(2).GetComponent<Image>().sprite = _pistolsSpriteList[(int)AmmoTypeEnums.Ammo.Stun_Ammo];
                    _pistolTypeObj_IGF.SetActive(true);
                }
                else if (weaponAmmo.AmmoType == AmmoTypeEnums.Ammo.Tracking_Ammo.ToString())
                {
                    _pistolTypeCount_IGF.text = weaponAmmo.AmmoCount.ToString();
                    _pistolTypeObj_IGF.transform.GetChild(2).GetComponent<Image>().sprite = _pistolsSpriteList[(int)AmmoTypeEnums.Ammo.Tracking_Ammo];
                    _pistolTypeObj_IGF.SetActive(true);
                }
                else
                {
                    _pistolTypeObj_IGF.SetActive(false);
                }
            }
        }
    }

    void UpdateTabMenuUIData()
    {
        //if (_weaponAmmoList.Count() > 0)
        //{
        //    foreach (var weaponAmmo in _weaponAmmoList)
        //    {
        //        print(weaponAmmo.AmmoType);

        //        if (weaponAmmo.AmmoType == AmmoTypeEnums.Grenade.Regular_Grenade.ToString())
        //        {
        //            _grenadeTypeCount_IGF.text = weaponAmmo.AmmoCount.ToString();
        //            _grenadeTypeObj_IGF.transform.GetChild(2).GetComponent<Image>().sprite = _grenadesSpriteList[(int)AmmoTypeEnums.Grenade.Regular_Grenade];
        //            _grenadeTypeObj_IGF.SetActive(true);
        //        }

        //        else if (weaponAmmo.AmmoType == AmmoTypeEnums.Grenade.Stealth_Grenade.ToString())
        //        {
        //            _grenadeTypeCount_IGF.text = weaponAmmo.AmmoCount.ToString();
        //            _grenadeTypeObj_IGF.transform.GetChild(2).GetComponent<Image>().sprite = _grenadesSpriteList[(int)AmmoTypeEnums.Grenade.Stealth_Grenade];
        //            _grenadeTypeObj_IGF.SetActive(true);
        //        }
        //        else
        //        {
        //            //_grenadeTypeObj_IGF.transform.parent.gameObject.SetActive(false);
        //            _grenadeTypeObj_IGF.SetActive(false);
        //            _grenadeTypeBaseObj_TMF.
        //        }
        //    }
        //}
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

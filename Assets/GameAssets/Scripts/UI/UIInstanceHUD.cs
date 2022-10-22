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
    //AgentData _agentData;

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
    [Header("Tab Menu Fields")]
    [SerializeField] Toggle _missileTypeObj_TMF_1;
    [SerializeField] TMP_Text _missileTypeCount_TMF_1;
    [SerializeField] Toggle _missileTypeObj_TMF_2;
    [SerializeField] TMP_Text _missileTypeCount_TMF_2;

    [SerializeField] Toggle _grenadeTypeObj_TMF_1;
    [SerializeField] TMP_Text _grenadeTypeCount_TMF_1;
    [SerializeField] Toggle _grenadeTypeObj_TMF_2;
    [SerializeField] TMP_Text _grenadeTypeCount_TMF_2;    
    [SerializeField] Toggle _grenadeTypeObj_TMF_3;
    [SerializeField] TMP_Text _grenadeTypeCount_TMF_3;

    [SerializeField] Toggle _healthPackTypeObj_TMF_1;
    [SerializeField] TMP_Text _healthPackTypeCount_TMF_1;

    [SerializeField] Toggle _pistolTypeObj_TMF_1;
    [SerializeField] TMP_Text _pistolTypeCount_TMF_1;
    [SerializeField] Toggle _pistolTypeObj_TMF_2;
    [SerializeField] TMP_Text _pistolTypeCount_TMF_2;    
    [SerializeField] Toggle _pistolTypeObj_TMF_3;
    [SerializeField] TMP_Text _pistolTypeCount_TMF_3;

    [SerializeField] Toggle _rifleTypeObj_TMF_1;
    [SerializeField] TMP_Text _rifleTypeCount_TMF_1;
    [SerializeField] Toggle _rifleTypeObj_TMF_2;
    [SerializeField] TMP_Text _rifleTypeCount_TMF_2;    
    [SerializeField] Toggle _rifleTypeObj_TMF_3;
    [SerializeField] TMP_Text _rifleTypeCount_TMF_3;

    [SerializeField] List<Sprite> _missilesSpriteList = new List<Sprite>();
    [SerializeField] List<Sprite> _grenadesSpriteList = new List<Sprite>();
    [SerializeField] List<Sprite> _HealthPackSpriteList = new List<Sprite>();

    [SerializeField] List<Sprite> _pistolsSpriteList = new List<Sprite>();
    [SerializeField] List<Sprite> _riflesSpriteList = new List<Sprite>();

    List<AgentData.AmmoPack> _weaponAmmoList = new List<AgentData.AmmoPack>();

    string _selectedMissile = "";
    string _selectedGrenade = "";
    string _selectedHealthPack = "";
    string _selectedPistolAmmo = "";
    string _selectedRifleAmmo = "";

    Vector3 _initialPosition;
    Vector3 _endPosition;

    bool _isHUDOpen = false;

    void Start()
    {
        _weaponAmmoList = _player.GetComponent<HumanoidMovingAgent>().AgentData.WeaponAmmo;

        //TODO: to delete
        SetDefaultItems(
            AmmoTypeEnums.Missile.DroneBusters_Missile.ToString(), 
            AmmoTypeEnums.Grenade.Regular_Grenade.ToString(),
            AmmoTypeEnums.HealthPack.Regular_HealthPack.ToString(),
            AmmoTypeEnums.PistolAmmo.Regular_PistolAmmo.ToString(),
            AmmoTypeEnums.RifleAmmo.Regular_RifleAmmo.ToString()
            );
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Tab))
        {
            HudOpen();
            UpdateInGameAndTabUIData();
            SetTabMenuUIData(); 
        }
        if (Input.GetKeyUp(KeyCode.Tab))
        {
            HudClose();
            UpdateInGameAndTabUIData();
        }
    }
    //TODO: to call Interface
    public void SetDefaultItems(string missile,string grenade, string healthPack, string pistol, string rifle)
    {
        _selectedMissile = missile;
        _selectedGrenade = grenade;
        _selectedHealthPack = healthPack;
        _selectedPistolAmmo = pistol;
        _selectedRifleAmmo = rifle;
    }

    void UpdateInGameAndTabUIData()
    {
        if (_weaponAmmoList.Count() > 0)
        {
            foreach (var weaponAmmo in _weaponAmmoList)
            {
                #region Missile
                if (weaponAmmo.AmmoType == AmmoTypeEnums.Missile.DroneBusters_Missile.ToString() || weaponAmmo.AmmoType == AmmoTypeEnums.Missile.MiniNuke_Missile.ToString())
                {
                    if (weaponAmmo.AmmoType == AmmoTypeEnums.Missile.DroneBusters_Missile.ToString())
                    {
                        _missileTypeCount_TMF_1.text = weaponAmmo.AmmoCount.ToString();

                        if (weaponAmmo.AmmoCount <= 0)
                        {
                            _missileTypeObj_TMF_1.interactable = false;
                        }
                        else
                        {
                            _missileTypeObj_TMF_1.interactable = true;
                        }

                        if (weaponAmmo.AmmoType == _selectedMissile)
                        {
                           // _missileTypeCount_IGF.text = weaponAmmo.AmmoCount.ToString();
                            _missileTypeObj_IGF.GetComponent<Image>().sprite = _missilesSpriteList[(int)AmmoTypeEnums.Missile.DroneBusters_Missile];
                        }
                    }

                    else if (weaponAmmo.AmmoType == AmmoTypeEnums.Missile.MiniNuke_Missile.ToString())
                    {
                        _missileTypeCount_TMF_2.text = weaponAmmo.AmmoCount.ToString();

                        if (weaponAmmo.AmmoCount <= 0)
                        {
                            _missileTypeObj_TMF_2.interactable = false;
                        }
                        else
                        {
                            _missileTypeObj_TMF_2.interactable = true;
                        }

                        if (weaponAmmo.AmmoType == _selectedMissile)
                        {
                            //_missileTypeCount_IGF.text = weaponAmmo.AmmoCount.ToString();
                            _missileTypeObj_IGF.GetComponent<Image>().sprite = _missilesSpriteList[(int)AmmoTypeEnums.Missile.MiniNuke_Missile];
                            //_grenadeTypeObj_IGF.transform.GetChild(2).GetComponent<Image>().sprite = _grenadesSpriteList[(int)AmmoTypeEnums.Grenade.EMP_Grenade];
                        }
                    }

                    else
                    {
                        //_missileTypeCount_IGF.text = weaponAmmo.AmmoCount.ToString();
                        _missileTypeObj_IGF.GetComponent<Image>().sprite = _missilesSpriteList[(int)AmmoTypeEnums.Grenade.Regular_Grenade];
                    }
                }
                #endregion

                #region Injection
                else if (weaponAmmo.AmmoType == AmmoTypeEnums.Grenade.Regular_Grenade.ToString() || weaponAmmo.AmmoType == AmmoTypeEnums.Grenade.EMP_Grenade.ToString() || weaponAmmo.AmmoType == AmmoTypeEnums.Grenade.ProximityTrap_Grenade.ToString())
                {
                    if (weaponAmmo.AmmoType == AmmoTypeEnums.Grenade.Regular_Grenade.ToString())
                    {
                        _grenadeTypeCount_TMF_1.text = weaponAmmo.AmmoCount.ToString();

                        if (weaponAmmo.AmmoCount <= 0)
                        {
                            _grenadeTypeObj_TMF_1.interactable = false;
                        }
                        else
                        {
                            _grenadeTypeObj_TMF_1.interactable = true;
                        }

                        if (weaponAmmo.AmmoType == _selectedGrenade)
                        {
                            //_grenadeTypeCount_IGF.text = weaponAmmo.AmmoCount.ToString();
                            _grenadeTypeObj_IGF.GetComponent<Image>().sprite = _grenadesSpriteList[(int)AmmoTypeEnums.Grenade.Regular_Grenade];
                        }
                    }

                    else if (weaponAmmo.AmmoType == AmmoTypeEnums.Grenade.EMP_Grenade.ToString())
                    {
                        _grenadeTypeCount_TMF_2.text = weaponAmmo.AmmoCount.ToString();

                        if (weaponAmmo.AmmoCount <= 0)
                        {
                            _grenadeTypeObj_TMF_2.interactable = false;
                        }
                        else
                        {
                            _grenadeTypeObj_TMF_2.interactable = true;
                        }

                        if (weaponAmmo.AmmoType == _selectedGrenade)
                        {
                            //_grenadeTypeCount_IGF.text = weaponAmmo.AmmoCount.ToString();
                            _grenadeTypeObj_IGF.GetComponent<Image>().sprite = _grenadesSpriteList[(int)AmmoTypeEnums.Grenade.EMP_Grenade];
                        }
                    }

                    else if (weaponAmmo.AmmoType == AmmoTypeEnums.Grenade.ProximityTrap_Grenade.ToString())
                    {
                        _grenadeTypeCount_TMF_3.text = weaponAmmo.AmmoCount.ToString();

                        if (weaponAmmo.AmmoCount <= 0)
                        {
                            _grenadeTypeObj_TMF_3.interactable = false;
                        }
                        else
                        {
                            _grenadeTypeObj_TMF_3.interactable = true;
                        }

                        if (weaponAmmo.AmmoType == _selectedGrenade)
                        {
                            //_grenadeTypeCount_IGF.text = weaponAmmo.AmmoCount.ToString();
                            _grenadeTypeObj_IGF.GetComponent<Image>().sprite = _grenadesSpriteList[(int)AmmoTypeEnums.Grenade.ProximityTrap_Grenade];
                        }
                    }

                    else
                    {
                        //_grenadeTypeCount_IGF.text = weaponAmmo.AmmoCount.ToString();
                        _grenadeTypeObj_IGF.GetComponent<Image>().sprite = _grenadesSpriteList[(int)AmmoTypeEnums.Grenade.Regular_Grenade];
                    }
                }
                #endregion

                #region Pistol
                else if (weaponAmmo.AmmoType == AmmoTypeEnums.PistolAmmo.Regular_PistolAmmo.ToString() || weaponAmmo.AmmoType == AmmoTypeEnums.PistolAmmo.Energy_PistolAmmo.ToString() || weaponAmmo.AmmoType == AmmoTypeEnums.PistolAmmo.Charge_PistolAmmo.ToString())
                {
                    if (weaponAmmo.AmmoType == AmmoTypeEnums.PistolAmmo.Regular_PistolAmmo.ToString())
                    {
                        _pistolTypeCount_TMF_1.text = weaponAmmo.AmmoCount.ToString();

                        if (weaponAmmo.AmmoCount <= 0)
                        {
                            _pistolTypeObj_TMF_1.interactable = false;
                        }
                        else
                        {
                            _pistolTypeObj_TMF_1.interactable = true;
                        }

                        if (weaponAmmo.AmmoType == _selectedPistolAmmo)
                        {
                            _pistolTypeObj_IGF.GetComponent<Image>().sprite = _pistolsSpriteList[(int)AmmoTypeEnums.PistolAmmo.Regular_PistolAmmo];
                        }
                    }

                    else if (weaponAmmo.AmmoType == AmmoTypeEnums.PistolAmmo.Energy_PistolAmmo.ToString())
                    {
                        _pistolTypeCount_TMF_2.text = weaponAmmo.AmmoCount.ToString();

                        if (weaponAmmo.AmmoCount <= 0)
                        {
                            _pistolTypeObj_TMF_2.interactable = false;
                        }
                        else
                        {
                            _pistolTypeObj_TMF_2.interactable = true;
                        }

                        if (weaponAmmo.AmmoType == _selectedPistolAmmo)
                        {
                            _pistolTypeObj_IGF.GetComponent<Image>().sprite = _pistolsSpriteList[(int)AmmoTypeEnums.PistolAmmo.Energy_PistolAmmo];
                        }
                    }

                    else if (weaponAmmo.AmmoType == AmmoTypeEnums.PistolAmmo.Charge_PistolAmmo.ToString())
                    {
                        _pistolTypeCount_TMF_3.text = weaponAmmo.AmmoCount.ToString();

                        if (weaponAmmo.AmmoCount <= 0)
                        {
                            _pistolTypeObj_TMF_3.interactable = false;
                        }
                        else
                        {
                            _pistolTypeObj_TMF_3.interactable = true;
                        }

                        if (weaponAmmo.AmmoType == _selectedPistolAmmo)
                        {
                            //_pistolTypeCount_IGF.text = weaponAmmo.AmmoCount.ToString();
                            _pistolTypeObj_IGF.GetComponent<Image>().sprite = _pistolsSpriteList[(int)AmmoTypeEnums.PistolAmmo.Charge_PistolAmmo];
                        }
                    }
                }
                #endregion

                #region Rifle
                else if (weaponAmmo.AmmoType == AmmoTypeEnums.RifleAmmo.Regular_RifleAmmo.ToString() || weaponAmmo.AmmoType == AmmoTypeEnums.RifleAmmo.Incendiary_RifleAmmo.ToString() || weaponAmmo.AmmoType == AmmoTypeEnums.RifleAmmo.Highcaliber_RifleAmmo.ToString())
                {
                    if (weaponAmmo.AmmoType == AmmoTypeEnums.RifleAmmo.Regular_RifleAmmo.ToString())
                    {
                        _rifleTypeCount_TMF_1.text = weaponAmmo.AmmoCount.ToString();

                        if (weaponAmmo.AmmoCount <= 0)
                        {
                            _rifleTypeObj_TMF_1.interactable = false;
                        }
                        else
                        {
                            _rifleTypeObj_TMF_1.interactable = true;
                        }

                        if (weaponAmmo.AmmoType == _selectedRifleAmmo)
                        {
                            _rifleTypeObj_IGF.GetComponent<Image>().sprite = _riflesSpriteList[(int)AmmoTypeEnums.RifleAmmo.Regular_RifleAmmo];
                        }
                    }

                    else if (weaponAmmo.AmmoType == AmmoTypeEnums.RifleAmmo.Incendiary_RifleAmmo.ToString())
                    {
                        _rifleTypeCount_TMF_2.text = weaponAmmo.AmmoCount.ToString();

                        if (weaponAmmo.AmmoCount <= 0)
                        {
                            _rifleTypeObj_TMF_2.interactable = false;
                        }
                        else
                        {
                            _rifleTypeObj_TMF_2.interactable = true;
                        }

                        if (weaponAmmo.AmmoType == _selectedRifleAmmo)
                        {
                            _rifleTypeObj_IGF.GetComponent<Image>().sprite = _riflesSpriteList[(int)AmmoTypeEnums.RifleAmmo.Incendiary_RifleAmmo];
                        }

                    }
                    else if (weaponAmmo.AmmoType == AmmoTypeEnums.RifleAmmo.Highcaliber_RifleAmmo.ToString())
                    {
                        _rifleTypeCount_TMF_3.text = weaponAmmo.AmmoCount.ToString();

                        if (weaponAmmo.AmmoCount <= 0)
                        {
                            _rifleTypeObj_TMF_3.interactable = false;
                        }
                        else
                        {
                            _rifleTypeObj_TMF_3.interactable = true;
                        }

                        if (weaponAmmo.AmmoType == _selectedRifleAmmo)
                        {
                            //_rifleTypeCount_IGF.text = weaponAmmo.AmmoCount.ToString();
                            _rifleTypeObj_IGF.GetComponent<Image>().sprite = _riflesSpriteList[(int)AmmoTypeEnums.RifleAmmo.Highcaliber_RifleAmmo];
                        }

                    }
                }
                #endregion
            }
        }
    }

    void SetTabMenuUIData()
    {
        //Missile
        if (_selectedMissile == AmmoTypeEnums.Missile.DroneBusters_Missile.ToString())
        {
            _missileTypeObj_TMF_1.isOn = true;
        }
        else if (_selectedMissile == AmmoTypeEnums.Missile.MiniNuke_Missile.ToString())
        {
            _missileTypeObj_TMF_2.isOn = true;
        }

        //Grenade
        if (_selectedGrenade == AmmoTypeEnums.Grenade.Regular_Grenade.ToString())
        {
            _grenadeTypeObj_TMF_1.isOn = true;
        }
        else if (_selectedGrenade == AmmoTypeEnums.Grenade.EMP_Grenade.ToString())
        {
            _grenadeTypeObj_TMF_2.isOn = true;
        }        
        else if (_selectedGrenade == AmmoTypeEnums.Grenade.ProximityTrap_Grenade.ToString())
        {
            _grenadeTypeObj_TMF_3.isOn = true;
        }

        //HealthPack
        if (_selectedHealthPack == AmmoTypeEnums.HealthPack.Regular_HealthPack.ToString())
        {
            _healthPackTypeObj_TMF_1.isOn = true;
        }

        //PistolAmmo
        if (_selectedPistolAmmo == AmmoTypeEnums.PistolAmmo.Regular_PistolAmmo.ToString())
        {
            _pistolTypeObj_TMF_1.isOn = true;
        }
        else if (_selectedPistolAmmo == AmmoTypeEnums.PistolAmmo.Energy_PistolAmmo.ToString())
        {
            _pistolTypeObj_TMF_2.isOn = true;
        }
        if (_selectedPistolAmmo == AmmoTypeEnums.PistolAmmo.Charge_PistolAmmo.ToString())
        {
            _pistolTypeObj_TMF_3.isOn = true;
        }

        //RifleAmmo
        if (_selectedRifleAmmo == AmmoTypeEnums.RifleAmmo.Regular_RifleAmmo.ToString())
        {
            _rifleTypeObj_TMF_1.isOn = true;
        }        
        else if (_selectedRifleAmmo == AmmoTypeEnums.RifleAmmo.Incendiary_RifleAmmo.ToString())
        {
            _rifleTypeObj_TMF_2.isOn = true;
        }
        else if (_selectedRifleAmmo == AmmoTypeEnums.RifleAmmo.Highcaliber_RifleAmmo.ToString())
        {
            _rifleTypeObj_TMF_3.isOn = true;
        }
    }

    public void UpdateMissileType(int type)
    {
        switch (type)
        {
            case 1:
                _selectedMissile = AmmoTypeEnums.Missile.DroneBusters_Missile.ToString();
                break;
            case 2:
                _selectedMissile = AmmoTypeEnums.Missile.MiniNuke_Missile.ToString();
                break;
        }
    }

    public void UpdateGrenadeType(int type)
    {
        switch (type)
        {
            case 1:
                _selectedGrenade = AmmoTypeEnums.Grenade.Regular_Grenade.ToString();
                break;            
            case 2:
                _selectedGrenade = AmmoTypeEnums.Grenade.EMP_Grenade.ToString();
                break;          
            case 3:
                _selectedGrenade = AmmoTypeEnums.Grenade.ProximityTrap_Grenade.ToString();
                break;
        }
    }

    public void UpdateHealthPackType(int type)
    {
        switch (type)
        {
            case 1:
                _selectedHealthPack = AmmoTypeEnums.HealthPack.Regular_HealthPack.ToString();
                break;
        }
    }

    public void UpdatePistolAmmoType(int type)
    {
        switch (type)
        {
            case 1:
                _selectedPistolAmmo = AmmoTypeEnums.PistolAmmo.Regular_PistolAmmo.ToString();
                break;
            case 2:
                _selectedPistolAmmo = AmmoTypeEnums.PistolAmmo.Energy_PistolAmmo.ToString();
                break;            
            case 3:
                _selectedPistolAmmo = AmmoTypeEnums.PistolAmmo.Charge_PistolAmmo.ToString();
                break;
        }
    }

    public void UpdateRifleAmmoType(int type)
    {
        switch (type)
        {
            case 1:
                _selectedRifleAmmo = AmmoTypeEnums.RifleAmmo.Regular_RifleAmmo.ToString();
                break;
            case 2:
                _selectedRifleAmmo = AmmoTypeEnums.RifleAmmo.Incendiary_RifleAmmo.ToString();
                break;
            case 3:
                _selectedRifleAmmo = AmmoTypeEnums.RifleAmmo.Highcaliber_RifleAmmo.ToString();
                break;
        }
    }

    void HudOpen()
    {
        _initialPosition = new Vector3(Screen.width / 2, Screen.height * -1, 0);
        _endPosition = new Vector3(Screen.width / 2, Screen.height / 2, 0f);

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

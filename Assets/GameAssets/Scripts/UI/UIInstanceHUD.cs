using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIInstanceHUD : MonoBehaviour
{
    [SerializeField] CanvasGroup _mainHUD;
    [SerializeField] GameObject _instanceHUD;

    [SerializeField] PlayerController _player;

    // In game Fields
    [Header("In Game Fields (IGF)")]
    [SerializeField] List<Sprite> _pistolImgList;
    [SerializeField] List<Sprite> _rifleImgList;

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
    [Header("Tab Menu Fields (TMF)")]
    [SerializeField] CanvasGroup _missile_TMF_1;
    [SerializeField] CanvasGroup _missile_TMF_2;

    [SerializeField] CanvasGroup _grenade_TMF_1;
    [SerializeField] CanvasGroup _grenade_TMF_2;
    [SerializeField] CanvasGroup _grenade_TMF_3;

    [SerializeField] CanvasGroup _healthPack_TMF_1;

    [SerializeField] CanvasGroup _pistol_TMF_1;
    [SerializeField] CanvasGroup _pistol_TMF_2;
    [SerializeField] CanvasGroup _pistol_TMF_3;

    [SerializeField] CanvasGroup _rifle_TMF_1;
    [SerializeField] CanvasGroup _rifle_TMF_2;
    [SerializeField] CanvasGroup _rifle_TMF_3;

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
    public bool isFreeToOpen;

    private PlayerController m_player;


    void Start()
    {
        _weaponAmmoList = _player.GetComponent<HumanoidMovingAgent>().AgentData.WeaponAmmo;

        //TODO: to delete
        SetDefaultItems(
            AmmoTypeEnums.Missile.DroneBusters_Missile.ToString(), 
            AmmoTypeEnums.Grenade.Regular_Grenade.ToString(),
            AmmoTypeEnums.RegenPack.Regular_HealthPack.ToString(),
            AmmoTypeEnums.PistolAmmo.Regular_PistolAmmo.ToString(),
            AmmoTypeEnums.RifleAmmo.Regular_RifleAmmo.ToString()
            );
        m_player = FindObjectOfType<PlayerController>();

        UpdateInGameAndTabUIData();
        SetTabMenuUIData();
        isFreeToOpen = true;
    }


    public bool isHudOPen()
    {
        return isFreeToOpen;
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Tab))
            {
            if (isFreeToOpen)
            {
                HudOpen();
                UpdateInGameAndTabUIData();
                SetTabMenuUIData();
            }

        }
        if (Input.GetKeyUp(KeyCode.Tab))
        {
            isFreeToOpen = true;
            HudClose();
            UpdateInGameAndTabUIData();
        }
    }

    public void CallToOpen()
    {
        HudOpen();
        UpdateInGameAndTabUIData();
        SetTabMenuUIData();
    }

    public void CallToClose()
    {
        isFreeToOpen = true;

        HudClose();
        UpdateInGameAndTabUIData();
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
        _weaponAmmoList = _player.GetComponent<HumanoidMovingAgent>().AgentData.WeaponAmmo; //1
        //_weaponAmmoList = _player.GetComponent<HumanoidMovingAgent>().AgentData.checkTotalAmmo();

        if (_weaponAmmoList.Count() > 0)
        {
            int i = 0;
            foreach (var weaponAmmo in _weaponAmmoList)
            {
                //TODO Dynamic
                //var textName = "_pistolTypeCount_TMF_" + i;
                //_pistolTypeCount_TMF_1.text = _player.GetComponent<HumanoidMovingAgent>().AgentData.checkTotalAmmo(AmmoTypeEnums.PistolAmmo.Regular_PistolAmmo.ToString()).ToString();

                //if (weaponAmmo.AmmoCount <= 0)
                //{
                //    _pistol_TMF_1.alpha = 0.3f;
                //    _pistol_TMF_1.interactable = false;
                //}
                //else
                //{
                //    _pistol_TMF_1.alpha = 1f;
                //    _pistol_TMF_1.interactable = true;
                //}

                #region Missile
                if (weaponAmmo.AmmoType == AmmoTypeEnums.Missile.DroneBusters_Missile.ToString() || weaponAmmo.AmmoType == AmmoTypeEnums.Missile.MiniNuke_Missile.ToString())
                {
                    if (weaponAmmo.AmmoType == AmmoTypeEnums.Missile.DroneBusters_Missile.ToString())
                    {
                        //_missileTypeCount_TMF_1.text = weaponAmmo.AmmoCount.ToString();
                        _missileTypeCount_TMF_1.text = _player.GetComponent<HumanoidMovingAgent>().AgentData.checkTotalAmmo(AmmoTypeEnums.Missile.DroneBusters_Missile.ToString()).ToString();

                        if (weaponAmmo.AmmoCount <= 0)
                        {
                            _missile_TMF_1.alpha = 0.3f;
                            _missile_TMF_1.interactable = false;
                        }
                        else
                        {
                            _missile_TMF_1.alpha = 1f;
                            _missile_TMF_1.interactable = true;
                        }

                        if (weaponAmmo.AmmoType == _selectedMissile)
                        {
                            _missileTypeObj_IGF.GetComponent<Image>().sprite = _missilesSpriteList[(int)AmmoTypeEnums.Missile.DroneBusters_Missile];
                        }
                    }

                    else if (weaponAmmo.AmmoType == AmmoTypeEnums.Missile.MiniNuke_Missile.ToString())
                    {
                        //_missileTypeCount_TMF_2.text = weaponAmmo.AmmoCount.ToString();
                        _missileTypeCount_TMF_2.text = _player.GetComponent<HumanoidMovingAgent>().AgentData.checkTotalAmmo(AmmoTypeEnums.Missile.MiniNuke_Missile.ToString()).ToString();

                        if (weaponAmmo.AmmoCount <= 0)
                        {
                            _missile_TMF_2.alpha = 0.3f;
                            _missile_TMF_2.interactable = false;
                        }
                        else
                        {
                            _missile_TMF_2.alpha = 1f;
                            _missile_TMF_2.interactable = true;
                        }

                        if (weaponAmmo.AmmoType == _selectedMissile)
                        {
                            _missileTypeObj_IGF.GetComponent<Image>().sprite = _missilesSpriteList[(int)AmmoTypeEnums.Missile.MiniNuke_Missile];
                        }
                    }

                    else
                    {
                        _missileTypeObj_IGF.GetComponent<Image>().sprite = _missilesSpriteList[(int)AmmoTypeEnums.Grenade.Regular_Grenade];
                    }
                }
                #endregion

                #region Grenade
                else if (weaponAmmo.AmmoType == AmmoTypeEnums.Grenade.Regular_Grenade.ToString() || weaponAmmo.AmmoType == AmmoTypeEnums.Grenade.EMP_Grenade.ToString() || weaponAmmo.AmmoType == AmmoTypeEnums.Grenade.ProximityTrap_Grenade.ToString())
                {
                    if (weaponAmmo.AmmoType == AmmoTypeEnums.Grenade.Regular_Grenade.ToString())
                    {
                        //_grenadeTypeCount_TMF_1.text = weaponAmmo.AmmoCount.ToString();
                        _grenadeTypeCount_TMF_1.text = _player.GetComponent<HumanoidMovingAgent>().AgentData.checkTotalAmmo(AmmoTypeEnums.Grenade.Regular_Grenade.ToString()).ToString();

                        if (weaponAmmo.AmmoCount <= 0)
                        {
                            _grenade_TMF_1.alpha = 0.3f;
                            _grenade_TMF_1.interactable = false;
                        }
                        else
                        {
                            _grenade_TMF_1.alpha = 1f;
                            _grenade_TMF_1.interactable = true;
                        }

                        if (weaponAmmo.AmmoType == _selectedGrenade)
                        {
                            _grenadeTypeObj_IGF.GetComponent<Image>().sprite = _grenadesSpriteList[(int)AmmoTypeEnums.Grenade.Regular_Grenade];
                        }
                    }

                    else if (weaponAmmo.AmmoType == AmmoTypeEnums.Grenade.EMP_Grenade.ToString())
                    {
                        //_grenadeTypeCount_TMF_2.text = weaponAmmo.AmmoCount.ToString();
                        _grenadeTypeCount_TMF_2.text = _player.GetComponent<HumanoidMovingAgent>().AgentData.checkTotalAmmo(AmmoTypeEnums.Grenade.EMP_Grenade.ToString()).ToString();

                        if (weaponAmmo.AmmoCount <= 0)
                        {
                            _grenade_TMF_2.alpha = 0.3f;
                            _grenade_TMF_2.interactable = false;
                        }
                        else
                        {
                            _grenade_TMF_2.alpha = 1f;
                            _grenade_TMF_2.interactable = true;
                        }

                        if (weaponAmmo.AmmoType == _selectedGrenade)
                        {
                            _grenadeTypeObj_IGF.GetComponent<Image>().sprite = _grenadesSpriteList[(int)AmmoTypeEnums.Grenade.EMP_Grenade];
                        }
                    }

                    else if (weaponAmmo.AmmoType == AmmoTypeEnums.Grenade.ProximityTrap_Grenade.ToString())
                    {
                        //_grenadeTypeCount_TMF_3.text = weaponAmmo.AmmoCount.ToString();
                        _grenadeTypeCount_TMF_3.text = _player.GetComponent<HumanoidMovingAgent>().AgentData.checkTotalAmmo(AmmoTypeEnums.Grenade.ProximityTrap_Grenade.ToString()).ToString();

                        if (weaponAmmo.AmmoCount <= 0)
                        {
                            _grenade_TMF_3.alpha = 0.3f;
                            _grenade_TMF_3.interactable = false;
                        }
                        else
                        {
                            _grenade_TMF_3.alpha = 1f;
                            _grenade_TMF_3.interactable = true;
                        }

                        if (weaponAmmo.AmmoType == _selectedGrenade)
                        {
                            _grenadeTypeObj_IGF.GetComponent<Image>().sprite = _grenadesSpriteList[(int)AmmoTypeEnums.Grenade.ProximityTrap_Grenade];
                        }
                    }

                    else
                    {
                        _grenadeTypeObj_IGF.GetComponent<Image>().sprite = _grenadesSpriteList[(int)AmmoTypeEnums.Grenade.Regular_Grenade];
                    }
                }
                #endregion

                #region Pistol
                else if (weaponAmmo.AmmoType == AmmoTypeEnums.PistolAmmo.Regular_PistolAmmo.ToString() || weaponAmmo.AmmoType == AmmoTypeEnums.PistolAmmo.Energy_PistolAmmo.ToString() || weaponAmmo.AmmoType == AmmoTypeEnums.PistolAmmo.Charge_PistolAmmo.ToString())
                {
                    if (weaponAmmo.AmmoType == AmmoTypeEnums.PistolAmmo.Regular_PistolAmmo.ToString())
                    {
                        //_pistolTypeCount_TMF_1.text = weaponAmmo.AmmoCount.ToString(); //1
                        _pistolTypeCount_TMF_1.text = _player.GetComponent<HumanoidMovingAgent>().AgentData.checkTotalAmmo(AmmoTypeEnums.PistolAmmo.Regular_PistolAmmo.ToString()).ToString();

                        if (weaponAmmo.AmmoCount <= 0)
                        {
                            _pistol_TMF_1.alpha = 0.3f;
                            _pistol_TMF_1.interactable = false;
                        }
                        else
                        {
                            _pistol_TMF_1.alpha = 1f;
                            _pistol_TMF_1.interactable = true;
                        }

                        if (weaponAmmo.AmmoType == _selectedPistolAmmo)
                        {
                            //_pistolTypeObj_IGF.GetComponent<Image>().sprite = _pistolsSpriteList[(int)AmmoTypeEnums.PistolAmmo.Regular_PistolAmmo];
                            _pistolTypeObj_IGF.GetComponent<Image>().sprite = _pistolImgList[0];
                        }
                    }

                    else if (weaponAmmo.AmmoType == AmmoTypeEnums.PistolAmmo.Energy_PistolAmmo.ToString())
                    {
                        //_pistolTypeCount_TMF_2.text = weaponAmmo.AmmoCount.ToString();
                        _pistolTypeCount_TMF_2.text = _player.GetComponent<HumanoidMovingAgent>().AgentData.checkTotalAmmo(AmmoTypeEnums.PistolAmmo.Energy_PistolAmmo.ToString()).ToString();

                        if (weaponAmmo.AmmoCount <= 0)
                        {
                            _pistol_TMF_2.alpha = 0.3f;
                            _pistol_TMF_2.interactable = false;
                        }
                        else
                        {
                            _pistol_TMF_2.alpha = 1f;
                            _pistol_TMF_2.interactable = true;
                        }

                        if (weaponAmmo.AmmoType == _selectedPistolAmmo)
                        {
                            //_pistolTypeObj_IGF.GetComponent<Image>().sprite = _pistolsSpriteList[(int)AmmoTypeEnums.PistolAmmo.Energy_PistolAmmo];
                            _pistolTypeObj_IGF.GetComponent<Image>().sprite = _pistolImgList[1];

                        }
                    }

                    else if (weaponAmmo.AmmoType == AmmoTypeEnums.PistolAmmo.Charge_PistolAmmo.ToString())
                    {
                        //_pistolTypeCount_TMF_3.text = weaponAmmo.AmmoCount.ToString();
                        _pistolTypeCount_TMF_3.text = _player.GetComponent<HumanoidMovingAgent>().AgentData.checkTotalAmmo(AmmoTypeEnums.PistolAmmo.Charge_PistolAmmo.ToString()).ToString();

                        if (weaponAmmo.AmmoCount <= 0)
                        {
                            _pistol_TMF_3.alpha = 0.3f;
                            _pistol_TMF_3.interactable = false;
                        }
                        else
                        {
                            _pistol_TMF_3.alpha = 1f;
                            _pistol_TMF_3.interactable = true;
                        }

                        if (weaponAmmo.AmmoType == _selectedPistolAmmo)
                        {
                            //_pistolTypeObj_IGF.GetComponent<Image>().sprite = _pistolsSpriteList[(int)AmmoTypeEnums.PistolAmmo.Charge_PistolAmmo];
                            _pistolTypeObj_IGF.GetComponent<Image>().sprite = _pistolImgList[2];

                        }
                    }
                }
                #endregion

                #region Rifle
                else if (weaponAmmo.AmmoType == AmmoTypeEnums.RifleAmmo.Regular_RifleAmmo.ToString() || weaponAmmo.AmmoType == AmmoTypeEnums.RifleAmmo.Energy_RifleAmmo.ToString() || weaponAmmo.AmmoType == AmmoTypeEnums.RifleAmmo.Highcaliber_RifleAmmo.ToString())
                {
                    if (weaponAmmo.AmmoType == AmmoTypeEnums.RifleAmmo.Regular_RifleAmmo.ToString())
                    {
                        //_rifleTypeCount_TMF_1.text = weaponAmmo.AmmoCount.ToString();
                        _rifleTypeCount_TMF_1.text = _player.GetComponent<HumanoidMovingAgent>().AgentData.checkTotalAmmo(AmmoTypeEnums.RifleAmmo.Regular_RifleAmmo.ToString()).ToString();

                        if (weaponAmmo.AmmoCount <= 0)
                        {
                            _rifle_TMF_1.alpha = 0.3f;
                            _rifle_TMF_1.interactable = false;
                        }
                        else
                        {
                            _rifle_TMF_1.alpha = 1f;
                            _rifle_TMF_1.interactable = true;
                        }

                        if (weaponAmmo.AmmoType == _selectedRifleAmmo)
                        {
                            //_rifleTypeObj_IGF.GetComponent<Image>().sprite = _riflesSpriteList[(int)AmmoTypeEnums.RifleAmmo.Regular_RifleAmmo];
                            _rifleTypeObj_IGF.GetComponent<Image>().sprite = _rifleImgList[0];

                        }
                    }

                    else if (weaponAmmo.AmmoType == AmmoTypeEnums.RifleAmmo.Energy_RifleAmmo.ToString())
                    {
                        //_rifleTypeCount_TMF_2.text = weaponAmmo.AmmoCount.ToString();
                        _rifleTypeCount_TMF_2.text = _player.GetComponent<HumanoidMovingAgent>().AgentData.checkTotalAmmo(AmmoTypeEnums.RifleAmmo.Energy_RifleAmmo.ToString()).ToString();

                        if (weaponAmmo.AmmoCount <= 0)
                        {
                            _rifle_TMF_2.alpha = 0.3f;
                            _rifle_TMF_2.interactable = false;
                        }
                        else
                        {
                            _rifle_TMF_2.alpha = 1f;
                            _rifle_TMF_2.interactable = true;
                        }

                        if (weaponAmmo.AmmoType == _selectedRifleAmmo)
                        {
                            //_rifleTypeObj_IGF.GetComponent<Image>().sprite = _riflesSpriteList[(int)AmmoTypeEnums.RifleAmmo.Energy_RifleAmmo];
                            _rifleTypeObj_IGF.GetComponent<Image>().sprite = _rifleImgList[1];

                        }

                    }
                    else if (weaponAmmo.AmmoType == AmmoTypeEnums.RifleAmmo.Highcaliber_RifleAmmo.ToString())
                    {
                        //_rifleTypeCount_TMF_3.text = weaponAmmo.AmmoCount.ToString();
                        _rifleTypeCount_TMF_3.text = _player.GetComponent<HumanoidMovingAgent>().AgentData.checkTotalAmmo(AmmoTypeEnums.RifleAmmo.Highcaliber_RifleAmmo.ToString()).ToString();

                        if (weaponAmmo.AmmoCount <= 0)
                        {
                            _rifle_TMF_2.alpha = 0.3f;
                            _rifle_TMF_2.interactable = false;
                        }
                        else
                        {
                            _rifle_TMF_3.alpha = 1f;
                            _rifle_TMF_3.interactable = true;
                        }

                        if (weaponAmmo.AmmoType == _selectedRifleAmmo)
                        {
                            //_rifleTypeObj_IGF.GetComponent<Image>().sprite = _riflesSpriteList[(int)AmmoTypeEnums.RifleAmmo.Highcaliber_RifleAmmo];
                            _rifleTypeObj_IGF.GetComponent<Image>().sprite = _rifleImgList[2];

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
        if (_selectedHealthPack == AmmoTypeEnums.RegenPack.Regular_HealthPack.ToString())
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
        else if (_selectedRifleAmmo == AmmoTypeEnums.RifleAmmo.Energy_RifleAmmo.ToString())
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
        HudCloseInstanly();
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
        HudCloseInstanly();
    }

    public void UpdateHealthPackType(int type)
    {
        switch (type)
        {
            case 1:
                _selectedHealthPack = AmmoTypeEnums.RegenPack.Regular_HealthPack.ToString();
                break;
        }
        HudCloseInstanly();
    }

    public void UpdatePistolAmmoType(int type)
    {
        var previous_ammo = _selectedPistolAmmo;
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
        if(previous_ammo != _selectedPistolAmmo)
        {
            m_player.SwitchAmmoType(AmmoTypeEnums.WeaponTypes.Pistol, _selectedPistolAmmo);
        }

        HudCloseInstanly();
    }

    public void UpdateRifleAmmoType(int type)
    {       
        var previous_ammo = _selectedRifleAmmo;    
        switch (type)
        {
            case 1:
                _selectedRifleAmmo = AmmoTypeEnums.RifleAmmo.Regular_RifleAmmo.ToString();
                break;
            case 2:
                _selectedRifleAmmo = AmmoTypeEnums.RifleAmmo.Energy_RifleAmmo.ToString();
                break;
            case 3:
                _selectedRifleAmmo = AmmoTypeEnums.RifleAmmo.Highcaliber_RifleAmmo.ToString();
                break;
        }
        if(previous_ammo != _selectedRifleAmmo)
        {
            m_player.SwitchAmmoType(AmmoTypeEnums.WeaponTypes.Rifle, _selectedRifleAmmo);
        }
        
        HudCloseInstanly();
    }

    void HudOpen()
    {
        _initialPosition = new Vector3(Screen.width / 2, Screen.height * -1, 0);
        _endPosition = new Vector3(Screen.width / 2, Screen.height / 2, 0f);

        if (!_isHUDOpen)
        {
            _isHUDOpen = true;
            //_mainHUD.alpha = 0;
            //_instanceHUD.SetActive(true);
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
                .setIgnoreTimeScale(true).setOnComplete((valu) =>
                {
                    //_instanceHUD.SetActive(false);
                    //_mainHUD.alpha = 1;
                    _isHUDOpen = false;
                    Time.timeScale = 1f;
                });

        }
    }
       
    void HudCloseInstanly()
    {
        isFreeToOpen = false;
        HudClose();
        UpdateInGameAndTabUIData();
    }
}

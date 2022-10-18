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
    //[Header("Tab Menu Fields - Base objects")]
    //[SerializeField] GameObject _missileTypeBaseObj_TMF;
    //[SerializeField] GameObject _grenadeTypeBaseObj_TMF;
    //[SerializeField] GameObject _healthPackTypeBaseObj_TMF;
    //[SerializeField] GameObject _pistolTypeBaseObj_TMF;
    //[SerializeField] GameObject _rifleTypeBaseObj_TMF;

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

    [SerializeField] List<Sprite> _grenadesSpriteList = new List<Sprite>();

    [SerializeField] List<Sprite> _pistolsSpriteList = new List<Sprite>();

    List<AgentData.AmmoPack> _weaponAmmoList = new List<AgentData.AmmoPack>();

    string _dfMissile = AmmoTypeEnums.Missile.DroneBusters_Missile.ToString();
    string _dfGrenade = AmmoTypeEnums.Grenade.Regular_Grenade.ToString();
    string _dfHealthPack = "Regular_HealthPack";
    string _dfPistolAmmo = AmmoTypeEnums.PistolAmmo.Regular_PistolAmmo.ToString();
    string _dfRifleAmmo = AmmoTypeEnums.RifleAmmo.Regular_RifleAmmo.ToString();

    string _selectedMissile = "";
    string _selectedGrenade = "";
    string _selectedHealthPack = "";
    string _selectedPistolAmmo = "";
    string _selectedRifleAmmo = "";

    AgentData _agentData;

    Vector3 _initialPosition;
    Vector3 _endPosition;

    //RectTransform _initialPosition;
    //RectTransform _endPosition;
    bool _isHUDOpen = false;

    //void Awake()
    //{

    //}

    void Start()
    {
        //_weaponAmmoList = _player.GetComponent<HumanoidMovingAgent>().AgentData.WeaponAmmo;
        _selectedMissile = _dfMissile;
        _selectedGrenade = _dfGrenade;
        _selectedHealthPack = _dfHealthPack;
        _selectedPistolAmmo = _dfPistolAmmo;
        _selectedRifleAmmo = _dfRifleAmmo;

    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Tab))
        {
            HudOpen();
            SetTabMenuUIData();
        }
        if (Input.GetKeyUp(KeyCode.Tab))
        {
            HudClose();
            UpdateInGameUIData(); //To Chenge

        }
    }

    void UpdateInGameUIData()
    {
        _weaponAmmoList = _player.GetComponent<HumanoidMovingAgent>().AgentData.WeaponAmmo;

        if (_weaponAmmoList.Count() > 0)
        {
            foreach (var weaponAmmo in _weaponAmmoList)
            {
                //print(weaponAmmo.AmmoType);

                if (weaponAmmo.AmmoType == AmmoTypeEnums.Grenade.Regular_Grenade.ToString() || weaponAmmo.AmmoType == AmmoTypeEnums.Grenade.EMP_Grenade.ToString() || weaponAmmo.AmmoType == AmmoTypeEnums.Grenade.ProximityTrap_Grenade.ToString())
                {
                    if (weaponAmmo.AmmoType == AmmoTypeEnums.Grenade.Regular_Grenade.ToString())
                    {

                        _grenadeTypeObj_TMF_1.interactable = true;
                        _grenadeTypeCount_TMF_1.text = weaponAmmo.AmmoCount.ToString();

                        if (weaponAmmo.AmmoType == _selectedGrenade)
                        {
                            print("IF Regular_Grenade");

                            _grenadeTypeCount_IGF.text = weaponAmmo.AmmoCount.ToString();
                            _grenadeTypeObj_IGF.GetComponent<Image>().sprite = _grenadesSpriteList[(int)AmmoTypeEnums.Grenade.Regular_Grenade];
                            //_grenadeTypeObj_IGF.transform.GetChild(2).GetComponent<Image>().sprite = _grenadesSpriteList[(int)AmmoTypeEnums.Grenade.Regular_Grenade];
                            //_grenadeTypeObj_IGF.SetActive(true);
                        }
                    }

                    else if (weaponAmmo.AmmoType == AmmoTypeEnums.Grenade.EMP_Grenade.ToString())
                    {

                        _grenadeTypeObj_TMF_2.interactable = true;
                        _grenadeTypeCount_TMF_2.text = weaponAmmo.AmmoCount.ToString();

                        if (weaponAmmo.AmmoType == _selectedGrenade)
                        {
                            print("ELSE IF EMP_Grenade");

                            _grenadeTypeCount_IGF.text = weaponAmmo.AmmoCount.ToString();
                            _grenadeTypeObj_IGF.GetComponent<Image>().sprite = _grenadesSpriteList[(int)AmmoTypeEnums.Grenade.EMP_Grenade];

                            //_grenadeTypeObj_IGF.transform.GetChild(2).GetComponent<Image>().sprite = _grenadesSpriteList[(int)AmmoTypeEnums.Grenade.EMP_Grenade];
                            //_grenadeTypeObj_IGF.SetActive(true);
                        }
                    }

                    else if (weaponAmmo.AmmoType == AmmoTypeEnums.Grenade.ProximityTrap_Grenade.ToString())
                    {

                        _grenadeTypeObj_TMF_3.interactable = true;
                        _grenadeTypeCount_TMF_3.text = weaponAmmo.AmmoCount.ToString();

                        if (weaponAmmo.AmmoType == _selectedGrenade)
                        {
                            print("ELSE IF ProximityTrap_Grenade");

                            _grenadeTypeCount_IGF.text = weaponAmmo.AmmoCount.ToString();
                            _grenadeTypeObj_IGF.GetComponent<Image>().sprite = _grenadesSpriteList[(int)AmmoTypeEnums.Grenade.ProximityTrap_Grenade];

                            //_grenadeTypeObj_IGF.transform.GetChild(2).GetComponent<Image>().sprite = _grenadesSpriteList[(int)AmmoTypeEnums.Grenade.ProximityTrap_Grenade];
                            //_grenadeTypeObj_IGF.SetActive(true);
                        }
                    }

                    else
                    {
                        print("ELSE");

                        //if (weaponAmmo.AmmoType == _selectedGrenade)
                        //{
                        _grenadeTypeCount_IGF.text = weaponAmmo.AmmoCount.ToString();
                        _grenadeTypeObj_IGF.GetComponent<Image>().sprite = _grenadesSpriteList[(int)AmmoTypeEnums.Grenade.Regular_Grenade];

                        //_grenadeTypeObj_IGF.transform.GetChild(2).GetComponent<Image>().sprite = _grenadesSpriteList[(int)AmmoTypeEnums.Grenade.Regular_Grenade];
                        //    _grenadeTypeObj_IGF.SetActive(true);
                        //}
                        //else
                        //{
                        //     _grenadeTypeObj_IGF.SetActive(false);
                        //}
                    }
                }

                else if (weaponAmmo.AmmoType == AmmoTypeEnums.PistolAmmo.Regular_PistolAmmo.ToString() || weaponAmmo.AmmoType == AmmoTypeEnums.PistolAmmo.Energy_PistolAmmo.ToString() || weaponAmmo.AmmoType == AmmoTypeEnums.PistolAmmo.Charge_PistolAmmo.ToString())
                {
                    if (weaponAmmo.AmmoType == AmmoTypeEnums.PistolAmmo.Regular_PistolAmmo.ToString())
                    {
                        _pistolTypeObj_TMF_1.interactable = true;
                        _pistolTypeCount_TMF_1.text = weaponAmmo.AmmoCount.ToString();

                        if (weaponAmmo.AmmoType == _selectedPistolAmmo)
                        {
                            _pistolTypeCount_IGF.text = weaponAmmo.AmmoCount.ToString();
                            _pistolTypeObj_IGF.GetComponent<Image>().sprite = _pistolsSpriteList[(int)AmmoTypeEnums.PistolAmmo.Regular_PistolAmmo];
                        }

                        //_pistolTypeObj_IGF.transform.GetChild(2).GetComponent<Image>().sprite = _pistolsSpriteList[(int)AmmoTypeEnums.PistolAmmo.Regular_PistolAmmo];
                        //_pistolTypeObj_IGF.SetActive(true);


                    }
                    else if (weaponAmmo.AmmoType == AmmoTypeEnums.PistolAmmo.Energy_PistolAmmo.ToString())
                    {
                        _pistolTypeObj_TMF_2.interactable = true;
                        _pistolTypeCount_TMF_2.text = weaponAmmo.AmmoCount.ToString();

                        if (weaponAmmo.AmmoType == _selectedPistolAmmo)
                        {
                            _pistolTypeCount_IGF.text = weaponAmmo.AmmoCount.ToString();
                            _pistolTypeObj_IGF.GetComponent<Image>().sprite = _pistolsSpriteList[(int)AmmoTypeEnums.PistolAmmo.Energy_PistolAmmo];
                        }



                        //_pistolTypeObj_IGF.transform.GetChild(2).GetComponent<Image>().sprite = _pistolsSpriteList[(int)AmmoTypeEnums.PistolAmmo.Energy_PistolAmmo];
                        //_pistolTypeObj_IGF.SetActive(true);

                    }
                    else if (weaponAmmo.AmmoType == AmmoTypeEnums.PistolAmmo.Charge_PistolAmmo.ToString())
                    {
                        if (weaponAmmo.AmmoType == _selectedPistolAmmo)
                        {
                            _pistolTypeCount_IGF.text = weaponAmmo.AmmoCount.ToString();
                            _pistolTypeObj_IGF.GetComponent<Image>().sprite = _pistolsSpriteList[(int)AmmoTypeEnums.PistolAmmo.Energy_PistolAmmo];
                        }


                        //_pistolTypeObj_IGF.transform.GetChild(2).GetComponent<Image>().sprite = _pistolsSpriteList[(int)AmmoTypeEnums.PistolAmmo.Charge_PistolAmmo];
                        //_pistolTypeObj_IGF.SetActive(true);


                    }
                    else
                    {
                        _pistolTypeObj_IGF.SetActive(false);
                    }
                }
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
                _selectedHealthPack = _dfHealthPack;
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    // Start is called before the first frame update
    public FloatingInfoText lootText;
    public Transform playerTransfrom;
    public Text ammo_count;

    private Interactable m_currentInteractable;
    private PlayerController m_player;
    private HumanoidMovingAgent m_movingAgent;

    public ItemStat ItemStatUI;

    [SerializeField] public Image _pistolImg;
    [SerializeField] public Image _rifleImg;
    [SerializeField] public Image _missileImg;
    [SerializeField] public Image _grenadeImg;
    [SerializeField] public Image _injectionImg;
    [SerializeField] public TMP_Text _pistolCountTxt;
    [SerializeField] public TMP_Text _rifleCountTxt;
    [SerializeField] public TMP_Text _missileCountTxt;
    [SerializeField] public TMP_Text _grenedCountTxt;
    [SerializeField] public TMP_Text _injectionCountTxt;

    [SerializeField] public TMP_Text _gameOverTxt;

    [SerializeField] public Image _healthBarImg;
    [SerializeField] public Image _regenBarImg;
    [SerializeField] public Image _bulletsBarImg;
    [SerializeField] public TMP_Text _healthCountTxt;
    [SerializeField] public TMP_Text _sheildCountTxt;
    [SerializeField] public TMP_Text _bulletCountTxt;

    [SerializeField] TMP_Text _magazinCountTxt;
    [SerializeField] Image _weaponImg;
    [SerializeField] Sprite[] _weaponList;

    [SerializeField] TMP_Text _pickupMsgTxt;
    //[SerializeField] TMP_Text[] _pickupMsgTxtList;

    [SerializeField] Transform _pickupMsgTxtParant;
    [SerializeField] GameObject _pickupMsgPanel;
    [SerializeField] GameObject _ammoBox;

    [SerializeField] GameObject _activeWeapon;
    public AgentData _agentData;

    //UI Animation variable
    float _healthTweenTime = 0.5f;
    float _sheildTweenTime = 0.5f;

    float _prevHealth = 1;
    float _prevSheild = 1;

    Coroutine pickupCorouting = null;

    bool _havingSheald = true;

    public void Start()
    {
        m_player = GameObject.FindObjectOfType<PlayerController>();
        m_movingAgent = m_player.GetComponent<HumanoidMovingAgent>();
        _agentData = m_movingAgent.GetAgentData();

        m_movingAgent.setOnDamagedCallback(onHealthChange);
        m_movingAgent.setOnAmmoPickupCallback(OnAmmoPickupEvent);
        m_movingAgent.setOnHealCallback(onHealthChange);

        
        update_health();
    }
    // Update is called once per frame
    void onHealthChange()
    {
        update_health();
    }

    public void OnAmmoPickupEvent(AmmoPack ammoPack)
    {
        _pickupMsgPanel.SetActive(true);
        StartCoroutine(CreateMsges(ammoPack));
    }

    IEnumerator CreateMsges(AmmoPack ammoPack)
    {
        List<TMP_Text> _pickupMsgTxtQueue = new List<TMP_Text>();
        var ammo_data = ammoPack.AmmoPackData;

        foreach (var ammo in ammo_data)
        {
            Debug.Log("adding");
            CreatePickupMsg(ammo.AmmoType + " X" + ammo.AmmoCount,_pickupMsgTxtQueue);
            yield return new WaitForSeconds(0.5f);
        }

        if (ammoPack.GrenadeCount > 0)
        {
            //
            var msg = "Grenade X" + ammoPack.GrenadeCount;
            CreatePickupMsg(msg,_pickupMsgTxtQueue);
            yield return new WaitForSeconds(0.5f);
        }

        if (ammoPack.HealthInjectionCount > 0)
        {
            
            var msg = "HealthInjection X" + ammoPack.HealthInjectionCount;
            CreatePickupMsg(msg,_pickupMsgTxtQueue);
            yield return new WaitForSeconds(0.5f);
        }


        
        yield return new WaitForSeconds(2f);
        _pickupMsgTxtQueue.Reverse();
        foreach (var msg in _pickupMsgTxtQueue)
        {
            yield return new WaitForSeconds(0.2f);
            LeanTween.scale(msg.gameObject, new Vector3(0.001f, 0f, 0f), 0.2f)
            .setEasePunch();
            yield return new WaitForSeconds(0.1f);
            Destroy(msg.gameObject);
        }
    }

    void CreatePickupMsg(string msg,List<TMP_Text>_pickupMsgTxtQueue)
    {
        var msgLine = Instantiate(_pickupMsgTxt, _pickupMsgTxtParant);
        msgLine.text = msg;

        LeanTween.cancel(msgLine.gameObject);
        msgLine.gameObject.transform.localScale = Vector3.one;
        LeanTween.scale(msgLine.gameObject, new Vector3(0.2f, 0f, 0f) * 2f, 0.2f)
            .setEasePunch();

        _pickupMsgTxtQueue.Add(msgLine);
    }

    void Update()
    {
        updateLootText();
        update_ammo_count();
        updateAmmo();
    }

    void updateAmmo()
    {
        //if (m_movingAgent.getCurrentWeapon() == null)
        //{
        //    _activeWeapon.SetActive(false);
        //}
        //else
        //{
        //    _activeWeapon.SetActive(true);
        //}

        if (m_movingAgent.getCurrentWeapon() != null)
        {
            _bulletCountTxt.text = m_movingAgent.getCurrentWeaponAmmoCount().ToString() + " / " + m_movingAgent.getCurrentWeaponMagazineSize().ToString();
            _bulletsBarImg.fillAmount = ((float)m_movingAgent.getCurrentWeaponAmmoCount()) / ((float)m_movingAgent.getCurrentWeaponMagazineSize());

            //_magazinCountTxt.text = (m_movingAgent.getCurrentWeaponMagazineSize() * m_movingAgent.getCurrentWeaponAmmoCount()).ToString(); //TODO

            //var weaponType = m_movingAgent.getCurrentWeaponType(); 

            //if (weaponType.ToString() == "primary")
            //{
            //    _weaponImg.sprite = _weaponList[0];
            //}
            //else if (weaponType.ToString() == "secondary")
            //{
            //    _weaponImg.sprite = _weaponList[1];
            //}
        }
        else
        {
            _bulletCountTxt.text = "0";
            _bulletsBarImg.fillAmount = 0;
        }

        // Set Injection Count
        _injectionCountTxt.text = m_movingAgent.GetAgentData().HealInjectionCount.ToString();
        if (m_movingAgent.GetAgentData().HealInjectionCount <= 0)
        {
            _injectionImg.color = Color.black;
        }
        else
        {
            _injectionImg.color = Color.white;
        }

        // Set Missile Count
        int missile_count = m_movingAgent.GetAgentData().checkAvailableAmmo("Missile");
        _missileCountTxt.text = missile_count.ToString();

        if(missile_count == 0)
        {
            _missileImg.color = Color.black;
        }
        else
        {
            _missileImg.color = Color.white;
        }

        // Set Grened Count
        _grenedCountTxt.text = m_movingAgent.GetGrenadeCount().ToString();

        if (m_movingAgent.GetGrenadeCount() <= 0)
        {
            _grenadeImg.color = Color.black;
        }
        else
        {
            _grenadeImg.color = Color.white;
        }

        // Set Primary Weapon Count
        if (m_movingAgent.GetPrimaryWeapon() != null)
        {
            var val = m_movingAgent.GetAgentData().checkAvailableAmmo(m_movingAgent.GetPrimaryWeapon().m_weaponAmmunitionName);
            _rifleCountTxt.text = val.ToString();
        }
        else
        {
            _rifleCountTxt.text = "0";
        }
        if (m_movingAgent.GetPrimaryWeapon() == null)
        {
            _rifleImg.color = Color.black;
        }
        else if (m_movingAgent.GetPrimaryWeapon() == m_movingAgent.getCurrentWeapon())
        {
            _rifleImg.color = Color.yellow;
        }
        else
        {
            _rifleImg.color = Color.white;
        }

        // Set Secondary Weapon Count
        if (m_movingAgent.GetSecondaryWeapon() != null)
        {
            var val = m_movingAgent.GetAgentData().checkAvailableAmmo(m_movingAgent.GetSecondaryWeapon().m_weaponAmmunitionName);
            _pistolCountTxt.text = val.ToString();
        }
        else
        {
            _pistolCountTxt.text = "0";
        }

        if (m_movingAgent.GetSecondaryWeapon() == null)
        {
            _pistolImg.color = Color.black;
        }
        else if (m_movingAgent.GetSecondaryWeapon() == m_movingAgent.getCurrentWeapon())
        {
            _pistolImg.color = Color.yellow;
        }
        else
        {
            _pistolImg.color = Color.white;
        }
    }

    private void update_health()
    {
         if (_agentData.Sheild > 0)
         {
             _havingSheald = true;
             _sheildCountTxt.text = ((int)_agentData.Sheild).ToString();
             AnimSheild();
         }
         else if(_agentData.Sheild == 0 && _havingSheald)
         {
             _sheildCountTxt.text = ((int)_agentData.Sheild).ToString();
             AnimSheild();
             _havingSheald = false;
         }
         else
         {
             _healthCountTxt.text = ((int)_agentData.Health).ToString();
             AnimHealth();
         }
        if (_agentData.Health <= 0)
        {
            _gameOverTxt.gameObject.SetActive(true);
        }
        _sheildCountTxt.text = ((int)_agentData.Sheild).ToString();
        _healthCountTxt.text = ((int)_agentData.Health).ToString();
        //AnimHealth();
        //AnimSheild();
    }

    public void AnimHealth()
    {
        LeanTween.cancel(_healthCountTxt.gameObject);
        _healthCountTxt.gameObject.transform.localScale = Vector3.one;
        LeanTween.scale(_healthCountTxt.gameObject, new Vector3(0.7f, 0.7f, 0.7f) * 2f, 2)
            .setEasePunch();

        //LeanTween.value(_healthCountTxt.gameObject, 0, 1, 3)
        //    .setEasePunch()
        //    .setOnUpdate(setHealthTxt);

        var currentHealth = _agentData.Health / _agentData.MaxHealth;

        if (_prevHealth > currentHealth)
        {
            LeanTween.value(_healthBarImg.gameObject, _prevHealth, currentHealth, _healthTweenTime)
            .setEaseInElastic()
            .setOnUpdate((value) =>
            {
                _healthBarImg.fillAmount = currentHealth == 0 ? 0 : value;
                _prevHealth = currentHealth;
            });
        }
        else if (_prevHealth <= currentHealth)
        {
            LeanTween.value(_healthBarImg.gameObject, _prevHealth, currentHealth, _healthTweenTime)
            .setEaseInElastic()
            .setOnUpdate((value) =>
            {
                _healthBarImg.fillAmount = currentHealth == 0 ? 0 : value;
                _prevHealth = currentHealth;
            });
        }

        //if (_prevHealth > currentHealth)
        //{
        //    LeanTween.value(_healthBarImg.gameObject, currentHealth, _prevHealth, _healthTweenTime)
        //    .setEaseInElastic()
        //    .setOnUpdate((value) =>
        //    {
        //        _healthBarImg.fillAmount = currentHealth == 0 ? 0 : value;
        //        _prevHealth = currentHealth;
        //    });
        //}
        //else if(_prevHealth <= currentHealth)
        //{
        //    LeanTween.value(_healthBarImg.gameObject, _prevHealth, currentHealth, _healthTweenTime)
        //    .setEaseInElastic()
        //    .setOnUpdate((value) =>
        //    {
        //        _healthBarImg.fillAmount = currentHealth == 0 ? 0 : value;
        //        _prevHealth = currentHealth;
        //    });
        //}
        //.setEasePunch()

        //_healthBarImg.fillAmount = currentHealth == 1 ? 1 : value;

    }

    public void AnimSheild()
    {
        LeanTween.cancel(_sheildCountTxt.gameObject);
        _sheildCountTxt.gameObject.transform.localScale = Vector3.one;
        LeanTween.scale(_sheildCountTxt.gameObject, new Vector3(0.7f, 0.7f, 0.7f) * 2f, 2)
            .setEasePunch();

        var currentSheild = _agentData.Sheild / _agentData.MaxSheild;

        //LeanTween.value(_regenBarImg.gameObject, currentSheild, _prevSheild, _sheildTweenTime)
        //    .setEaseInOutElastic()
        //    .setOnUpdate((value) =>
        //    {
        //        _regenBarImg.fillAmount = currentSheild == 0 ? 0 : value;
        //        _prevSheild = currentSheild;
        //    });

        if (_prevSheild > currentSheild)
        {
            LeanTween.value(_regenBarImg.gameObject, _prevSheild, currentSheild, _sheildTweenTime)
            .setEaseInElastic()
            .setOnUpdate((value) =>
            {
                _regenBarImg.fillAmount = currentSheild == 0 ? 0 : value;
                _prevSheild = currentSheild;
            });
        }
        else if (_prevSheild <= currentSheild)
        {
            LeanTween.value(_regenBarImg.gameObject, _prevSheild, currentSheild, _sheildTweenTime)
            .setEaseInElastic()
            .setOnUpdate((value) =>
            {
                _regenBarImg.fillAmount = currentSheild == 0 ? 0 : value;
                _prevSheild = currentSheild;
            });
        }
    }

    private void update_ammo_count()
    {
        if(ammo_count == null)
        {
            return;
        }

        if (m_movingAgent.getCurrentWeapon() !=null)
        {
            var loaded = m_movingAgent.getCurrentWeaponAmmoCount();

            switch (m_movingAgent.getCurrentWeapon().getWeaponType())
            {
                case Weapon.WEAPONTYPE.primary:
                case Weapon.WEAPONTYPE.secondary:

                    
                    var agent_data = m_movingAgent.GetAgentData();
                    int totalAmmo = agent_data.checkAvailableAmmo(m_movingAgent.getCurrentWeapon().m_weaponAmmunitionName);
                    // agent_data.weaponAmmoCount.TryGetValue(m_movingAgent.getCurrentWeapon().m_weaponAmmunitionName, out totalAmmo);
                    ammo_count.text = loaded.ToString() + "/" + totalAmmo.ToString();

                    break;
                case Weapon.WEAPONTYPE.grenede:
                    ammo_count.text = loaded.ToString();
                    break;
            }
        }
        else
        {
            ammo_count.text = "";
        }
    }

    private void updateLootText()
    {
        if(m_currentInteractable)
        {
            m_currentInteractable.setOutLineState(false);
            ItemStatUI.setVisibility(false);
        }

        m_currentInteractable = AgentItemFinder.findNearItem(playerTransfrom.position);

        if(m_currentInteractable)
        {
            lootText.setInteratableObject(m_currentInteractable);
            m_currentInteractable.setOutLineState(true);
            updateWeaponStat(m_currentInteractable);
        }
        else
        {
            
            lootText.resetText();
        }
    }

    private void updateWeaponStat(Interactable int_object)
    {
        PrimaryWeapon new_primaryWeapon = int_object as PrimaryWeapon;

        if (new_primaryWeapon != null)
        {
            ItemStatUI.setVisibility(true);
            ItemStatUI.setItemName(int_object.name);
            ItemStatUI.setItem(int_object.transform);
            var current_pw = m_movingAgent.GetPrimaryWeapon();
            var old_damage = new_primaryWeapon.damage;
            var old_firerate = new_primaryWeapon.fireRate;
            if(current_pw !=null)
            {   
                old_damage = current_pw.damage;
                old_firerate = current_pw.fireRate;
            }
            ItemStatUI.setDamage(new_primaryWeapon.damage,old_damage);
            ItemStatUI.setFireRate(new_primaryWeapon.fireRate,old_firerate);         
            return;
        }

        SecondaryWeapon new_secondaryWeapon = int_object as SecondaryWeapon;
        if (new_secondaryWeapon != null)
        {
            ItemStatUI.setVisibility(true);
            ItemStatUI.setItem(int_object.transform);
            ItemStatUI.setItemName(int_object.name);
            var current_sw = m_movingAgent.GetSecondaryWeapon();
            var old_damage = new_secondaryWeapon.damage;
            var old_firerate = new_secondaryWeapon.fireRate;
            if(current_sw !=null)
            {   
                old_damage = current_sw.damage;
                old_firerate = current_sw.fireRate;
            }
            ItemStatUI.setDamage(new_secondaryWeapon.damage,old_damage);
            ItemStatUI.setFireRate(new_secondaryWeapon.fireRate,old_firerate);
            return;
        }


    }
}

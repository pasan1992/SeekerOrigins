using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    // Start is called before the first frame update
    public Text lootText;
    public Transform playerTransfrom;
    public TMP_Text ammo_count;

    private Interactable m_currentInteractable;
    private PlayerController m_player;
    private HumanoidMovingAgent m_movingAgent;

    public ItemStat ItemStatUI;

    [SerializeField] Image _InteractImg;
    [SerializeField] TMP_Text _lootText;

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

    //[SerializeField] GameObject _interactImg;
    [SerializeField] GameObject _pickupMsgTxt;
    //[SerializeField] TMP_Text _pickupMsgTxt;
    //[SerializeField] TMP_Text[] _pickupMsgTxtList;

    [SerializeField] Transform _pickupMsgTxtParant;
    [SerializeField] GameObject _pickupMsgPanel;
    private FloatingGameUI m_onPickupMsgPanelUI;
    [SerializeField] GameObject _ammoBox;

    [SerializeField] Transform _taskMsgTxtParant;
    [SerializeField] GameObject _taskMsgPanel;

    [SerializeField] Transform _targetParant;
    [SerializeField] Image _target;

    [SerializeField] GameObject _activeWeapon;
    private AgentData _agentData;

    int _primaryWeaponAmmo;
    int _secondryWeaponAmmo;

    //UI Animation variable
    float _healthTweenTime = 0.5f;
    float _sheildTweenTime = 0.5f;

    float _prevHealth = 1;
    float _prevSheild = 1;

    Coroutine pickupCorouting = null;

    bool _havingSheald = true;

    [System.Serializable]
    public class PickupIcons
    {
        public string PickupName;
        public Sprite PickupImage;
    }
    public List<PickupIcons> _pickupIcons;
    private Dictionary<string,Sprite> _pickupIconDict = new Dictionary<string, Sprite>();

    public void Start()
    {
        m_player = GameObject.FindObjectOfType<PlayerController>();
        m_movingAgent = m_player.GetComponent<HumanoidMovingAgent>();
        _agentData = m_movingAgent.GetAgentData();
        m_onPickupMsgPanelUI = _pickupMsgPanel.GetComponent<FloatingGameUI>();

        m_movingAgent.setOnDamagedCallback(onHealthChange);
        m_movingAgent.setOnAmmoPickupCallback(OnAmmoPickupEvent);
        m_movingAgent.setOnHealCallback(onHealthChange);
        m_movingAgent.setOnWeaponPickupEvent(onWeaponPickupEvent);
        
        update_health();

        foreach(PickupIcons icon in _pickupIcons)
        {
            _pickupIconDict.Add(icon.PickupName,icon.PickupImage);
        }
    }

    void Update()
    {
        updateLootText();
        update_ammo_count();
        updateAmmo();
        //if (target.CompareTag("Target"))
        //{
        //Vector3 mousePos = Input.mousePosition;
        //_targetParant.position = mousePos;
        //}
    }

    public void onWeaponPickupEvent(Interactable obj)
    {
        StartCoroutine(CreateWeaponPickupMsg(obj));
    }

    #region Pickups
    public void OnAmmoPickupEvent(AmmoPack ammoPack)
    {
        m_onPickupMsgPanelUI.target = ammoPack.gameObject.transform;
        _pickupMsgPanel.SetActive(true);
        StartCoroutine(CreateMsges(ammoPack));
    }

    IEnumerator CreateWeaponPickupMsg(Interactable obj)
    {
        List<GameObject> _pickupMsgTxtQueue = new List<GameObject>();
        CreatePickupMsg(obj.name,1,_pickupMsgTxtQueue);
        yield return new WaitForSeconds(2f);       
        yield return deleteMsgQueue(_pickupMsgTxtQueue);
    }

    IEnumerator CreateMsges(AmmoPack ammoPack)
    {
        List<GameObject> _pickupMsgTxtQueue = new List<GameObject>();
        var ammo_data = ammoPack.AmmoPackData;

        foreach (var ammo in ammo_data)
        {
            CreatePickupMsg(ammo.AmmoType,ammo.AmmoCount,_pickupMsgTxtQueue);
            yield return new WaitForSeconds(0.5f);
        }

        if (ammoPack.GrenadeCount > 0)
        {
            //
            //var msg = "Grenade X" + ammoPack.GrenadeCount;
            CreatePickupMsg("Grenade", ammoPack.GrenadeCount,_pickupMsgTxtQueue);
            yield return new WaitForSeconds(0.5f);
        }

        if (ammoPack.HealthInjectionCount > 0)
        {
            
            //var msg = "HealthInjection X" + ammoPack.HealthInjectionCount;
            CreatePickupMsg("HealthInjection",ammoPack.HealthInjectionCount,_pickupMsgTxtQueue);
            yield return new WaitForSeconds(0.5f);
        }


        
        yield return new WaitForSeconds(2f);
        _pickupMsgTxtQueue.Reverse();
        yield return deleteMsgQueue(_pickupMsgTxtQueue);
        _pickupMsgPanel.SetActive(false);

    }

    IEnumerator deleteMsgQueue(List<GameObject> _pickupMsgTxtQueue)
    {
        foreach (var msg in _pickupMsgTxtQueue)
        {
            yield return new WaitForSeconds(0.2f);
            LeanTween.scale(msg.gameObject, new Vector3(0.001f, 0f, 0f), 0.2f)
            .setEasePunch();
            yield return new WaitForSeconds(0.1f);
            Destroy(msg.gameObject);
        }
    }

    void CreatePickupMsg(string item_name,int item_count,List<GameObject> _pickupMsgTxtQueue)
    {
        var msgLine = Instantiate(_pickupMsgTxt, _pickupMsgTxtParant);
        msgLine.GetComponentInChildren<TMP_Text>().text = item_name.ToString() + " X " + item_count.ToString();
        var pickupImage = _pickupIconDict["Default"];
        _pickupIconDict.TryGetValue(item_name,out pickupImage);
        msgLine.GetComponentsInChildren<Image>()[1].sprite = pickupImage;

        LeanTween.cancel(msgLine.gameObject);
        msgLine.gameObject.transform.localScale = Vector3.one;
        LeanTween.scale(msgLine.gameObject, new Vector3(0.2f, 0f, 0f) * 2f, 0.2f)
            .setEasePunch();

        _pickupMsgTxtQueue.Add(msgLine);
    }
    #endregion

    #region Health
    void onHealthChange()
    {
        update_health();
    }

    private void update_health()
    {
         if (_agentData.Sheild > 0)
         {
             _havingSheald = true;
            // _sheildCountTxt.text = ((int)_agentData.Sheild).ToString();
             AnimSheild();
         }
         else if(_agentData.Sheild == 0 && _havingSheald)
         {
            // _sheildCountTxt.text = ((int)_agentData.Sheild).ToString();
             AnimSheild();
             _havingSheald = false;
         }
         else
         {
            // _healthCountTxt.text = ((int)_agentData.Health).ToString();
             AnimHealth();
         }
        if (_agentData.Health <= 0)
        {
            //_gameOverTxt.gameObject.SetActive(true);
        }
        //_sheildCountTxt.text = ((int)_agentData.Sheild).ToString();
       // _healthCountTxt.text = ((int)_agentData.Health).ToString();
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
        _healthCountTxt.text = ((int)_agentData.Health).ToString();

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
                _healthBarImg.fillAmount = currentHealth == 1 ? 1 : value;
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
        _sheildCountTxt.text = ((int)_agentData.Sheild).ToString();
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
                _regenBarImg.fillAmount = currentSheild == 1 ? 1 : value;
                _prevSheild = currentSheild;
            });
        }
    }
    #endregion

    #region Ammo
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
            //_bulletCountTxt.text = m_movingAgent.getCurrentWeaponAmmoCount().ToString() + " / " + m_movingAgent.getCurrentWeaponMagazineSize().ToString();
            //_bulletsBarImg.fillAmount = ((float)m_movingAgent.getCurrentWeaponAmmoCount()) / ((float)m_movingAgent.getCurrentWeaponMagazineSize());
            //_target.fillAmount = ((float)m_movingAgent.getCurrentWeaponAmmoCount()) / ((float)m_movingAgent.getCurrentWeaponMagazineSize());

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
            //_bulletCountTxt.text = "0";
            //_bulletsBarImg.fillAmount = 0;
            _target.fillAmount = 0;
        }

        #region Injection
        int injectionCount = m_movingAgent.GetAgentData().HealInjectionCount;
        _injectionCountTxt.text = injectionCount.ToString();

        if (injectionCount <= 0)
        {
            _injectionImg.transform.GetChild(1).GetComponent<Image>().color = Color.black;
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.H))
            {
                _injectionImg.transform.GetChild(1).GetComponent<Image>().color = Color.yellow;

                LeanTween.cancel(_injectionImg.gameObject);
                LeanTween.scale(_injectionImg.gameObject, new Vector3(0.7f, 0.7f, 0.7f) * 2f, 1)
                .setEasePunch();
            }
            else if (Input.GetKey(KeyCode.H))
            {
                _missileImg.transform.GetChild(1).GetComponent<Image>().color = Color.yellow;
            }
            else
            {
                _injectionImg.transform.GetChild(1).GetComponent<Image>().color = Color.white;
            }
            _injectionImg.gameObject.transform.localScale = Vector3.one;
        }
        #endregion

        #region Missile
        int missile_count = m_movingAgent.GetAgentData().checkAvailableAmmo("Missile");
        _missileCountTxt.text = missile_count.ToString();

        if (missile_count <= 0)
        {
            _missileImg.transform.GetChild(1).GetComponent<Image>().color = Color.black;
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                _missileImg.transform.GetChild(1).GetComponent<Image>().color = Color.yellow;

                LeanTween.cancel(_missileImg.gameObject);
                LeanTween.scale(_missileImg.gameObject, new Vector3(0.7f, 0.7f, 0.7f) * 2f, 1)
                .setEasePunch();
            }
            else if (Input.GetKey(KeyCode.Q))
            {
                _missileImg.transform.GetChild(1).GetComponent<Image>().color = Color.yellow;
            }
            else
            {
                _missileImg.transform.GetChild(1).GetComponent<Image>().color = Color.white;
            }
            _missileImg.gameObject.transform.localScale = Vector3.one;
        }
        #endregion

        #region Grened
        _grenedCountTxt.text = m_movingAgent.GetGrenadeCount().ToString();

        if (m_movingAgent.GetGrenadeCount() <= 0)
        {
            _grenadeImg.transform.GetChild(1).GetComponent<Image>().color = Color.black;
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.G))
            {
                _grenadeImg.transform.GetChild(1).GetComponent<Image>().color = Color.yellow;

                LeanTween.cancel(_grenadeImg.gameObject);
                LeanTween.scale(_grenadeImg.gameObject, new Vector3(0.7f, 0.7f, 0.7f) * 2f, 1)
                .setEasePunch();
            }
            else if (Input.GetKey(KeyCode.G))
            {
                _grenadeImg.transform.GetChild(1).GetComponent<Image>().color = Color.yellow;
            }
            else
            {
                _grenadeImg.transform.GetChild(1).GetComponent<Image>().color = Color.white;
            }
            _grenadeImg.gameObject.transform.localScale = Vector3.one;
        }
        #endregion

        #region Primary Weapon
        if (m_movingAgent.GetPrimaryWeapon() != null)
        {
            _primaryWeaponAmmo = m_movingAgent.GetAgentData().checkAvailableAmmo(m_movingAgent.GetPrimaryWeapon().m_weaponAmmunitionName) + m_movingAgent.getPrimaryWeaponAmmoCount();
            _rifleCountTxt.text = _primaryWeaponAmmo.ToString();

            //LeanTween.cancel(_rifleImg.gameObject);
            //LeanTween.scale(_rifleImg.gameObject, new Vector3(0.7f, 0.7f, 0.7f) * 2f, 0.5f)
            //.setEasePunch();
        }

        LeanTween.cancel(_rifleImg.gameObject);

        if (_primaryWeaponAmmo == 0 || m_movingAgent.GetPrimaryWeapon() == null)
        {
            _rifleImg.transform.GetChild(1).GetComponent<Image>().color = Color.black;
            LeanTween.moveLocalY(_rifleImg.gameObject, 90, 0.1f);
            LeanTween.scale(_rifleImg.gameObject, Vector3.one, 0.3f);
        }
        else
        {
            if (m_movingAgent.GetPrimaryWeapon() == m_movingAgent.getCurrentWeapon())
            {
                _rifleImg.transform.GetChild(1).GetComponent<Image>().color = Color.yellow;
                LeanTween.moveLocalY(_rifleImg.gameObject, 100, 0.1f);
                LeanTween.scale(_rifleImg.gameObject, new Vector3(1.4f, 1.4f, 1f), 0.2f);
            }
            else
            {
                _rifleImg.transform.GetChild(1).GetComponent<Image>().color = Color.white;
                LeanTween.moveLocalY(_rifleImg.gameObject, 90, 0.1f);
                LeanTween.scale(_rifleImg.gameObject, Vector3.one, 0.1f);
            }
        }
        #endregion

        #region Secondary Weapon
        if (m_movingAgent.GetSecondaryWeapon() != null)
        {
            _secondryWeaponAmmo = m_movingAgent.GetAgentData().checkAvailableAmmo(m_movingAgent.GetSecondaryWeapon().m_weaponAmmunitionName) + m_movingAgent.getSecondaryWeaponAmmoCount();
            _pistolCountTxt.text = _secondryWeaponAmmo.ToString();

            //LeanTween.cancel(_pistolImg.gameObject);
            //LeanTween.scale(_pistolImg.gameObject, new Vector3(0.7f, 0.7f, 0.7f) * 2f, 0.5f)
            //.setEasePunch();
        }

        LeanTween.cancel(_pistolImg.gameObject);

        if (_secondryWeaponAmmo == 0 || m_movingAgent.GetSecondaryWeapon() == null)
        {
            _pistolImg.transform.GetChild(1).GetComponent<Image>().color = Color.black;
            LeanTween.moveLocalY(_pistolImg.gameObject, 90, 0.1f);
            LeanTween.scale(_pistolImg.gameObject, Vector3.one, 0.3f);
        }
        else
        {
            if (m_movingAgent.GetSecondaryWeapon() == m_movingAgent.getCurrentWeapon())
            {
                _pistolImg.transform.GetChild(1).GetComponent<Image>().color = Color.yellow;
                LeanTween.moveLocalY(_pistolImg.gameObject, 100, 0.1f);
                LeanTween.scale(_pistolImg.gameObject, new Vector3(1.4f, 1.4f, 1f), 0.2f);
            }
            else
            {
                _pistolImg.transform.GetChild(1).GetComponent<Image>().color = Color.white;
                LeanTween.moveLocalY(_pistolImg.gameObject, 90, 0.1f);
                LeanTween.scale(_pistolImg.gameObject, Vector3.one, 0.1f);
            }
        }
        #endregion
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
            m_currentInteractable.setOutLineState(Interactable.outLineState.white);
            ItemStatUI.setVisibility(false);
        }

        m_currentInteractable = AgentItemFinder.findNearItem(playerTransfrom.position,true);

        if(m_currentInteractable)
        {
            //lootText.setInteratableObject(m_currentInteractable);
            m_currentInteractable.setOutLineState(Interactable.outLineState.Blue);
            _lootText.text = m_currentInteractable.name + ": Press E to Interact";
            //lootText.text = m_currentInteractable.name + ": Press E to Interact";

            FloatingGameUI onInteractiveUI = _InteractImg.GetComponent<FloatingGameUI>();
            onInteractiveUI.target = m_currentInteractable.gameObject.transform;

            _InteractImg.gameObject.SetActive(true);
            //updateWeaponStat(m_currentInteractable);
        }
        else
        {
            _InteractImg.gameObject.SetActive(false);

            //lootText.text = "";
            _lootText.text = "";
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
    #endregion
}

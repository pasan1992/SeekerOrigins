using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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


    [SerializeField] Image _pistolImg;
    [SerializeField] Image _rifleImg;
    [SerializeField] Image _grenadeImg;
    [SerializeField] TMP_Text _grenedCountTxt;

    [SerializeField] Image _healthBarImg;
    [SerializeField] Image _regenBarImg;
    [SerializeField] Image _bulletsBarImg;
    [SerializeField] TMP_Text _healthCountTxt;
    [SerializeField] TMP_Text _regenCountTxt;
    [SerializeField] TMP_Text _bulletCountTxt;

    [SerializeField] TMP_Text _magazinCountTxt;
    [SerializeField] Image _weaponImg;
    [SerializeField] Sprite[] _weaponList;

    [SerializeField] GameObject _activeWeapon;
    AgentData _agentData;

    public void Start()
    {
        m_player = GameObject.FindObjectOfType<PlayerController>();       
        m_movingAgent = m_player.GetComponent<HumanoidMovingAgent>();
        _agentData = m_movingAgent.GetAgentData();

    }
    // Update is called once per frame
    void Update()
    {

        updateLootText();
        update_ammo_count();
        updateAmmo();
        update_health();
    }

    void updateAmmo()
    {
        if (m_movingAgent.getCurrentWeapon() == null)
        {
            _activeWeapon.SetActive(false);
        }
        else
        {
            _activeWeapon.SetActive(true);
        }

        _grenedCountTxt.text = m_movingAgent.GetGrenadeCount().ToString();

        if (m_movingAgent.GetGrenadeCount() <= 0)
        {
            _grenadeImg.color = Color.black;
        }
        else
        {
            _grenadeImg.color = Color.white;
        }

        if(m_movingAgent.GetPrimaryWeapon() == null)
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

        if (m_movingAgent.getCurrentWeapon() != null)
        {
            _bulletCountTxt.text = m_movingAgent.getCurrentWeaponAmmoCount().ToString() + " / " + m_movingAgent.getCurrentWeaponMagazineSize().ToString();
            _bulletsBarImg.fillAmount = ((float)m_movingAgent.getCurrentWeaponAmmoCount()) / ((float)m_movingAgent.getCurrentWeaponMagazineSize());

            //_magazinCountTxt.text = (m_movingAgent.getCurrentWeaponMagazineSize() * m_movingAgent.getCurrentWeaponAmmoCount()).ToString(); //TODO

            var weaponType = m_movingAgent.getCurrentWeaponType(); 

            if (weaponType.ToString() == "primary")
            {
                _weaponImg.sprite = _weaponList[0];
            }
            else if (weaponType.ToString() == "secondary")
            {
                _weaponImg.sprite = _weaponList[1];
            }
        }
        else
        {
            _bulletCountTxt.text = "0";
            _bulletsBarImg.fillAmount = 0;
        }
    }

    private void update_health()
    {
        _healthCountTxt.text = ((int)_agentData.Health).ToString();
        _healthBarImg.fillAmount = _agentData.Health / _agentData.MaxHealth;

        _regenCountTxt.text = ((int)_agentData.Sheild).ToString();
        _regenBarImg.fillAmount = _agentData.Sheild / _agentData.MaxSheild;
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
                    int totalAmmo = 0;
                    agent_data.weaponAmmoCount.TryGetValue(m_movingAgent.getCurrentWeapon().m_weaponAmmunitionName, out totalAmmo);
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

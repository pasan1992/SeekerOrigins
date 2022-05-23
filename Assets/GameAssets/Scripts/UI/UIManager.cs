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

    [SerializeField] Image _healthBarImg;
    [SerializeField] Image _regenBarImg;
    [SerializeField] Image _bulletsBarImg;
    [SerializeField] TMP_Text _healthCountTxt;
    [SerializeField] TMP_Text _regenCountTxt;
    [SerializeField] TMP_Text _bulletCountTxt;

    [SerializeField] TMP_Text _magazinCountTxt;
    [SerializeField] Image _weaponImg;
    [SerializeField] Image[] _weaponList;


    public void Start()
    {
        m_player = GameObject.FindObjectOfType<PlayerController>();
        
        m_movingAgent = m_player.GetComponent<HumanoidMovingAgent>();
    }
    // Update is called once per frame
    void Update()
    {
        updateLootText();
        update_ammo_count();
    }

    private void update_health()
    {
        var agent_data = m_movingAgent.GetAgentData();
        //     m_movingAgent.getCurrentWeaponAmmoCount();
        //     m_movingAgent.getCurrentWeaponType();
        //     m_movingAgent.grenate
        //     agent_data.WeaponAmmo
        //m_movingAgent.getCurrentWeapon()
        //m_movingAgent.GetPrimaryWeapon();
        //m_movingAgent.GetSecondaryWeapon();


        ////-----1
        //if (!m_movingAgent.grenate)
        //{
        //    _grenadeImg.color = Color.black;
        //}
        //else
        //{
        //    _grenadeImg.color = Color.white;
        //}
        //if (!)
        //{
        //    _pistolImg.color = Color.black;
        //}
        //else
        //{
        //    _pistolImg.color = Color.white;
        //}
        //if (!)
        //{
        //    _rifleImg.color = Color.black;
        //}
        //else
        //{
        //    _rifleImg.color = Color.white;
        //}



        ////-----2
        //_bulletCountTxt.text = m_movingAgent.getCurrentWeaponAmmoCount().ToString();

        //_bulletsBarImg.fillAmount = 

        //_healthCountTxt.text = 

        //_healthBarImg.fillAmount = 

        //_regenCountTxt.text =

        //_regenBarImg.fillAmount = 





        ////-----3
        //_magazinCountTxt.text =

        //var weaponType = m_movingAgent.getCurrentWeaponType();

        //if (weaponType)
        //{
        //    _weaponImg.sprite = _weaponList[0].sprite;
        //}
        //else
        //{
        //    _weaponImg.sprite = _weaponList[1].sprite;
        //}


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

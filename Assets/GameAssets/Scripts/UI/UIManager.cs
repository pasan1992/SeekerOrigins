using System.Collections;
using System.Collections.Generic;
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

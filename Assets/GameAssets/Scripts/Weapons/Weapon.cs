using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : Interactable
{   
    
    #region Initialize
    public enum WEAPONTYPE { primary,secondary,grenede,Common};

    [System.Serializable]
    public class WeaponNonFunctionalProperties
    {
        public Vector3 magazinePositionOffset;
        public GameObject magazineObjProp;

        public Vector3 handPlacementOffset;

        public Vector3 weaponRecoilOffset;
        
    }

    public WeaponNonFunctionalProperties nonFunctionalProperties;
    public AgentBasicData.AgentFaction m_ownersFaction;
    protected bool m_isAimed = false;
    protected bool weaponSafty = false;
    protected bool is_owner_moving = false;


    protected GameObject m_target;
    public float accuracy_rating = 50;
    public float damage = 3;

    public float energyDamage = 0;
    public string m_weaponAmmunitionName = "BasicMag";

    #endregion

    #region Update
    public virtual void updateWeapon()
    {
    }
    #endregion

    #region Commands
    public virtual void dropWeapon()
    {
        OnItemDrop();
    }

    public virtual void resetWeapon()
    {

    }
    #endregion


    #region Getters and Setters
    public virtual WEAPONTYPE getWeaponType()
    {
        return WEAPONTYPE.Common;
    }

    public virtual void setWeaponSafty(bool enabled)
    {
        weaponSafty = enabled;
    }
    public virtual void setAimed(bool aimed)
    {
        m_isAimed = aimed;
    }

    public virtual void setGunTarget(GameObject target)
    {
        this.m_target = target;
    }

    public virtual void setOwnerFaction(AgentBasicData.AgentFaction owner)
    {
        m_ownersFaction = owner;
    }

    public void set_is_owner_moving(bool is_moving)
    {
        this.is_owner_moving = is_moving;
    }

    public bool get_is_owner_moving()
    {
        return is_owner_moving;
    }

    #endregion

    #region Events
    public virtual void onWeaponEquip()
    {

    }
    #endregion

    #region Utility
    #endregion

}

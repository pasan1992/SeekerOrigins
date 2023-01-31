using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class AgentData : AgentBasicData
{
    [System.Serializable]
    public struct AmmoPack
    {
        public string AmmoType;
        public int AmmoCount;
    }


    [SerializeField]
    private Dictionary<string,int> weaponAmmoCount;
    public PrimaryWeapon primaryWeapon;
    public SecondaryWeapon secondaryWeapon;
    public Grenade grenade;

    public bool use_cover = true;

    public int HealInjectionCount = 1;
    public int HealPerInjection = 20;

    public List<Interactable> inventryItems;
    public AgentBehaviorStatus behaviorStatus = AgentBehaviorStatus.Suspicious;
    public List<AmmoPack> WeaponAmmo = new List<AmmoPack>();

    public AmmoTypeEnums.RifleAmmo SelectedPrimaryAmmoType = AmmoTypeEnums.RifleAmmo.Regular_RifleAmmo;
    public AmmoTypeEnums.PistolAmmo SelectedSecondayAmmoType = AmmoTypeEnums.PistolAmmo.Regular_PistolAmmo;

    public bool immuneToStunn = false;

    public bool drop_reward = true;

    public bool unbalanced = false;

    public bool dodgeAll = false;

    public bool grenader = false;

    public AgentData()
    {
        InitalizeAmmo();
    }

    public void InitalizeAmmo()
    {
        if (WeaponAmmo.Count == 0)
        {
            setDefaultAmmo();
        }
        else
        {        
            weaponAmmoCount = new Dictionary<string, int>();
            foreach (AmmoPack apack in WeaponAmmo)
            {              
                weaponAmmoCount.Add(apack.AmmoType, apack.AmmoCount);
            }
        }
    }

    private void setDefaultAmmo()
    {
        weaponAmmoCount = new Dictionary<string, int>();
        weaponAmmoCount.Add("PistolAmmo", 10000);
        weaponAmmoCount.Add("RifleAmmo", 20);
        weaponAmmoCount.Add("Mag", 15);
        weaponAmmoCount.Add("Missile", 3);
    }

    public int useAmmoCount(string ammo_type,int count)
    {
        //use a given amount of ammo and return the used amount
        int rocketAmmo = 0;
        weaponAmmoCount.TryGetValue(ammo_type,out rocketAmmo);

        if(rocketAmmo > count)
        {
            rocketAmmo -= count;
            weaponAmmoCount[ammo_type] =rocketAmmo;
            return count;
        }
        else
        {
            weaponAmmoCount[ammo_type] =0;
            return rocketAmmo;
        }
    }

    public int checkTotalAmmo(string ammoName)
    {
        var unloaded = checkUnloadAvaialbleAmmo(ammoName);
        var loaded = 0;

        if (primaryWeapon !=null && primaryWeapon.posibleAmmoTypes.ContainsKey(ammoName) && primaryWeapon.getCurrentAmmoType() == ammoName)
        {
            loaded = primaryWeapon.getLoadedAmmoCount();
        }
    


        if (secondaryWeapon !=null && secondaryWeapon.posibleAmmoTypes.ContainsKey(ammoName) && secondaryWeapon.getCurrentAmmoType() == ammoName)
        {
            loaded = secondaryWeapon.getLoadedAmmoCount();
        }
        return unloaded + loaded;
    }

    public AgentData getSaveAgentData()
    {     
        WeaponAmmo = new List<AmmoPack>();
        foreach(var entry in weaponAmmoCount)
        {
            var newAmmoPack = new AmmoPack();
            newAmmoPack.AmmoCount = entry.Value;
            newAmmoPack.AmmoType = entry.Key;
            WeaponAmmo.Add(newAmmoPack);
        }
        return this;
    }

    public int checkUnloadAvaialbleAmmo(string ammoName)
    {
        int ammo = 0;
        weaponAmmoCount.TryGetValue(ammoName,out ammo);
        return ammo;
    }

    public void AddAmmo(string ammoName,int addCount)
    {
        int currntCount = checkUnloadAvaialbleAmmo(ammoName);
        weaponAmmoCount[ammoName] = currntCount + addCount;
    }


}

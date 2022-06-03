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
    public Dictionary<string,int> weaponAmmoCount;
    public PrimaryWeapon primaryWeapon;
    public SecondaryWeapon secondaryWeapon;
    public Grenade grenade;

    public List<Interactable> inventryItems;
    public AgentBehaviorStatus behaviorStatus = AgentBehaviorStatus.Suspicious;
    public List<AmmoPack> WeaponAmmo = new List<AmmoPack>();

    public bool drop_reward = true;

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
    }
}

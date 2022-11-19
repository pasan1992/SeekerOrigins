using UnityEngine;

public class SecondaryWeapon : RangedWeapon
{
    
    public override void Awake()
    {
        base.Awake();
        properties.Type = InteractableProperties.InteractableType.PickupInteraction;
        camplayer = GameObject.FindObjectOfType<GamePlayCam>();
        posibleAmmoTypes.Add(AmmoTypeEnums.PistolAmmo.Energy_PistolAmmo.ToString(),new RangedWeapon.AmmunitionType(this.damage,ProjectilePool.POOL_OBJECT_TYPE.ElectricProjectile,this.fireRate,0,this.damage*4));
        posibleAmmoTypes.Add(AmmoTypeEnums.PistolAmmo.Regular_PistolAmmo.ToString(),new RangedWeapon.AmmunitionType(this.damage,ProjectilePool.POOL_OBJECT_TYPE.BasicProjectile,this.fireRate,0,0));
        SwitchAmmoType(AmmoTypeEnums.PistolAmmo.Regular_PistolAmmo.ToString());
        m_ammoCount[AmmoTypeEnums.PistolAmmo.Regular_PistolAmmo.ToString()] = m_magazineSize;
    }

    public void Start()
    {
        //m_weaponAmmunitionName = "PistolAmmo";
    }


    #region command

    public override void pullTrigger()
    {
        base.pullTrigger();
    }

    public override void releaseTrigger()
    {
        base.releaseTrigger();
    }

    public override void dropWeapon()
    {
        base.dropWeapon();
        triggerPulled = false;
    }

    protected override void playWeaponFireSound()
    {
        m_audioScource.PlayOneShot(m_soundManager.getLaserPistolAudioClip());
    }
    #endregion

    #region updates

    public override void updateWeapon()
    {
        base.updateWeapon();
    }
    #endregion

    #region getters and setters

    public override WEAPONTYPE getWeaponType()
    {
        return WEAPONTYPE.secondary;
    }
    #endregion
}

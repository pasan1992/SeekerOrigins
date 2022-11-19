using UnityEngine;


public class PrimaryWeapon : RangedWeapon
{
    

    #region command

    public override void Awake()
    {
        base.Awake();
        posibleAmmoTypes.Add(AmmoTypeEnums.RifleAmmo.Energy_RifleAmmo.ToString(),new RangedWeapon.AmmunitionType(this.damage,ProjectilePool.POOL_OBJECT_TYPE.ElectricProjectile,this.fireRate,0.45f,this.damage*2));
        posibleAmmoTypes.Add(AmmoTypeEnums.RifleAmmo.Regular_RifleAmmo.ToString(),new RangedWeapon.AmmunitionType(this.damage,ProjectilePool.POOL_OBJECT_TYPE.BasicProjectile,this.fireRate,0.25f,0));
        properties.Type = InteractableProperties.InteractableType.PickupInteraction;
        camplayer = GameObject.FindObjectOfType<GamePlayCam>();
        SwitchAmmoType(AmmoTypeEnums.RifleAmmo.Regular_RifleAmmo.ToString());
        m_ammoCount[AmmoTypeEnums.RifleAmmo.Regular_RifleAmmo.ToString()] = m_magazineSize;
    }

    public void Start()
    {
        
    }

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
    }

    protected override void playWeaponFireSound()
    {
        m_audioScource.PlayOneShot(m_soundManager.getLaserRifalAudioClip());
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
        return WEAPONTYPE.primary;
    }
    #endregion
}

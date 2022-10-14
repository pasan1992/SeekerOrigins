using UnityEngine;


public class PrimaryWeapon : RangedWeapon
{
    

    #region command

    public override void Awake()
    {
        m_weaponAmmunitionName = "RifleAmmo";
        base.Awake();
        posibleAmmoTypes.Add("Incindeary",new RangedWeapon.AmmunitionType("Incindeary",2,ProjectilePool.POOL_OBJECT_TYPE.IncendearyProjectile,3,10,"IncendearyAmmo"));
        posibleAmmoTypes.Add("Ordinary",new RangedWeapon.AmmunitionType("Ordinary",this.damage,ProjectilePool.POOL_OBJECT_TYPE.BasicProjectile,this.fireRate,0,"RifleAmmo"));
        m_ammoCount["IncendearyAmmo"] = 0;
        properties.Type = InteractableProperties.InteractableType.PickupInteraction;
        camplayer = GameObject.FindObjectOfType<GamePlayCam>();
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

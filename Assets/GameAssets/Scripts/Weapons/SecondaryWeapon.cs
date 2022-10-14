using UnityEngine;

public class SecondaryWeapon : RangedWeapon
{
    
    public override void Awake()
    {
        base.Awake();
        properties.Type = InteractableProperties.InteractableType.PickupInteraction;
        camplayer = GameObject.FindObjectOfType<GamePlayCam>();
        posibleAmmoTypes.Add("Incindeary",new RangedWeapon.AmmunitionType("Incindeary",2,ProjectilePool.POOL_OBJECT_TYPE.IncendearyProjectile,1,3,"IncendearyAmmo"));
        posibleAmmoTypes.Add("Ordinary",new RangedWeapon.AmmunitionType("Ordinary",this.damage,ProjectilePool.POOL_OBJECT_TYPE.BasicProjectile,this.fireRate,1,"RifleAmmo"));
        m_ammoCount["IncendearyAmmo"] = 0;
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

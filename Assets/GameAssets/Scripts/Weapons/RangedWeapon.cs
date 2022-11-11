using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(BoxCollider))]
public abstract class RangedWeapon : Weapon
{
    public class AmmunitionType
    {
        public float damage;
        
        public ProjectilePool.POOL_OBJECT_TYPE particleType;
        public float fireRate;
        public float dot_time;
        public float energyDamage;

        public AmmunitionType(float damage,ProjectilePool.POOL_OBJECT_TYPE particleType,float fireRate,float dot_time,float energyDamage = 0)
        {
            this.damage = damage;
            this.particleType = particleType;
            this.fireRate = fireRate;
            this.dot_time = dot_time;
            this.energyDamage = energyDamage;
        }
    }

    public ProjectilePool.POOL_OBJECT_TYPE projectile = ProjectilePool.POOL_OBJECT_TYPE.BasicProjectile;

    public IDictionary<string, AmmunitionType> posibleAmmoTypes = new Dictionary<string, AmmunitionType>();

    public float dotTime = 0;

    public delegate void WeaponFireDeligaet(float weight);



    [Header("Fire Effects")]
    public ParticleSystem gunMuzzle;
    public ParticleSystem gunFireLight;

    [Header("Functional Parameters")]
    // This is for aimIK
    public GameObject targetPointTransfrom;
    public float fireRate;
    public float weaponRecoil = 2;
    public bool m_enableLine = true;
    public int m_magazineSize = 0;
    public bool full_auto = false;
    public float SoundMaxDistance = 3;


    // Firing pattern related variables
    protected float burstFireInterval; // for full auto
    protected bool waitToFire; // for single fire

    protected LayerMask hitLayerMask;
    
    protected Rigidbody m_rigidbody;
    protected BoxCollider m_collider;
    protected WeaponFireDeligaet m_onWeaponFire;
    protected Vector3 m_gunFireingPoint;
    protected AudioSource m_audioScource;
    protected SoundManager m_soundManager;
    protected ProjectilePool m_projectilePool;
    protected bool triggerPulled = false;
    protected bool m_realoding = false;
    protected IDictionary<string,int> m_ammoCount = new Dictionary<string,int>();
    protected GamePlayCam camplayer;

    private float POSSIBLE_MAX_RANGE = 40;
    public Light m_weaponLight;
    public override void Awake()
    {
        base.Awake();
        m_rigidbody = this.GetComponent<Rigidbody>();
        m_collider = this.GetComponent<BoxCollider>();
        m_audioScource = this.GetComponent<AudioSource>();
        m_soundManager = GameObject.FindObjectOfType<SoundManager>();
        m_projectilePool = GameObject.FindObjectOfType<ProjectilePool>();
        hitLayerMask = LayerMask.NameToLayer("Enemy");
        m_ammoCount[m_weaponAmmunitionName] = m_magazineSize;
        m_weaponLight = this.GetComponentInChildren<Light>();

        if(m_weaponLight != null)
            m_weaponLight.intensity = 0;
    }


    #region updates
    public override void updateWeapon()
    {
        if(targetPointTransfrom)
        {
            m_gunFireingPoint = targetPointTransfrom.transform.position - targetPointTransfrom.transform.forward * 0.1f;
        }

        if(full_auto)
        {
            updateContinouseFire();
        }
        else
        {
            update_single_Fire();
        }
    }

    public void SwitchAmmoType(string ammoTypeName)
    {
        var ammoType =  posibleAmmoTypes[ammoTypeName];
        this.damage = ammoType.damage;
        this.fireRate = ammoType.fireRate;
        this.dotTime = ammoType.dot_time;
        this.projectile = ammoType.particleType;
        this.energyDamage = ammoType.energyDamage;
        this.m_weaponAmmunitionName = ammoTypeName;
    }

    protected void updateContinouseFire()
    {
        if(triggerPulled)
        {
            burstFireInterval += Time.deltaTime;

            if (burstFireInterval > (1 / fireRate))
            {
                burstFireInterval = 0;
                fireWeapon();

                if (this.isActiveAndEnabled && getLoadedAmmoCount() > 0)
                {
                    StartCoroutine(waitAndRecoil());
                }

            }
        }
    }

    public void update_single_Fire()
    {
        base.updateWeapon();
        if (waitToFire)
        {
            burstFireInterval += Time.deltaTime;

            if ((1 / fireRate) <= burstFireInterval)
            {
                waitToFire = false;
                burstFireInterval = 0;
            }
        }
    }
    #endregion 

    #region getters and setters



    public void setReloading(bool isReloading)
    {
        m_realoding = isReloading;

        if(!m_realoding)
        {
            nonFunctionalProperties.magazineObjProp.SetActive(true);
        }
    }

    public bool isReloading()
    {
        return m_realoding;
    }

    public void SetGunTargetLineStatus(bool status)
    {
        m_enableLine = status;
    }

    public void addOnWeaponFireEvent(WeaponFireDeligaet onfire)
    {
        m_onWeaponFire = null;
        m_onWeaponFire = onfire;
    }

    public int getLoadedAmmoCount()
    {
        if (m_ammoCount.ContainsKey(m_weaponAmmunitionName))
        {
            return m_ammoCount[m_weaponAmmunitionName];
        }
        return 0;
    }

    public void setAmmoCount(int count)
    {
        m_ammoCount[m_weaponAmmunitionName] = count;
    }

    public bool isWeaponEmpty()
    {
        if (m_ammoCount.ContainsKey(m_weaponAmmunitionName))
        {
            return m_ammoCount[m_weaponAmmunitionName] == 0;
        }
        return true;
    }

    #endregion

    #region commands

    protected IEnumerator waitAndRecoil()
    {
        yield return new WaitForSeconds(0.1f);
        if(this.enabled)
        {
            m_onWeaponFire(weaponRecoil);

            if (gunMuzzle != null)
            {
                gunMuzzle.Play();
                gunFireLight.Play();
                //m_audioScource.PlayOneShot(m_soundManager.getLaserFireAudioClip());
                playWeaponFireSound();
            }
        }


    }

    protected abstract void playWeaponFireSound();

    protected virtual void fireWeapon()
    {
        if(getLoadedAmmoCount() > 0)
        {
            var originalPos = m_target.transform.position;
            m_target.transform.position += Random.onUnitSphere* calculate_recall_offset() + new Vector3(0,Random.Range(-0.2f,0f),0);
            m_ammoCount[m_weaponAmmunitionName] -=1;
            // GameObject Tempprojectile = GameObject.Instantiate(projectile, m_gunFireingPoint, this.transform.rotation);
            GameObject Tempprojectile = m_projectilePool.getPoolObject(this.projectile);
            Tempprojectile.transform.position = m_gunFireingPoint;
            Tempprojectile.transform.rotation = this.transform.rotation;


            Tempprojectile.transform.forward = (m_target.transform.position - m_gunFireingPoint).normalized;

            Tempprojectile.SetActive(true);
 
            // BasicProjectile projetcileBasic = Tempprojectile.GetComponent<BasicProjectile>();
            // projetcileBasic.speed = 5f;
            // projetcileBasic.setFiredFrom(m_ownersFaction);
            // projetcileBasic.setTargetTransfrom(m_target.transform);
            ProjectileMover proj = Tempprojectile.GetComponent<ProjectileMover>();  
            proj.damageFuture= null;

            RaycastHit hitPos =  DamageCalculator.checkFire(m_gunFireingPoint,m_target.transform.position,m_ownersFaction,new CommonFunctions.Damage(damage,energyDamage),proj,this.dotTime);
            EnvironmentSound.Instance.broadcastSound(this.transform.position,m_ownersFaction,SoundMaxDistance);

            
            if(hitPos.point != Vector3.zero)
            {
                Tempprojectile.transform.forward = (hitPos.point - m_gunFireingPoint).normalized;        
            }
            proj.StartFire(hitPos);


            m_target.transform.position = originalPos;
            if (this.isActiveAndEnabled)
            {
                StartCoroutine(waitAndRecoil());
            }

            if (m_ownersFaction == AgentBasicData.AgentFaction.Player)
            {
                StartCoroutine(camplayer.cam_Shake((m_target.transform.position - m_gunFireingPoint).normalized, 1));
            }
        }
        else
        {
            m_audioScource.PlayOneShot(m_soundManager.getEmptyGunSound());
        }
    }

    private float calculate_recall_offset()
    {
        
        var distance = Vector3.Distance(m_target.transform.position, m_gunFireingPoint);

        float moving_offset = 0;
        if (is_owner_moving)
            moving_offset = 0.5f;

        return (100f - accuracy_rating) * distance * 0.001f + moving_offset;
    }

    public override void dropWeapon()
    {
        triggerPulled = false;
        this.transform.parent = null;


        //m_rigidbody.isKinematic = false;
        //m_rigidbody.useGravity = true;
        //m_collider.isTrigger = false;

        //m_rigidbody.isKinematic = true;
        //m_rigidbody.useGravity = false;
        //m_rigidbody.detectCollisions = false;
        //m_rigidbody.velocity = Vector3.zero;
        //m_collider.isTrigger = true;


        properties.interactionEnabled = true;
        base.dropWeapon();
        place_on_ground();
    }

    public void place_on_ground()
    {
        RaycastHit hit;
        if (Physics.Raycast(this.transform.position,Vector3.down,out hit,10, CommonFunctions.getFloorLayerMask()))
        {
            this.transform.position = hit.point + Vector3.up*0.2f;
        }
    }

    public virtual void reloadWeapon()
    {
        setReloading(true);
        /*
        GameObject obj = ProjectilePool.getInstance().getPoolObject(ProjectilePool.POOL_OBJECT_TYPE.RifleAmmo);
        obj.transform.position = this.transform.position + nonFunctionalProperties.magazinePositionOffset;
        obj.transform.parent = null;
        obj.SetActive(true);
        obj.transform.rotation = Quaternion.Euler(Random.insideUnitSphere*90);
        */
        nonFunctionalProperties.magazineObjProp.SetActive(false);
    }

    public override void resetWeapon()
    {
        Debug.Log("Reseting weapon");
        m_rigidbody.isKinematic = true;
        m_rigidbody.useGravity = false;
        m_collider.isTrigger = true;
    }

    public virtual void pullTrigger()
    {
        if(!weaponSafty && !m_realoding)
        {
            triggerPulled = true;

            // If the weapon is auto
            if(full_auto)
            {
                pull_trigger_auto_weapon();
            }
            else
            {
                pull_trigger_single_weapon();
            }
        }
    }

    private void pull_trigger_auto_weapon()
    {
        burstFireInterval = (1 / fireRate) + 0.1f;
    }

    private void pull_trigger_single_weapon()
    {
        if (!waitToFire)
        {
            fireWeapon();
            waitToFire = true;
        }
    }

    public virtual void releaseTrigger()
    {
        triggerPulled = false;
    }

    #endregion

    #region eventHandlers

    public override void onWeaponEquip()
    {
        setAimed(true);
        if(m_rigidbody ==null)
        {
            //m_rigidbody = this.GetComponent<Rigidbody>();
        }
        m_rigidbody.isKinematic = true;

    }

    public override void OnPlaceOnHoster()
    {
        base.OnPlaceOnHoster();

        if(m_weaponLight !=null)
            m_weaponLight.intensity = 0;
    }

    public override void OnPlaceOnHand()
    {
        base.OnPlaceOnHand();
        if(m_weaponLight !=null)
           m_weaponLight.intensity = 1.3f;
    }

    // private void checkFire(Vector3 startPositon, Vector3 targetPositon)
    // {
    //     RaycastHit hit;
    //     string[] layerMaskNames = { "HalfCoverObsticles","FullCoverObsticles","Enemy" };
    //     bool hitOnEnemy = false;

    //     // Give offset to starting postion to avoid bullets colliding in own covers
    //     Vector3 offsetTargetPositon =  (targetPositon - startPositon).normalized + startPositon;
    //     if (Physics.Raycast(offsetTargetPositon, targetPositon - startPositon, out hit,100, LayerMask.GetMask(layerMaskNames)))
    //     {
    //         switch(hit.transform.tag)
    //         {
    //             case "Cover":
    //             case "Wall":
    //             DamageCalculator.hitOnWall(hit.collider,hit.point);
    //             break;
    //             case "Enemy":
    //             case "Player":
    //             case "Head":
    //             case "Chest":
    //             DamageCalculator.onHitEnemy(hit.collider,m_ownersFaction,(targetPositon-startPositon).normalized);
    //             hitOnEnemy = true;
    //             break;       
    //         }          

    //     }
        
    //     // Check fire for the second time for find crouched enemies.
    //     if(!hitOnEnemy && Physics.Raycast(offsetTargetPositon, targetPositon + new Vector3(0,-1f,0) - startPositon, out hit,100, LayerMask.GetMask(layerMaskNames)))
    //     {
    //         switch(hit.transform.tag)
    //         {
    //             case "Cover":
    //             case "Wall":
    //             //DamageCalculator.onHitEnemy(hit.collider,m_ownersFaction,(targetPositon-startPositon).normalized);
    //                 DamageCalculator.hitOnWall(hit.collider,hit.point);
    //             break;
    //             case "Enemy":
    //             case "Player":
    //             case "Head":
    //             case "Chest":
    //                 DamageCalculator.onHitEnemy(hit.collider,m_ownersFaction,(targetPositon-startPositon).normalized);
    //             break;       
    //         }                
    //     }
    // }

    #endregion
}

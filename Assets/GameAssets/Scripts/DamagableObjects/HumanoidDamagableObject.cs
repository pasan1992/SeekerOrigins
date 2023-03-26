using UnityEngine;
public class HumanoidDamagableObject : MovingAgentDamagableObject
{
    public GameObject currentAvatar;

    public GameObject[] avatar;

    private bool destroyed = false;

    private int postDestroyDamage = 0;

    public bool DontTakeDamage = false;

    public void damgeHead(float dmg)
    {
        damage(new CommonFunctions.Damage(dmg,dmg),m_movingAgent.getHeadTransfrom().GetComponentInChildren<Collider>(),-m_movingAgent.getHeadTransfrom().forward,m_movingAgent.getHeadTransfrom().position,
        AgentData.AgentFaction.Player,0);
    }

    public override bool damage(CommonFunctions.Damage damageValue,Collider collider, Vector3 force, Vector3 point, AgentBasicData.AgentFaction fromFaction ,float stunPrecentage = 0)
    {
        // fromFaction is not used atm
        /*
        if (!GamePlayCam.IsVisibleToCamera(m_movingAgent.getTransfrom()))
        {

            Debug.Log("Not damaging");
            return false;
        }
        */
        if(DontTakeDamage)
        {
            return false;
        }
        
        if ( (stunPrecentage > Random.value && !m_movingAgent.isDisabled()) || (m_movingAgent.GetAgentData().unbalanced && Random.value > 0.5f))
        {
            //StartCoroutine(DotDamage(damageValue,collider,force,point,fromFaction,(int)stunPrecentage));
            //return false;
            //m_movingAgent.GetAgentData().AgentNature == AgentBasicData.AGENT_NATURE.DROID
            if(damageValue.energyDamage > damageValue.healthDamage)
            {
                m_movingAgent.setShocked(3f,force);
            }
            else
            {
                m_movingAgent.setStunned(3f,force);
            }
            
        }


        if (collider.name == "Head")
        {
            damageValue.healthDamage += damageValue.healthDamage * 1.5f;
            damageValue.energyDamage += damageValue.energyDamage * 1.5f;
        }
        if(m_movingAgent.isDisabled())
        {
            damageValue.healthDamage += damageValue.healthDamage * 2f;
            damageValue.energyDamage += damageValue.energyDamage * 2f;            
        }
        
        m_movingAgent.damageAgent(calculate_bonous_damage(-m_movingAgent.getTransfrom().forward,force,damageValue));
        m_movingAgent.reactOnHit(collider, force, point);


        ProjectilePool.POOL_OBJECT_TYPE damageEffect = ProjectilePool.POOL_OBJECT_TYPE.ElectricParticleEffect;
        if(m_movingAgent.GetAgentData().Sheild == 0)
        {
            damageEffect = particleEffectOnDamage;
        }

        GameObject smoke = ProjectilePool.getInstance().getPoolObject(damageEffect);
        if(smoke)
        {
        smoke.SetActive(true);
        smoke.transform.position = collider.transform.position;
        }

        if (getRemaningHealth() == 0)
        {


            

            if (!destroyedEventCalled)
            {

                collider.transform.localScale = Vector3.zero;
                switch (m_movingAgent.GetAgentData().AgentNature)
                {
                    case AgentBasicData.AGENT_NATURE.DROID:
                        droid_damage_effects(damageValue.healthDamage, collider, force, point);
                        break;
                    case AgentBasicData.AGENT_NATURE.HUMANOID:
                        humanDamageEffect(damageValue.healthDamage, collider, force, point);
                        break;
                }
                destroyedEventCalled = true;

                if(onDestroyedEvent != null)
                {
                    onDestroyedEvent();
                }
                
                if(m_objectUI)
                {
                    m_objectUI.OnDistryedObject();
                }

                
                StartCoroutine(waitAndDestory(5));
                
            }

        }

        if(m_movingAgent.IsFunctional())
        {
            if(onDamagedEvent !=null)
            {
                onDamagedEvent();
            }
        }
        StartCoroutine(onDamageEffect());
        
        return !m_movingAgent.IsFunctional();
    }

    private CommonFunctions.Damage calculate_bonous_damage(Vector3 face_direction,Vector3 force_direction,CommonFunctions.Damage damage)
    {
        var angle = Vector3.Angle(face_direction, force_direction);
        if (angle > 90)
        {
            damage.energyDamage = damage.energyDamage * 1.5f;
            damage.healthDamage = damage.healthDamage * 1.5f;
            return damage;
        }
        return damage;
    }

    private void humanDamageEffect(float damageValue, Collider collider, Vector3 force, Vector3 point)
    {
        //StartCoroutine("waitAndDestory");

        GameObject smoke = ProjectilePool.getInstance().getPoolObject(ProjectilePool.POOL_OBJECT_TYPE.HumanoidExplosion);
        smoke.SetActive(true);
        smoke.transform.position = collider.transform.position;

        SetFireEffect(false,null);
    }


    private void droid_damage_effects(float damageValue, Collider collider, Vector3 force, Vector3 point)
    {
        if (!destroyed)
        {
            destroyed = true;
            var random_value = 0;
            if(!m_movingAgent.isHidden())
            {
                random_value = Random.Range(0,4);
            }   
            postDestoryEffect(collider.transform);



            randomDestroyEffect(random_value,collider);

            /*
            if(random_value < 0.3)
            {
                postDestroyDamage = 2;
                droid_damage_effects(damageValue, collider, force, point);
            }
            else if(random_value < 0.5)
            {
                postDestroyDamage = 4;
                droid_damage_effects(damageValue, collider, force, point);
            }
            */
            

            // Invoke("postDestoryEffect", 1);
        }       
    }

    // private void droid_damage_effects(float damageValue, Collider collider, Vector3 force, Vector3 point)
    // {
    //     if (!destroyed)
    //     {
    //         destroyed = true;
    //         GameObject explosion = ProjectilePool.getInstance().getPoolObject(ProjectilePool.POOL_OBJECT_TYPE.DroidExplosionParticleEffect);
    //         explosion.transform.position = this.transform.position;
    //         explosion.SetActive(true);
    //         explosion.GetComponent<DroidExplosion>().explodePart(DroidExplosion.ExplosionPart.Head);

    //        // StartCoroutine(camplayer.cam_Shake(0.5f, 0.5f));
    //         currentAvatar.SetActive(false);

    //         if (avatar.Length > 0)
    //         {
    //             avatar[0].SetActive(true);
    //         }

    //         var random_value = Random.value;

    //         /*
    //         if(random_value < 0.3)
    //         {
    //             postDestroyDamage = 2;
    //             droid_damage_effects(damageValue, collider, force, point);
    //         }
    //         else if(random_value < 0.5)
    //         {
    //             postDestroyDamage = 4;
    //             droid_damage_effects(damageValue, collider, force, point);
    //         }
    //         */
            

    //         Invoke("postDestoryEffect", 1);
    //     }
    //     else if (m_movingAgent.GetAgentData().AgentNature == AgentBasicData.AGENT_NATURE.DROID)
    //     {
    //         GameObject postDestoryExplosion = ProjectilePool.getInstance().getPoolObject(ProjectilePool.POOL_OBJECT_TYPE.DroidExplosionParticleEffect);
    //         if (postDestoryExplosion == null)
    //         {
    //             Debug.LogWarning("Not enough effects");
    //             return;
    //         }
                
    //         postDestoryExplosion.transform.position = collider.transform.position;
    //         postDestoryExplosion.SetActive(true);
    //         m_audioSource.PlayOneShot(m_soundManager.getDroneExplosion());


    //         GameObject postDestoryExplosion2 = ProjectilePool.getInstance().getPoolObject(ProjectilePool.POOL_OBJECT_TYPE.DroidExplosionParticleEffect);
    //         postDestoryExplosion2.transform.position = collider.transform.position;
    //         postDestoryExplosion2.SetActive(true);
    //         switch (postDestroyDamage)
    //         {
    //             case 2:
    //                 postDestoryExplosion.GetComponent<DroidExplosion>().explodePart(DroidExplosion.ExplosionPart.Hands);
    //                 postDestoryExplosion2.GetComponent<DroidExplosion>().explodePart(DroidExplosion.ExplosionPart.OneLeg);
    //                 //postDestoryExplosion.GetComponent<DroidExplosion>().explodePart(DroidExplosion.ExplosionPart.Legs);
    //                 avatar[0].SetActive(false);
    //                 avatar[1].SetActive(true);
    //                 break;
    //             case 4:
    //                 postDestoryExplosion.GetComponent<DroidExplosion>().explodePart(DroidExplosion.ExplosionPart.Body);
    //                 postDestoryExplosion2.GetComponent<DroidExplosion>().explodePart(DroidExplosion.ExplosionPart.OneLeg);
    //                 avatar[1].SetActive(false);
    //                 avatar[0].SetActive(false);
    //                 //avatar[2].SetActive(true);
    //                 break;
    //                 // case 3:
    //                 //     postDestoryExplosion.GetComponent<DroidExplosion>().explodePart(DroidExplosion.ExplosionPart.Body);
    //                 //     headlessAvatar[2].SetActive(false);
    //                 // break;
    //         }

    //         postDestroyDamage++;

    //     }
    // }

    private void randomDestroyEffect(int destroyEffectId,Collider collider)
    {
        if(avatar.Length < 3)
        {
            return;
        }

            currentAvatar.SetActive(false);
            GameObject postDestoryExplosion = ProjectilePool.getInstance().getPoolObject(ProjectilePool.POOL_OBJECT_TYPE.DroidExplosionParticleEffect);
            if (postDestoryExplosion == null)
            {
                Debug.LogWarning("Not enough effects");
                return;
            }
                
            postDestoryExplosion.transform.position = collider.transform.position;
            postDestoryExplosion.SetActive(true);
            CommonFunctions.PlaySound("DroidExplosion",m_audioSource);


            GameObject postDestoryExplosion2 = ProjectilePool.getInstance().getPoolObject(ProjectilePool.POOL_OBJECT_TYPE.DroidExplosionParticleEffect);
            postDestoryExplosion2.transform.position = collider.transform.position;
            

            switch (destroyEffectId)
            {
                case 0:
                    postDestoryExplosion.GetComponent<DroidExplosion>().explodePart(DroidExplosion.ExplosionPart.Head);
                    avatar[0].SetActive(true);
                    avatar[1].SetActive(false);
                    avatar[2].SetActive(false);
                    avatar[3].SetActive(false);

                break;
                case 1:
                    postDestoryExplosion.GetComponent<DroidExplosion>().explodePart(DroidExplosion.ExplosionPart.OneLeg);
                    avatar[0].SetActive(false);
                    avatar[1].SetActive(true);
                    avatar[2].SetActive(false);
                    avatar[3].SetActive(false);
                break;
                case 2:
                    postDestoryExplosion.GetComponent<DroidExplosion>().explodePart(DroidExplosion.ExplosionPart.Hands);
                    postDestoryExplosion2.GetComponent<DroidExplosion>().explodePart(DroidExplosion.ExplosionPart.OneLeg);
                    postDestoryExplosion2.SetActive(true);
                    avatar[0].SetActive(false);
                    avatar[1].SetActive(false);
                    avatar[2].SetActive(true);
                    avatar[3].SetActive(false);
                    break;
                case 3:
                    postDestoryExplosion.GetComponent<DroidExplosion>().explodePart(DroidExplosion.ExplosionPart.OneHand);
                    postDestoryExplosion2.SetActive(false);
                    avatar[0].SetActive(false);
                    avatar[1].SetActive(false);
                    avatar[2].SetActive(false);
                    avatar[3].SetActive(true);
                    //avatar[2].SetActive(true);
                    break;
            }
    }

    private void postDestoryEffect(Transform location)
    {
        GameObject smoke = ProjectilePool.getInstance().getPoolObject(ProjectilePool.POOL_OBJECT_TYPE.ElectricParticleEffect);
        GameObject electricEffet = ProjectilePool.getInstance().getPoolObject(ProjectilePool.POOL_OBJECT_TYPE.SmokeEffect);
        smoke.SetActive(true);
        electricEffet.SetActive(true);
        smoke.transform.position = location.position;
        electricEffet.transform.position = location.position;
        smoke.transform.parent = location.transform;
        electricEffet.transform.parent = location.transform;
    }
}
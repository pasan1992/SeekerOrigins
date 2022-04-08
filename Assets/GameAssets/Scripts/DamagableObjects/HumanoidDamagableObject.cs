using UnityEngine;
public class HumanoidDamagableObject : MovingAgentDamagableObject
{
    public GameObject currentAvatar;

    public GameObject[] avatar;

    private bool destroyed = false;

    private int postDestroyDamage = 0 ;


    public override bool damage(float damageValue,Collider collider, Vector3 force, Vector3 point, AgentBasicData.AgentFaction fromFaction)
    {
        // fromFaction is not used atm
        /*
        if (!GamePlayCam.IsVisibleToCamera(m_movingAgent.getTransfrom()))
        {

            Debug.Log("Not damaging");
            return false;
        }
        */


        if (collider.name == "Head")
        {
            damageValue += damageValue * 1.5f;
        }
        m_movingAgent.damageAgent(calculate_bonous_damage(-m_movingAgent.getTransfrom().forward,force,damageValue));
        m_movingAgent.reactOnHit(collider, force, point);
        if (getRemaningHealth() == 0)
        {

            switch (m_movingAgent.GetAgentData().AgentNature)
            {
                case AgentBasicData.AGENT_NATURE.DROID:
                    droid_damage_effects(damageValue, collider, force, point);
                    break;
                case AgentBasicData.AGENT_NATURE.HUMANOID:
                    break;
            }

            

            if (!destroyedEventCalled)
            {
                destroyedEventCalled = true;

                if(onDestroyedEvent != null)
                {
                    onDestroyedEvent();
                }
                

                m_objectUI.OnDistryedObject();
                
            }

        }

        if(m_movingAgent.IsFunctional())
        {
            if(onDamagedEvent !=null)
            {
                onDamagedEvent();
            }
        }
        
        return !m_movingAgent.IsFunctional();
    }

    private float calculate_bonous_damage(Vector3 face_direction,Vector3 force_direction,float damage)
    {
        var angle = Vector3.Angle(face_direction, force_direction);
        if (angle > 90)
        {
            return damage * 1.5f;
        }
        return damage;
    }


    private void droid_damage_effects(float damageValue, Collider collider, Vector3 force, Vector3 point)
    {
        if (!destroyed)
        {
            destroyed = true;
            GameObject explosion = ProjectilePool.getInstance().getPoolObject(ProjectilePool.POOL_OBJECT_TYPE.DroidExplosionParticleEffect);
            explosion.transform.position = this.transform.position;
            explosion.SetActive(true);
            explosion.GetComponent<DroidExplosion>().explodePart(DroidExplosion.ExplosionPart.Head);

           // StartCoroutine(camplayer.cam_Shake(0.5f, 0.5f));
            currentAvatar.SetActive(false);

            if (avatar.Length > 0)
            {
                avatar[0].SetActive(true);
            }

            var random_value = Random.value;

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
            

            Invoke("postDestoryEffect", 1);
        }
        else if (m_movingAgent.GetAgentData().AgentNature == AgentBasicData.AGENT_NATURE.DROID)
        {
            GameObject postDestoryExplosion = ProjectilePool.getInstance().getPoolObject(ProjectilePool.POOL_OBJECT_TYPE.DroidExplosionParticleEffect);
            if (postDestoryExplosion == null)
            {
                Debug.LogWarning("Not enough effects");
                return;
            }
                
            postDestoryExplosion.transform.position = collider.transform.position;
            postDestoryExplosion.SetActive(true);
            m_audioSource.PlayOneShot(m_soundManager.getDroneExplosion());


            GameObject postDestoryExplosion2 = ProjectilePool.getInstance().getPoolObject(ProjectilePool.POOL_OBJECT_TYPE.DroidExplosionParticleEffect);
            postDestoryExplosion2.transform.position = collider.transform.position;
            postDestoryExplosion2.SetActive(true);
            switch (postDestroyDamage)
            {
                case 2:
                    postDestoryExplosion.GetComponent<DroidExplosion>().explodePart(DroidExplosion.ExplosionPart.Hands);
                    postDestoryExplosion2.GetComponent<DroidExplosion>().explodePart(DroidExplosion.ExplosionPart.OneLeg);
                    //postDestoryExplosion.GetComponent<DroidExplosion>().explodePart(DroidExplosion.ExplosionPart.Legs);
                    avatar[0].SetActive(false);
                    avatar[1].SetActive(true);
                    break;
                case 4:
                    postDestoryExplosion.GetComponent<DroidExplosion>().explodePart(DroidExplosion.ExplosionPart.Body);
                    postDestoryExplosion2.GetComponent<DroidExplosion>().explodePart(DroidExplosion.ExplosionPart.OneLeg);
                    avatar[1].SetActive(false);
                    avatar[0].SetActive(false);
                    //avatar[2].SetActive(true);
                    break;
                    // case 3:
                    //     postDestoryExplosion.GetComponent<DroidExplosion>().explodePart(DroidExplosion.ExplosionPart.Body);
                    //     headlessAvatar[2].SetActive(false);
                    // break;
            }

            postDestroyDamage++;

        }
    }

    private void postDestoryEffect()
    {
        GameObject smoke = ProjectilePool.getInstance().getPoolObject(ProjectilePool.POOL_OBJECT_TYPE.ElectricParticleEffect);
        GameObject electricEffet = ProjectilePool.getInstance().getPoolObject(ProjectilePool.POOL_OBJECT_TYPE.SmokeEffect);
        smoke.SetActive(true);
        electricEffet.SetActive(true);
        smoke.transform.position = this.transform.position;
        electricEffet.transform.position = this.transform.position;
    }
}
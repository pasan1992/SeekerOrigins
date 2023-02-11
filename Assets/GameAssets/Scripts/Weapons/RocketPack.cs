using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketPack : MonoBehaviour
{
    // Start is called before the first frame update
    public List<Transform> Locations;
    private int m_current_transfrom = 0;
    public FloatingGameUI targetIcon;

    private string currentRocketName = AmmoTypeEnums.Missile.DroneBusters_Missile.ToString();
    
    public void FireMissleLocation(Vector3 location, AgentData agentData)
    {

        var ammo_count = agentData.useAmmoCount(getCurrentRokectName(),1);

        if(ammo_count > 0)
        {
            GameObject basicRocketObj = ProjectilePool.getInstance().getPoolObject(ProjectilePool.POOL_OBJECT_TYPE.BasicRocket);
            var fire_transfrom = getFireTransfrom();
            basicRocketObj.transform.position = fire_transfrom.transform.position + new Vector3(0, 0, 0);
            basicRocketObj.transform.rotation = fire_transfrom.transform.rotation;
            basicRocketObj.SetActive(true);
            BasicRocket rocket = basicRocketObj.GetComponent<BasicRocket>();
            rocket.fireRocketLocation(location);
            rocket.rocketScale = 0.4f;
            Time.timeScale = 1;
        }

    }

    public string getCurrentRokectName()
    {
        return currentRocketName;
    }

    public void FireMissleTransfrom(Transform transfrom, AgentData agentData)
    {
        var ammo_count = agentData.useAmmoCount(getCurrentRokectName(),1);

        if(ammo_count > 0)
        {
            GameObject basicRocketObj = ProjectilePool.getInstance().getPoolObject(ProjectilePool.POOL_OBJECT_TYPE.BasicRocket);
            var fire_transfrom = getFireTransfrom();
            basicRocketObj.transform.position = fire_transfrom.transform.position + new Vector3(0, 0, 0);
            basicRocketObj.transform.rotation = fire_transfrom.transform.rotation;
            basicRocketObj.SetActive(true);
            BasicRocket rocket = basicRocketObj.GetComponent<BasicRocket>();
            rocket.fireRocketTransfrom(transfrom);
            rocket.rocketScale = 0.4f;
            Time.timeScale = 1;

            if(targetIcon)
            {
                targetIcon.target = transfrom;
                targetIcon.setMainImageStatus(true);
                StartCoroutine(disableTarget(1.5f));
            }
        }
    }

    private Transform getFireTransfrom()
    {
        m_current_transfrom += 1;
        if(m_current_transfrom > Locations.Count-1)
        {
            m_current_transfrom = 0;
        }
        return Locations[m_current_transfrom];
    }


    public void fireAllRockets(AgentData agentData)
    {
        List<DamagableObject> objs = new List<DamagableObject>();

        Collider[] hitColliders = Physics.OverlapSphere(this.transform.position, 30);
            foreach (Collider hitCollider in hitColliders)
            {


                switch (hitCollider.tag)
                {
                    case "Enemy":
                    case "Player":
                    case "Head":
                    case "Chest":

                        DamagableObject damagableObject =  hitCollider.GetComponentInParent<DamagableObject>();
                        var agent = hitCollider.GetComponentInParent<ICyberAgent>();
                        if(damagableObject == null || agent == null || CommonFunctions.isAllies(agent.GetAgentData() , agentData ) || !agent.IsFunctional() || objs.Contains(damagableObject))
                        {
                            continue;
                        }
                        
                        objs.Add(damagableObject);
                        FireMissleTransfrom(hitCollider.transform,agentData);
                        break;
                    case "Wall":
                        break;
                    case "Cover":
                        break;
                    case "Floor":

                        break;
                }
            }

    }

    IEnumerator disableTarget(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        targetIcon.setMainImageStatus(false);
    }
}

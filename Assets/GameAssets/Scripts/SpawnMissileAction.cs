using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace HutongGames.PlayMaker.Actions
{
    public class SpawnMissileAction : FsmStateAction
    {
        // Start is called before the first frame update

        public HumanoidMovingAgent AttackerAgent;
        public Transform targetTransfrom;
        public Transform fireTransfrom;

        public bool FollowMissile = false;


        public override void OnEnter()
        {
                if(AttackerAgent)
                {
                    AttackerAgent.setPoint(2,-(targetTransfrom.position - AttackerAgent.getCurrentPosition()));
                }
                StartCoroutine(waitAndFire());

        }

        IEnumerator waitAndFire()
        {
            yield return new WaitForSeconds(0.8f);
            GameObject basicRocketObj = ProjectilePool.getInstance().getPoolObject(ProjectilePool.POOL_OBJECT_TYPE.BasicRocket);
            basicRocketObj.transform.position = fireTransfrom.transform.position + new Vector3(0, 0, 0);
            basicRocketObj.transform.rotation = fireTransfrom.transform.rotation;
            basicRocketObj.SetActive(true);
            BasicRocket rocket = basicRocketObj.GetComponent<BasicRocket>();
            if(FollowMissile)
            {
                rocket.fireRocketTransfrom(targetTransfrom);
            }
            else
            {
                rocket.fireRocketLocation(targetTransfrom.position);
            }
            
            Finish();
        }

        public override void OnUpdate()
        {

        }

    }
}


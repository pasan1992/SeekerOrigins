using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoPack : Interactable
{
    // Start is called before the first frame update
    public List<AgentData.AmmoPack> AmmoPackData;
    public int GrenadeCount = 0;
    public string AmmoType = "RifleAmmo";
    public int count = 10;
    void Start()
    {
        properties.Type = InteractableProperties.InteractableType.PickupInteraction;
    }

    public override void OnPickUpAction()
    {

        StartCoroutine(WaitAndShowEffect());
        AmmoPackData = new List<AgentData.AmmoPack>();
        GrenadeCount = 0;
        base.OnPickUpAction();
    }

    public IEnumerator WaitAndShowEffect()
    {

        foreach(AgentData.AmmoPack ammo in AmmoPackData)
        {
            var obj = ProjectilePool.getInstance().getPoolObject(ProjectilePool.POOL_OBJECT_TYPE.DamageText);
            obj.SetActive(true);
            if(obj != null)
            {
                var dmg_text = obj.GetComponent<FloatingDamageText>();
                dmg_text.SetText("+" +ammo.AmmoType + ":" + ammo.AmmoCount.ToString(),this.transform.position,Color.blue);
            }
            yield return new WaitForSeconds(4);
            
        }    
        
        if(GrenadeCount !=0)
        {
             yield return new WaitForSeconds(1);
             var obj = ProjectilePool.getInstance().getPoolObject(ProjectilePool.POOL_OBJECT_TYPE.DamageText);
            obj.SetActive(true);
            if(obj != null)
            {
                var dmg_text = obj.GetComponent<FloatingDamageText>();
                dmg_text.SetText("+" +"Grenade" + ":" + GrenadeCount,this.transform.position,Color.blue);
            }           
        }
        
    }
}

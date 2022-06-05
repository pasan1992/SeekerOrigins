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
        var ammoPackData = AmmoPackData;
        var grenadeCount = GrenadeCount;
        AmmoPackAnimation();
        //base.OnPickUpAction();
        ResetAmmoPack();
    }

    void ResetAmmoPack()
    {
        AmmoPackData = new List<AgentData.AmmoPack>();
        GrenadeCount = 0;
    }

    void AmmoPackAnimation()
    {
        var seq = LeanTween.sequence();
        seq.append(LeanTween.moveY(gameObject,gameObject.transform.position.y + 0.02f,0.1f));
        seq.append( LeanTween.rotateAroundLocal(gameObject,Vector3.up,Random.value*100,0.1f).setEaseInCirc() );
        seq.append( LeanTween.rotateAroundLocal(gameObject,Vector3.up,-Random.value*100,0.1f).setEaseInCirc()  );
        seq.append(LeanTween.moveY(gameObject,gameObject.transform.position.y - 0.02f,0.1f));
    }
}

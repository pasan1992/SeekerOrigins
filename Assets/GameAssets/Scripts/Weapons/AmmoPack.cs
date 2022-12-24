using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoPack : Interactable
{
    // Start is called before the first frame update
    public List<AgentData.AmmoPack> AmmoPackData;
    
    //public int GrenadeCount = 0;

    public int HealthInjectionCount = 0;

    private Animator m_anim;

    void Start()
    {
        properties.Type = InteractableProperties.InteractableType.PickupInteraction;
        m_anim = this.GetComponent<Animator>();
    }

    public override void OnPickUpAction()
    {
        var ammoPackData = AmmoPackData;
        //var grenadeCount = GrenadeCount;
        //AmmoPackAnimation();
        //base.OnPickUpAction();
        ResetAmmoPack();
        
    }

    void ResetAmmoPack()
    {
        properties.interactionEnabled = false;
    }

    void AmmoPackAnimation()
    {
        var seq = LeanTween.sequence();
        seq.append(LeanTween.moveY(gameObject,gameObject.transform.position.y + 0.02f,0.1f));
        seq.append( LeanTween.rotateAroundLocal(gameObject,Vector3.up,Random.value*100,0.1f).setEaseInCirc() );
        seq.append( LeanTween.rotateAroundLocal(gameObject,Vector3.up,-Random.value*100,0.1f).setEaseInCirc()  );
        seq.append(LeanTween.moveY(gameObject,gameObject.transform.position.y - 0.02f,0.1f));
    }

    public override void OnInteractionStart()
    {
        m_anim.SetTrigger("open");
        m_outLine.enabled = false;
    }
}

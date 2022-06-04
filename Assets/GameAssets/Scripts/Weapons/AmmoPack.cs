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
        LeanTween.cancel(gameObject);
        gameObject.transform.localScale = Vector3.one;
        LeanTween.scale(gameObject, new Vector3(0.1f, 0.1f, 0.1f) * 0.5f, 0.3f)
            .setEasePunch();
    }
}

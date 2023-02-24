using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace HutongGames.PlayMaker.Actions
{

public class ActionAddAmmo : FsmStateAction
{
    // Start is called before the first frame update
    public HumanoidMovingAgent agent;

    public string AmmoName;
    public int AmmoCount;

    public Transform PackTransform;

    public override void OnEnter()
    {
        
        var AmmoStruct = new AgentData.AmmoPack();
        AmmoStruct.AmmoCount = AmmoCount;
        AmmoStruct.AmmoType = AmmoName;


        
        var obj = new GameObject();
        obj.transform.position = PackTransform.position;
        obj.AddComponent<AmmoPack>();
        var fakeAmmoPack = obj.GetComponent<AmmoPack>();     
        fakeAmmoPack.visualProperties = new Interactable.VisualProperites();
        fakeAmmoPack.properties = new Interactable.InteractableProperties();
        List<AgentData.AmmoPack> AmmoPackData = new List<AgentData.AmmoPack>();
        
        AmmoPackData.Add(AmmoStruct);

        fakeAmmoPack.AmmoPackData = AmmoPackData;
        agent.consume_ammo_pack(fakeAmmoPack);
        Finish();
    }


    #if UNITY_EDITOR

#endif
}

}


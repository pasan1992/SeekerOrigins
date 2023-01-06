using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
namespace HutongGames.PlayMaker.Actions
{

public class ActionSetPlayerState : FsmStateAction
{
    private PlayerController agentControl;
    public enum PLAYER_STATE {CORUCH,HOSTERWEAPON,ENABLE_COMBAT_UI,DISABLE_COMBAT_UI,DISABLE_CONTROL,ENABLE_CONTROL};
    public PLAYER_STATE Action;

    public override void OnEnter()
    {
        agentControl = PlayerController.getInstance();
        var hagent = ((HumanoidMovingAgent)agentControl.getICyberAgent());
        switch(Action)
        {
            case PLAYER_STATE.CORUCH:
                if(!hagent.isHidden())
                {
                    hagent.toggleHide();
                }
            break;
            case PLAYER_STATE.HOSTERWEAPON:
                hagent.hosterWeapon();
            break;     
            case PLAYER_STATE.ENABLE_COMBAT_UI:
                UICanvasHandler.getInstance().setActionHudStatus(true);
            break;
            case PLAYER_STATE.DISABLE_COMBAT_UI:
                UICanvasHandler.getInstance().setActionHudStatus(false);
            break;
            case PLAYER_STATE.DISABLE_CONTROL:
                agentControl.setEnabled(false);
            break;
            case PLAYER_STATE.ENABLE_CONTROL:
                agentControl.setEnabled(true);
            break;               
        }
        Finish();
    }   

}

}


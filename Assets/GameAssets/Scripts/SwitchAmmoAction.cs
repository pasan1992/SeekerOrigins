using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace HutongGames.PlayMaker.Actions
{
    public class SwitchAmmoAction : FsmStateAction
    {
        public AmmoTypeEnums.WeaponTypes weaponType;
        public string AmmoType;

        public HumanoidMovingAgent m_agent;
        public override void OnEnter()
        {
            m_agent.SwitchAmmoType(weaponType,AmmoType);
            Finish();
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace HutongGames.PlayMaker.Actions
{

public class SaveGameAction : FsmStateAction
{
    // Start is called before the first frame update
    public enum Action {SAVE,LOAD,REST};

    public Action action = Action.SAVE;
    public int checkpointID = 0;

    public override void OnEnter()
    {
        var svm = SaveGameManager.getInstance();
        switch(action)
        {
            case Action.SAVE:
                svm.SaveLevel(checkpointID);
            break;
            case Action.LOAD:
                //svm.StartLevel();
            break;
            case Action.REST:
                //svm.ResetLevel();
            break;
        }

        
    }
}
}

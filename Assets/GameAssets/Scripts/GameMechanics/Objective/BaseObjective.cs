using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class BaseObjective : MonoBehaviour
{
    // Start is called before the first frame update
    public string Objective;
    public TMP_Text m_ObjectiveText;
    public PlayMakerFSM externalFSM;
    public PlayMakerFSM storyFSM;
    public string OnObjectiveCompelteEvent;

    public virtual void StartObjective()
    {
        if(m_ObjectiveText !=null)
        {
            m_ObjectiveText.text = Objective;
        }

    }

    public virtual void onObjectiveComplete()
    {
        if (externalFSM == null)
        {
            PlayMakerFSM.BroadcastEvent(OnObjectiveCompelteEvent);
        }
        else
        {
            externalFSM.Fsm.Event(OnObjectiveCompelteEvent);
        }

        if(storyFSM != null)
        {
            storyFSM.Fsm.Event(OnObjectiveCompelteEvent);
        }
        this.enabled = false;
    }
}

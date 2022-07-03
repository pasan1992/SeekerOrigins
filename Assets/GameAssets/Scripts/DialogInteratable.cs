using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DialogActions {Explain =1,Shrug=2,No=3,Salute=3,Talk2=4,Warn=5,Talk1=6,Think=7,No_Action=-1}


public class DialogInteratable : Interactable
{
    // Start is called before the first frame update
    [System.Serializable]
    public class DialogStatment
    {
        public float startWaitTime;
        public string text;
        public AudioClip audio;
        public string charName;
        public float time;
        public DialogActions action = DialogActions.No_Action;
    }

    public bool disableOnEnd = false;
    public List<DialogStatment> Dialogs;
    public AudioSource m_audioSource;
    public Animator m_animator;

    private int m_nextDialogID = 0;
    private DialogStatment m_currentDialog;
    private float m_currentDialogTime = 0;

    public void Start()
    {
        //m_audioSource = m_audioSource ? this.GetComponent<AudioSource>() : m_audioSource;
    }

    private bool getNextDialog()
    {
        if(Dialogs.Count > m_nextDialogID)
        {
            var c_dialog = Dialogs[m_nextDialogID];
            m_nextDialogID +=1;
            m_currentDialog = c_dialog;
            return true;
        }
        m_currentDialog = null;
        return false;
    }

    public void SkipDialog()
    {
        m_currentDialogTime = float.MaxValue;
        m_audioSource.Stop();
    }

    public IEnumerator PlayDialog()
    {
        while(getNextDialog())
        {
            //DialogTextPanel.getInstance().setText(m_currentDialog.charName + ": " + m_currentDialog.text);
            m_currentDialogTime=0;
            if(m_currentDialog.audio !=null)
            {
                yield return new WaitForSeconds(m_currentDialog.startWaitTime);
                m_audioSource.PlayOneShot(m_currentDialog.audio);
            }
            if(m_animator != null)
            {
                if(m_currentDialog.action != DialogActions.No_Action)
                {
                    Debug.Log(m_currentDialog.action.ToString());
                    m_animator.SetTrigger(m_currentDialog.action.ToString());
                }
                
            }
            while(m_currentDialog.time > m_currentDialogTime)
            {
                yield return new WaitForSeconds(Time.deltaTime);
                m_currentDialogTime += Time.deltaTime;
            }
        }
        
        this.properties.interactionEnabled = disableOnEnd? false : true;
        this.enabled = disableOnEnd? false : true;
        gameObject.GetComponent<Interactable>().enabled = disableOnEnd ? true : false;
        //DialogTextPanel.getInstance().setText("");
        m_nextDialogID = 0;
    }

    public override void stopInteraction()
    {
        base.stopInteraction();
        m_nextDialogID = 0; 
    }
}

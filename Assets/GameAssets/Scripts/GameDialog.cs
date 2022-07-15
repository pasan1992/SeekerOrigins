using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDialog : MonoBehaviour
{
    // Start is called before the first frame update
    private DialogManager m_dialogManager;
    public List<DialogInteratable.DialogStatment> Dialog;
    void Start()
    {
        m_dialogManager = GameObject.FindObjectOfType<DialogManager>();
    }
    public void PushDialog()
    {
        //m_dialogManager.StartDialog(Dialog);
    }
}

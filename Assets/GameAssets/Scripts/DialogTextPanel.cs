using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogTextPanel : MonoBehaviour
{
    // Start is called before the first frame update
    private static DialogTextPanel dialogPanel;
    private Text m_text;

    public void Start()
    {
        m_text = GetComponentInChildren<Text>();
    }

    public static DialogTextPanel getInstance()
    {
        if(dialogPanel ==null)
        {
            dialogPanel = GameObject.FindObjectOfType<DialogTextPanel>();
        }

        return dialogPanel;
    }

    public void setText(string value)
    {
        m_text.text = value;
    }
}

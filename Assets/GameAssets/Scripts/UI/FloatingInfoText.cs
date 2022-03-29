using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FloatingInfoText : FloatingGameUI
{


    public Text m_text;


    void Awake()
    {
        //m_text = this.GetComponent<Text>();
    }

    public void setText(string name)
    {
        m_text.text = name;
    }

    public void setInteratableObject(Interactable interactable)
    {
        if(m_text !=null)
        {
            m_text.text = interactable.properties.itemName;
            m_offset = interactable.visualProperties.nameTagOffset;  
            target = interactable.transform; 
        }
    }

    public void resetText()
    {
        target = null;
        this.transform.position = Vector3.zero;
    }

    public void setTarget(Transform target,Vector3 offset)
    {
        this.target = target;
        this.m_offset = offset;
    }
}

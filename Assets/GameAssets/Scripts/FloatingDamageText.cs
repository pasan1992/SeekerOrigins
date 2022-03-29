using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class FloatingDamageText : FloatingUI
{
    public Text m_text;
    public float m_time;

    public void Start()
    {
        //m_text = this.GetComponentInChildren<Text>();
    }

    public void SetText(string value,Vector3 position,Color color)
    {
        m_text.text = value;
        m_text.color = color;
        StartCoroutine(WaitAndDisable(m_time));
        this.transform.position = position;
    }


    public void Update()
    {
        this.transform.Translate(new Vector3(0,0.02f,0));
    }

    IEnumerator WaitAndDisable(float time)
    {
        yield return new WaitForSeconds(time);
        this.gameObject.SetActive(false);
    }
}

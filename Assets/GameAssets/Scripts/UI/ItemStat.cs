using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemStat : MonoBehaviour
{
    // Start is called before the first frame update

    private LineRenderer m_line_rendere;

    public Vector3 offset;
    public Transform cornerPoint;

    public Text ItemName;
    public StatBar m_damageBar;
    public StatBar m_fireRateBar;

    public Transform[] OtherComponents;
    void Start()
    {
        m_line_rendere = this.GetComponent<LineRenderer>();
    }

    public void setDamage(float new_stat,float old_Stat)
    {
        m_damageBar.setCompareStat(new_stat,old_Stat);
    }

    public void setFireRate(float new_state, float old_Stat)
    {
        m_fireRateBar.setCompareStat(new_state,old_Stat);
    }

    public void setItemName(string name)
    {
        ItemName.text = name;
    }

    public void setVisibility(bool visible)
    {
        m_damageBar.gameObject.SetActive(visible);
        m_fireRateBar.gameObject.SetActive(visible);
        ItemName.gameObject.SetActive(visible);

        foreach(Transform obj in OtherComponents)
        {
            obj.gameObject.SetActive(visible);
        }
    }
    public void setItem(Transform itemLocation)
    {
        this.transform.position = itemLocation.transform.position + offset;
        m_line_rendere.SetPosition(0,cornerPoint.position);
        m_line_rendere.SetPosition(1,itemLocation.position);
    }
}

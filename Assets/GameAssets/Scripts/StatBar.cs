using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatBar : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform UpArrow;
    public Transform DownArrow;

    public Slider maxBar;
    public Image maxcolor;
    public Slider minBar;
    public Image minColor;
    public void setCompareStat(float new_stat,float old_Stat)
    {
        if(new_stat > old_Stat)
        {
            maxcolor.color = Color.green;
            minColor.color = Color.yellow;
            maxBar.value = new_stat/40 + 0.5f;
            minBar.value = old_Stat/40 + 0.5f;
            UpArrow.gameObject.SetActive(true);
            DownArrow.gameObject.SetActive(false);
        }
        else if(new_stat < old_Stat)
        {
            maxcolor.color = Color.red;
            minColor.color = Color.yellow;
            maxBar.value = old_Stat/40 + 0.5f;
            minBar.value = new_stat/40 + 0.5f;
            UpArrow.gameObject.SetActive(false);
            DownArrow.gameObject.SetActive(true);
        }
        else
        {
            
            maxcolor.color = Color.yellow;
            minColor.color = Color.yellow;
            maxBar.value = old_Stat/40 + 0.5f;
            minBar.value = new_stat/40 + 0.5f;
            UpArrow.gameObject.SetActive(false);
            DownArrow.gameObject.SetActive(false);
        }
    }

    public void setVisibility(bool visible){
        maxBar.gameObject.SetActive(visible);
        minBar.gameObject.SetActive(visible);
    }
}

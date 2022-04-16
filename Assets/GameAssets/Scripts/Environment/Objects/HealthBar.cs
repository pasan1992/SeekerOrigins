using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider healthbar;
    public bool AppearWhenDamaged = true;

    private float m_previousPercentage = 1;
    private float max_health = 0;

    public Sprite sheild_image;
    public Sprite half_sheild_image;
    public Image Sheild_icon;

    public Image Nameplate;

    private Image Healthfill;
    #region Initialize
    public void Start()
    {

        if (AppearWhenDamaged)
        {
            makeHealthBarInvisible();
        }
        set_no_cover();
        Healthfill = healthbar.GetComponentsInChildren<Image>()[1];
    }
    #endregion


    #region Update


    #endregion

    #region Getters and Setters
    /*
    public void setMaxHealth(float health)
    {
        max_health = health;
    }
    */

    //public float getMaxHealth()
    //{
    //    return m_maxHealth;
    //}

    //public void setHealth(float health)
    //{
    //    m_currentHealth = health;
    //    calculateHealthBar();
    //}

    //public float getHealth()
    //{
    //    return m_currentHealth;
    //}

    public void setHealthPercentage(float value)
    {
        if (AppearWhenDamaged)
        {
            OnlyEnableHealthBarWhenDamaged(value);
        }

        if (healthbar)
        {
            healthbar.value = value;
            //m_healtBar.color = Color.Lerp(Color.red, Color.green, value);
        }
    }

    public void setHealthPercentage(AgentBasicData data)
    {
        float value = 0;
        if (data.Sheild > 0)
        {
            value = data.Sheild / data.MaxSheild;
            Healthfill.color = Color.blue;
        }
        else
        {
            value = data.Health / data.MaxHealth;
            Healthfill.color = Color.red;
        }
        if (AppearWhenDamaged)
        {
            OnlyEnableHealthBarWhenDamaged(value);
        }

        if (healthbar)
        {
            healthbar.value = value;
            //m_healtBar.color = Color.Lerp(Color.red, Color.green, value);
        }
    }

    private void OnlyEnableHealthBarWhenDamaged(float percentage)
    {
        if (m_previousPercentage != percentage)
        {
            makeHealthBarVisible();
            CancelInvoke();
            Invoke("makeHealthBarInvisible", 1);
            m_previousPercentage = percentage;
        }
    }

    private void makeHealthBarVisible()
    {
        healthbar.gameObject.SetActive(true);
        Nameplate.gameObject.SetActive(true);
    }

    private void makeHealthBarInvisible()
    {
        healthbar.gameObject.SetActive(false);
        Nameplate.gameObject.SetActive(false);
    }

    public void set_full_cover()
    {
        Sheild_icon.enabled = true;
        Sheild_icon.sprite = sheild_image;
    }

    public void set_half_cover()
    {
        Sheild_icon.enabled = true;
        Sheild_icon.sprite = half_sheild_image;
    }

    public void set_no_cover()
    {
        Sheild_icon.enabled = false;
    }

    #endregion

    #region Events
    #endregion

    #region Utility
    #endregion
}

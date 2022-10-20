using UnityEngine;

public class DamageModule
{
    public delegate void OnDestoryDeligate();
    protected OnDestoryDeligate m_onDestroy;
    protected Outline m_outLine;

    protected AgentBasicData m_basicData;

    private float damaged_count;
    private GameEvents.BasicNotifactionEvent m_onHeal;

    public DamageModule(AgentBasicData basicData,OnDestoryDeligate onDestroyCallback,Outline outline)
    {
        m_basicData = basicData;
        m_onDestroy += onDestroyCallback;
        m_outLine = outline;
    }

    #region commands

    public virtual void destroyCharacter()
    {
    }
    #endregion

    #region getters and setters
    public virtual void resetCharacter()
    {
        m_outLine.enabled = true;
        m_basicData.Health = m_basicData.MaxHealth;
        m_outLine.OutlineColor = Color.Lerp(Color.red, Color.green, m_basicData.Health / m_basicData.MaxHealth);
    }

    public  virtual void stun(float duration)
    {

    }

    public void setHealth(float health)
    {
        if (m_basicData.Health == 0)
            return;

        m_basicData.Health = health;
        m_basicData.MaxHealth = health;
        m_outLine.OutlineColor = Color.Lerp(Color.red, Color.green, m_basicData.Health / m_basicData.MaxHealth);

        if (m_basicData.Health <= 0)
        {
            m_basicData.Health = 0;
            m_outLine.enabled = false;
            destroyCharacter();
            m_onDestroy();
        }
        
    }

    public void DamageByAmount(CommonFunctions.Damage damage)
    {
        damaged_count = m_basicData.HealthRegenWait;
        //setDamageText(amount,m_basicData.getAgentTransform().position,Color.yellow);
        var health_amount = damage.healthDamage;

        //Shield damage
        var sheild_amount = damage.energyDamage;
        m_basicData.Sheild = m_basicData.Sheild - sheild_amount;
        if (m_basicData.Sheild < 0 )
        {
            m_basicData.Sheild = 0;
        }

        if (m_basicData.Health == 0)
            return;

        if (health_amount > m_basicData.Sheild)
        {
            health_amount = health_amount - m_basicData.Sheild;
            
            m_basicData.Sheild = 0;
            
        }
        else
        {          
            m_basicData.Sheild = m_basicData.Sheild - health_amount;
            //setDamageText(amount,m_basicData.getAgentTransform().position,Color.blue);
            return;
        }



        m_basicData.Health -= health_amount;
        //setDamageText(amount,m_basicData.getAgentTransform().position,Color.red);


        if (m_basicData.Health <= 0)
        {
            m_basicData.Health = 0;
            destroyCharacter();
            m_onDestroy();
        }
    }

    public virtual void update()
    {
        regen_sheid();
        regen_health();
    }

    private void regen_health()
    {
         damaged_count -= Time.deltaTime;
        if(damaged_count < 0)
        {
            damaged_count = 0;
        }

        if(true)
        {
            if(damaged_count < m_basicData.HealthRegenWait/2)
            {
                m_basicData.Health += Time.deltaTime * m_basicData.HealthRegen * ( (m_basicData.HealthRegenWait - damaged_count)/m_basicData.HealthRegenWait);
                if(m_basicData.Health > m_basicData.MaxHealth)
                {
                    m_basicData.Health = m_basicData.MaxHealth;
                }

                if(m_onHeal !=null)
                {
                    m_onHeal();
                }
            }
        }       
    }

    private void regen_sheid()
    {
        damaged_count -= Time.deltaTime*m_basicData.Regen;
        if(damaged_count < 0)
        {
            damaged_count = 0;
        }

        if(damaged_count == 0)
        {
            if(m_basicData.Sheild < m_basicData.MaxSheild)
            {
                m_basicData.Sheild += Time.deltaTime * m_basicData.Regen;
                if(m_basicData.Sheild > m_basicData.MaxSheild)
                {
                    m_basicData.Sheild = m_basicData.MaxSheild;
                }

                if(m_outLine)
                m_outLine.OutlineColor = Color.Lerp(Color.red, Color.green, m_basicData.Health / m_basicData.MaxHealth);
            }
        }
    }

    private void setDamageText(float damage, Vector3 position, Color color)
    {
        var obj = ProjectilePool.getInstance().getPoolObject(ProjectilePool.POOL_OBJECT_TYPE.DamageText);
        obj.SetActive(true);
        if(obj != null)
        {
            var dmg_text = obj.GetComponent<FloatingDamageText>();
            dmg_text.SetText("+" +damage.ToString(),position,color);
        }
    }

    public virtual bool HealthAvailable()
    {
        return m_basicData.Health > 0;
    }

    public Color getHealthColor()
    {
       return m_outLine.OutlineColor;
    }

    public float getHealthPercentage()
    { 
        if(m_basicData.Health != 0)
        {
            return m_basicData.Health / m_basicData.MaxHealth;
        }
        else
        {
            return 0;
        }
    }

    public void setOnHealCallback(GameEvents.BasicNotifactionEvent callback)
    {
        m_onHeal +=callback;
    }


    #endregion
}

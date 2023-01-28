using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionalObjective : MonoBehaviour
{
    private static OptionalObjective instance;
    public static OptionalObjective Instance { get { return instance; } }

    private Dictionary<string,string> m_objectives;

    public void  Awake() {

        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }

        m_objectives = new Dictionary<string, string>();
    }

    public void setObjective(string obj,string value)
    {
        m_objectives[obj] = value;
    }

    public string getObjectiveValue(string obj)
    {
        string value = "NONE";
        m_objectives.TryGetValue(obj,out value);
        return value;
    }

    public List<string> GetObjectiveNames()
    {
        return new List<string>(m_objectives.Keys);
    }

}

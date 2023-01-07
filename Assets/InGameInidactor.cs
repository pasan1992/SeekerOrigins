using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameInidactor : MonoBehaviour
{
    // Start is called before the first frame update
    public enum IndicatorTypes {Ammo,Kill,Question,Health,Heart,Skull,Arrow}
    private IndicatorTypes type;
    

    public IndicatorTypes IndicatorType
    {
        set 
        {
            SetInidactorType(value);
            type = value;
        }

        get => type;
    }



    public GameObject ammo;
    public GameObject kill;
    public GameObject health;
    public GameObject heart;
    public GameObject question;
    public GameObject Skull;
    public GameObject Arrow;


    private void SetInidactorType(IndicatorTypes type )
    {
        ammo.SetActive(false);
        kill.SetActive(false);
        health.SetActive(false);
        heart.SetActive(false);
        question.SetActive(false);
        Skull.SetActive(false);
        Arrow.SetActive(false);

        switch(type){
            case IndicatorTypes.Ammo:
                ammo.SetActive(true);
            break;
            case IndicatorTypes.Kill:
                kill.SetActive(true);
            break;
            case IndicatorTypes.Question:
                question.SetActive(true);
            break;
            case IndicatorTypes.Heart:
                health.SetActive(true);
            break;
            case IndicatorTypes.Skull:
                Skull.SetActive(true);
            break;
            case IndicatorTypes.Health:
                health.SetActive(true);
            break;
            case IndicatorTypes.Arrow:
                Arrow.SetActive(true);
                break;
        }
    }

    void Start()
    {
        IndicatorType = type;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoPack : Interactable
{
    // Start is called before the first frame update
    public string AmmoType = "RifleAmmo";
    public int count = 10;
    void Start()
    {
        properties.Type = InteractableProperties.InteractableType.PickupInteraction;
    }
}

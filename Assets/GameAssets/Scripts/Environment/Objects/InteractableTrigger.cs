using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableTrigger : MonoBehaviour
{
    PlayerController player;
    Interactable interactable;
    public float InteractionRadious =1;
    public bool trig_enabled = true;
    public void Start()
    {
        player = FindObjectOfType<PlayerController>();
        interactable = GetComponent<Interactable>();
    }

    public void Update()
    {
        if(trig_enabled && player)
        {
            var distance = Vector3.Distance(player.transform.position, this.transform.position);

            if(distance < InteractionRadious)
            {
                if(interactable is AmmoPack)
                {
                    ((HumanoidMovingAgent)player.getICyberAgent()).consume_ammo_pack((AmmoPack)interactable);
                }
                else
                {
                    player.getICyberAgent().interactWith(interactable, interactable.properties.Type);
                }
                trig_enabled = false;
            }
        }
    }
    public void reset_trigger()
    {
        trig_enabled = true;
    }
}

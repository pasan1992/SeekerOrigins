using UnityEngine;

public interface DamagableObject
{
    Transform getTransfrom();

    // Return true if destroyed
    // fromFaction is not used atm
    bool damage(CommonFunctions.Damage damageValue,Collider collider, Vector3 force, Vector3 point,AgentBasicData.AgentFaction fromFaction,float dot_time = 0);

    bool isDestroyed();

    float getRemaningHealth();

    float getTotalHealth();

    float getArmor();

    void setOnDamaged(GameEvents.BasicNotifactionEvent onDamaged);

    void setOnDestroyed(GameEvents.BasicNotifactionEvent onDestroyed);

    void resetObject();

    void damage_effect(Transform position);

}

public class AmmoTypeEnums
{
    public enum WeaponTypes
    {
        Missile,
        Grenade,
        RegenPack,
        Rifle,
        Pistol
    }
    public enum Missile
    {
        DroneBusters_Missile,
        MiniNuke_Missile,
    }

    public enum Grenade
    {
        Regular_Grenade,
        EMP_Grenade,
        ProximityTrap_Grenade,
    }

    public enum RegenPack
    {
        Regular_HealthPack,
    }



    public enum RifleAmmo
    {
        Regular_RifleAmmo,
        Energy_RifleAmmo,
        Incendiary_RifleAmmo,
        Highcaliber_RifleAmmo,
    }

    public enum PistolAmmo
    {
        Regular_PistolAmmo,
        Energy_PistolAmmo,
        Charge_PistolAmmo,
    }

    public static RifleAmmo getRifleAmmoType(string name)
    {
        switch(name)
        {
            case "Regular_RifleAmmo":
                return RifleAmmo.Regular_RifleAmmo;
            case "Energy_RifleAmmo":
                return RifleAmmo.Energy_RifleAmmo;
             case "Incendiary_RifleAmmo":
                return RifleAmmo.Incendiary_RifleAmmo;       
            case "Highcaliber_RifleAmmo":
                return RifleAmmo.Highcaliber_RifleAmmo;      
            default:
                return RifleAmmo.Regular_RifleAmmo; 
        }
    }

    public static PistolAmmo getPistolAmmoType(string name)
    {
        switch(name)
        {
            case "Regular_PistolAmmo":
                return PistolAmmo.Regular_PistolAmmo;
            case "Energy_RifleAmmo":
                return PistolAmmo.Energy_PistolAmmo;      
            case "Charge_PistolAmmo":
                return PistolAmmo.Charge_PistolAmmo;   
            default:
                return PistolAmmo.Regular_PistolAmmo;                      
        }
    }

    //Mag
    //    Missile
    //    RifleAmmo

}

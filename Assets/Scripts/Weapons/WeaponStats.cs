using UnityEngine;

public enum AmmoType { Pistol, Shotgun, Rifle }

/// <summary>
/// Stats of the weapon entity. All it does is storing values that can be
/// useful for WeaponLogic or other scripts
/// </summary>
public class WeaponStats : MonoBehaviour
{
    [Header("Name")]
    [SerializeField]
    private string _name;

    [Header("Object")]
    public Vector3 firstPersonSize;

    public string Name { get { return _name; } }

    [Header("Damage")]
    public int Damage;
    public int DamageNPC;

    [Header("Reload Time")]
    public float ReloadTime;

    [Header("Rate Of Fire")]
    public float RateOfFire; // Rate of fire in seconds
    public float RateOfFireNPC;

    [Header("Spread")]
    public float Spread;
    public float SpreadNPC;

    [Header("Range")]
    public float Range;
    public float RangeNPC;

    [Header("Slot and Index")]
    public KeyCode WeaponSlot; // Which input should the player press to equip the weapon?

    public int weaponIndex;

    [Header("Ammo")]
    [Min(0)]
    [SerializeField]
    private int _mag;

    public int Mag { get { return _mag; } }

    [HideInInspector]
    public int currentAmmo;

    [SerializeField]
    private AmmoType ammoType;

    public AmmoType AmmoType { get { return ammoType; } }

    [HideInInspector]
    public WeaponLogic logic;

    [HideInInspector]
    public InteractionController interact;

    [HideInInspector]
    public bool isPlayerEquipped = false;

    private void Awake()
    {
        currentAmmo = _mag;

        logic = GetComponent<WeaponLogic>();

        interact = GetComponent<InteractionController>();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The man himself
/// </summary>
public class Player : MonoBehaviour
{
    #region Fields and properties

    [Min(0)]
    [SerializeField]
    private float health;

    public float Health { get { return health; } }
    
    [Min(0)]
    [SerializeField]
    private float maxHealth;

    public float MaxHealth { get { return maxHealth; } }

    [SerializeField]
    Transform FeetPosition; // Used to know where to place the camera when the player dies

    [SerializeField]
    private WeaponStats[] StartWeapons; // Weapons that'll spawn with the player

    [SerializeField]
    private GameObject WeaponPlacement;

    private KeyCode[] weaponInput = new KeyCode[]
    {
        KeyCode.Alpha1,
        KeyCode.Alpha2,
        KeyCode.Alpha3,
    };

    // Dictionary containing the key inputs and the weapons. This is used when the player
    // has two weapons in the same slot
    private Dictionary<KeyCode, List<WeaponStats>> weapons = new Dictionary<KeyCode, List<WeaponStats>>();

    // Dictionary containing the weapons' Game Objects. Used only for manipulating the Game Object itself.
    private Dictionary<WeaponStats, GameObject> weaponObjects = new Dictionary<WeaponStats, GameObject>();

    private WeaponStats currentWeapon;

    // The ammo that the player is currently carrying
    private Dictionary<AmmoType, int> currentAmmo = new Dictionary<AmmoType, int>()
    {
        { AmmoType.Pistol, 20 },
        { AmmoType.Rifle, 30 },
        { AmmoType.Rifle2, 0 },
        { AmmoType.Revolver, 0 }
    };

    // Read-only property of the currentAmmo dictionary
    public Dictionary<AmmoType, int> CurrentAmmo { get { return currentAmmo; } }

    // The maximum ammo that the player can carry
    static private Dictionary<AmmoType, int> maxAmmo = new Dictionary<AmmoType, int>()
    {
        { AmmoType.Pistol, 120 },
        { AmmoType.Shotgun, 30 },
        { AmmoType.Rifle, 150 },
        { AmmoType.Rifle2, 150 },
        { AmmoType.Revolver, 36 }
    };
    
    private PlayerMovement PlayerMovement;

    private Camera Cam;

    static public bool isDead = false;

    #endregion

    #region Unity Methods

    void Awake()
    {
        isDead = false;

        PlayerMovement = GetComponent<PlayerMovement>();
        
        Cam = GetComponentInChildren<Camera>();

        InitializeWeapons();
    }

    private void Start()
    {
        RefreshGUI();
    }

    private KeyCode previousKey; // Will be used for weapon selection
    private int weaponIndex = 0;

    void Update()
    {
        if(currentWeapon == null || currentWeapon.logic.CanSwitch)
        {
            foreach (KeyCode key in weaponInput)
            {
                if (Input.GetKeyDown(key))
                {

                    if(key == previousKey)
                    {
                        weaponIndex++;

                        if (weaponIndex >= weapons[key].Count) weaponIndex = 0;
                    }
                    else
                    {
                        weaponIndex = 0;
                    }

                    if (weapons[key].Count > 0) EquipWeapon(weapons[key][weaponIndex]);

                    previousKey = key;
                }
            }
        }

        if(currentWeapon != null)
        {
            if(Input.GetKeyDown(KeyCode.Mouse0))
            {
                currentWeapon.logic.ShootDown();
            }
            else if (Input.GetKey(KeyCode.Mouse0))
            {
                currentWeapon.logic.Shoot(Cam.transform.position, Cam.transform.forward);

                RefreshGUI();
            }
            else if (Input.GetKeyUp(KeyCode.Mouse0))
            {
                currentWeapon.logic.ShootUp();
            }
            else if(Input.GetKeyDown(KeyCode.R))
            {
                currentWeapon.logic.Reload();
            }
        }
    }

    #endregion

    #region GUI

    public void RefreshGUI()
    {
        GUI_Logic gui = GameManager.singleton.GUI;

        gui.Health.text = ((int)health).ToString();

        if(health >= maxHealth / 2)
        {
            gui.Health.color = Color.Lerp(Color.yellow, Color.green, (health - maxHealth / 2) / (maxHealth / 2));
        }
        else
        {
            gui.Health.color = Color.Lerp(Color.red, Color.yellow, health / (maxHealth / 2));
        }

        if (currentWeapon != null)
        {
            gui.CurrentMag.text = currentWeapon.currentAmmo.ToString();

            gui.MagSize.text = currentWeapon.Mag.ToString();

            gui.CurrentAmmo.text = currentAmmo[currentWeapon.AmmoType].ToString();
        }
    }

    #endregion

    #region Health

    public void AddHealth(float value)
    {
        if (health + value >= maxHealth) health = maxHealth;
        else health += value;

        RefreshGUI();
    }

    public void RemoveHealth(float value)
    {
        AddHealth(-value);

        if (health < 1) StartCoroutine(Kill());
    }

    private IEnumerator Kill()
    {
        PlayerMovement.Stand(); // Making him stand is necessary, otherwise the camera ends up below the ground

        yield return null; // Making sure the player is standing

        Destroy(PlayerMovement);

        // Rolls the camera to make it look like the player is lying down on the ground
        Cam.transform.rotation = Quaternion.Euler(0f, 0f, 90f);

        Cam.transform.position = FeetPosition.position;
        //

        if(currentWeapon != null) Destroy(currentWeapon.gameObject);

        isDead = true;

        GameManager.singleton.GameOver(false);

        Destroy(this);
    }

    #endregion

    #region Weapons and ammo

    public void AddAmmo(AmmoType ammoType, int value)
    {
        if(!currentAmmo.ContainsKey(ammoType))
        {
            currentAmmo.Add(ammoType, value);
        }
        else
        {
            currentAmmo[ammoType] += value;
        }
    }

    private void InitializeWeapons()
    {
        foreach (AmmoType ammo in maxAmmo.Keys)
        {
            AddAmmo(ammo, 0);
        }

        // This dictionary is used to first store the starting weapons in a hashset to make sure we don't
        // get the same weapon twice, then copy the contents in the main dictionary, which contains lists instead.
        // It's probably not the best way to do it, but it would have been easier if you could access Hashset contents
        // with indexes. Unfortunately, you can't.
        var tempWeapons = new Dictionary<KeyCode, HashSet<WeaponStats>>();

        foreach (KeyCode key in weaponInput)
        {
            tempWeapons.Add(key, new HashSet<WeaponStats>());

            weapons.Add(key, new List<WeaponStats>());
        }



        foreach (WeaponStats weapon in StartWeapons)
        {
            if (weapon == default) continue;

            foreach (KeyCode key in weaponInput)
            {
                if (weapon.WeaponSlot == key)
                {
                    tempWeapons[weapon.WeaponSlot].Add(weapon);
                    break;
                }
            }
        }

        foreach (KeyCode key in tempWeapons.Keys)
        {
            foreach (WeaponStats weapon in tempWeapons[key])
            {
                CreateWeaponInstance(weapon);
            }
        }
    }

    /// <summary>
    /// Gives a new weapon to the player
    /// </summary>
    /// <param name="weapon"></param>
    public void GiveWeapon(WeaponStats weapon)
    {
        foreach(WeaponStats _weapon in weapons[weapon.WeaponSlot])
        {
            if (weapon.Name == _weapon.Name && weapon.AmmoType == _weapon.AmmoType)
            {
                AddAmmo(weapon.AmmoType, weapon.currentAmmo);

                RefreshGUI();

                return;
            }
        }

        CreateWeaponInstance(weapon);
    }

    /// <summary>
    /// Equips a weapon that the player already has
    /// </summary>
    /// <param name="weapon"></param>
    void EquipWeapon(WeaponStats weapon)
    {
        foreach(Transform _object in WeaponPlacement.transform)
        {
            _object.gameObject.SetActive(false);
        }

        if(!weaponObjects.ContainsKey(weapon))
        {
            CreateWeaponInstance(weapon);
        }

        weaponObjects[weapon].SetActive(true);

        currentWeapon = weapon;

        RefreshGUI();
    }

    /// <summary>
    /// Creates the actual Game Object of the weapon
    /// </summary>
    /// <param name="weapon"></param>
    private void CreateWeaponInstance(WeaponStats weapon)
    {
        GameObject newWeapon = Instantiate(weapon.gameObject, WeaponPlacement.transform);

        newWeapon.transform.position = WeaponPlacement.transform.position;
        newWeapon.transform.rotation = WeaponPlacement.transform.rotation;

        newWeapon.transform.localScale = weapon.firstPersonSize;

        weapon = newWeapon.GetComponent<WeaponStats>();

        weapon.logic.clearPickup();

        foreach(KeyCode key in weapons.Keys)
        {
            if (weapon.WeaponSlot == key)
            {
                weapons[key].Add(weapon);
                break;
            }
        }

        weaponObjects.Add(weapon, newWeapon);

        newWeapon.SetActive(false);

        SwitchLayers(newWeapon.transform);

        if(weapon.currentAmmo <= 0 || weapon.currentAmmo > weapon.Mag) weapon.currentAmmo = weapon.Mag;

        weapon.isPlayerEquipped = true;

        SortWeapons();
    }

    /// <summary>
    /// Sort the weapons in each slot from their weapon index. If two weapons have
    /// the same index, one will override the other.
    /// </summary>
    private void SortWeapons()
    {
        foreach(List<WeaponStats> slot in weapons.Values)
        {
            for(int i = 0; i < slot.Count; i++)
            {
                // To prevent an out-of-range
                while(slot[i].weaponIndex >= slot.Count)
                {
                    slot.Add(default);
                }

                if(i != slot[i].weaponIndex)
                {
                    WeaponStats temp = slot[slot[i].weaponIndex];

                    slot[slot[i].weaponIndex] = slot[i];
                    slot[i] = temp;
                }
            }

            for(int i = slot.Count - 1; i >= 0; i--)
            {
                if (slot[i] == default) slot.RemoveAt(i);
            }
        }
    }

    /// <summary>
    /// Sets the weapon's layer to First-Person
    /// </summary>
    /// <param name="_object"></param>
    private void SwitchLayers(Transform _object)
    {
        _object.gameObject.layer = 9;
        foreach(Transform subObject in _object)
        {
            SwitchLayers(subObject);
        }
    }

    #endregion
}

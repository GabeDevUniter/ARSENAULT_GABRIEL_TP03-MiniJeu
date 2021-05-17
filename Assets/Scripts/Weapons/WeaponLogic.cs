using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Brain of the weapon entity. Takes care of the effects, trajectory, reloading,
/// hit detection, etc.
/// </summary>
public class WeaponLogic : MonoBehaviour
{
    #region Fields and properties

    public string Name { get { return stats.Name; } }

    [Header("Particles")]
    [SerializeField]
    private ParticleSystem muzzleFlash;

    [SerializeField]
    private GameObject bloodImpact;

    [SerializeField]
    private GameObject bulletImpact;

    [Header("Sound Effects")]
    [SerializeField]
    private AudioSource audio_fire;

    [SerializeField]
    private AudioSource audio_reload;

    // Audio Range
    private AudioRange AudioRange;
    //

    // Misc
    private WeaponStats stats;

    private Collider hitCollider = null;

    Dictionary<AmmoType, int> playerAmmo;
    //

    // Conditions
    private bool isShooting = false;

    private bool isReloading = false;

    public bool IsHit { get; private set; } = false;

    public bool CanSwitch { get { return !isShooting && !isReloading; } }
    //

    #endregion

    #region Unity Methods

    // Start is called before the first frame update
    void Awake()
    {
        stats = GetComponent<WeaponStats>();

        AudioRange = GetComponent<AudioRange>();
    }

    private void Start()
    {
        // In the Start() to make sure the player is instantiated before getting his current ammo
        playerAmmo = GameManager.singleton.PlayerLogic.CurrentAmmo;
    }

    #endregion

    #region Give Weapon

    /// <summary>
    /// Giving the current weapon to the player. Used for item drops
    /// </summary>
    public void GiveToPlayer()
    {
        GameManager.singleton.PlayerLogic.GiveWeapon(stats);
    }

    #endregion

    #region Shoot

    /// <summary>
    /// When shooting, only plays the particle effect once
    /// </summary>
    public void ShootDown()
    {
        if (isShooting || isReloading || stats.currentAmmo <= 0) return;

        AudioRange.Trigger();

        muzzleFlash.Play();
    }

    /// <summary>
    /// Shoots as long as the player holds the left mouse button
    /// </summary>
    /// <param name="start">Origin of the bullet</param>
    /// <param name="direction">Direction of the bullet</param>
    public void Shoot(Vector3 start, Vector3 direction)
    {
        if (isShooting || isReloading || stats.currentAmmo <= 0) return;

        HitDetect(start, direction);

        StartCoroutine(ShootCoroutine());
    }

    /// <summary>
    /// Stops the muzzle flash when the player lets go of the left mouse button
    /// </summary>
    public void ShootUp()
    {
        muzzleFlash.Stop();
    }

    /// <summary>
    /// Plays the muzzle flash, decrement the weapon's current mag, plays the audio, then wait before being able to shoot again.
    /// </summary>
    private IEnumerator ShootCoroutine()
    {
        isShooting = true;
        
        if(!muzzleFlash.isPlaying) muzzleFlash.Play();

        stats.currentAmmo -= 1;

        audio_fire.Play();

        yield return new WaitForSeconds(stats.RateOfFire);

        isShooting = false;
    }

    #endregion

    #region Reloading

    /// <summary>
    /// Starts the Reloading() coroutine
    /// </summary>
    public void Reload()
    {
        if (isShooting || isReloading || stats.currentAmmo == stats.Mag || playerAmmo[stats.AmmoType] == 0) return;

        StartCoroutine(Reloading());
    }

    /// <summary>
    /// Main Reloading() method. Will play the audio effect, execute the reload time
    /// and finally replenish the ammo.
    /// </summary>
    /// <returns></returns>
    private IEnumerator Reloading()
    {
        isReloading = true;

        audio_reload.Play();

        GameManager.singleton.GUI.Reloading.enabled = true;

        yield return new WaitForSeconds(stats.ReloadTime);

        // Take whatever's left of the mag
        int remain = stats.Mag - stats.currentAmmo;

        if(playerAmmo[stats.AmmoType] >= remain)
        {
            playerAmmo[stats.AmmoType] -= remain;

            stats.currentAmmo = stats.Mag;
        }
        else // If the current ammo is lower than the mag, take whatever's left of the current ammo
        {
            stats.currentAmmo += playerAmmo[stats.AmmoType];

            playerAmmo[stats.AmmoType] = 0;
        }

        GameManager.singleton.PlayerLogic.RefreshGUI();

        GameManager.singleton.GUI.Reloading.enabled = false;

        isReloading = false;
    }

    #endregion

    #region Hit Detection

    /// <summary>
    /// Main hit detection method. Takes care of the bullet trajectory, find out who's hit and
    /// decides what happens when they're hit.
    /// </summary>
    /// <param name="start">Origin of the bullet</param>
    /// <param name="direction">Direction of the bullet</param>
    private void HitDetect(Vector3 start, Vector3 direction)
    {
        float spread = Random.Range(-stats.Spread, stats.Spread);

        direction.x += spread;
        direction.z += spread;

        // Happy little binary logics to define which layers should react to the bullets
        int layermask = 1 << LayerMask.NameToLayer("Player") |
            1 << LayerMask.NameToLayer("Item") |
            1 << LayerMask.NameToLayer("Default") |
            1 << LayerMask.NameToLayer("NPC") |
            1 << LayerMask.NameToLayer("Hitboxes");

        if (Physics.Raycast(start, direction, out RaycastHit hit, 50f, layermask))
        {
            _start = start;
            _dir = direction;

            _hitPoint = hit.point;

            
            hitCollider = hit.collider;
            
            if(hitCollider != null && hitCollider.isTrigger == false)
            {
                ////////////////////////////////////////////////////////////////////////////

                // Target is the player
                if (hitCollider.CompareTag("Player") && !stats.isPlayerEquipped)
                {
                    GameManager.singleton.PlayerLogic.RemoveHealth(stats.DamageNPC);
                }

                ////////////////////////////////////////////////////////////////////////////

                // Target is an NPC
                else if (hitCollider.CompareTag("NPC"))
                {
                    Grunt grunt = hitCollider.GetComponentInParent<Grunt>();

                    if(grunt != null)
                    {
                        DamageMultiplier multiplier = hitCollider.GetComponent<DamageMultiplier>();

                        float _multiplier = multiplier != null ? multiplier.Multiplier : 1f;


                        grunt.RemoveHealth(stats.Damage * _multiplier);

                        // Gives a little push to the NPC when shot. Only effective when dead.
                        if(grunt.IsDead)
                        {
                            foreach (Rigidbody bone in grunt.Ragdoll)
                            {
                                bone.AddExplosionForce(15f, hit.point, 1f, 0.2f, ForceMode.Impulse);
                            }
                        }
                        

                        PlayParticle(bloodImpact, start, hit.point);
                    }
                }

                ////////////////////////////////////////////////////////////////////////////

                // Target is an explosive
                else if (hitCollider.CompareTag("Explosive"))
                {
                    Explosion explosion = hitCollider.GetComponent<Explosion>();

                    if (explosion != null) explosion.RemoveHealth(stats.Damage);
                }

                ////////////////////////////////////////////////////////////////////////////

                // Target is anything that wasn't mentioned above, like walls for instance
                else
                {
                    PlayParticle(bulletImpact, start, hit.point);
                }
            }
        }
    }

    #endregion

    /// <summary>
    /// Clear any components related to interaction
    /// </summary>
    public void clearPickup()
    {
        Destroy(GetComponent<Rigidbody>());

        Destroy(GetComponent<BoxCollider>());

        Destroy(GetComponent<InteractionController>());
    }

    /// <summary>
    /// Emits the impact particle, whether it be a bullet impact or blood impact
    /// </summary>
    /// <param name="particle">Particle to emit</param>
    /// <param name="start">Origin of the bullet</param>
    /// <param name="end">End of the bullet</param>
    private void PlayParticle(GameObject particle, Vector3 start, Vector3 end)
    {
        Vector3 tempStart = start;

        tempStart -= end;

        GameObject impact = Instantiate(particle, end, Quaternion.LookRotation(tempStart, Vector3.up));
        impact.GetComponent<ParticleSystem>().Play();
    }

#if true

    Vector3 _start;
    Vector3 _dir;
    Vector3 _hitPoint;
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(_start, _dir);

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(_start, _hitPoint);
    }

#endif

}

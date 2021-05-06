using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Main class for the NPC. Takes care of the NPC's behaviours and states
/// </summary>
public class Grunt : Statemachine
{
    #region Fields and properties

    static private int GruntCount = 0;

    public bool IsDead { get; private set; } = false;

    [Header("Basic Settings")]
    [SerializeField]
    private float Health = 35;

    [Header("Field of vision")]
    public float fieldAngle = 50f;
    public float fieldHeight = 10f;
    public float fieldDistance = 20f;

    [Header("Transforms")]
    public Transform EyeTransform;
    public Transform BulletStart;

    [Header("Weapons")]
    public WeaponStats currentWeapon;
    public GameObject WeaponDrop;

    [Header("Dialogs")]
    [SerializeField]
    private DialogDirector dialogs_Idle;

    public DialogDirector Dialogs_Idle { get { return dialogs_Idle; } }

    [SerializeField]
    private DialogDirector dialogs_Alert;

    public DialogDirector Dialogs_Alert { get { return dialogs_Alert; } }

    [SerializeField]
    private DialogDirector dialogs_Death;

    public DialogDirector Dialogs_Death { get { return dialogs_Death; } }

    //

    // Ragdoll

    private RagdollController RagdollController;

    public Rigidbody[] Ragdoll { get { return RagdollController.Ragdoll; } }

    //

    // Components
    private NavMeshAgent agent;
    private NPCMovement movement;
    private Animator animator;

    public NavMeshAgent Agent { get { return agent; } }
    public NPCMovement Movement { get { return movement; } }

    //

    // Player

    public Vector3 PlayerHead { get { return GameManager.singleton.PlayerHead; } }

    static private Vector3 tempPlayerHead;
    static private Quaternion TurnRotation;

    private RaycastHit playerHit;

    //

    // Audio Range

    private AudioRange audioRange;

    public AudioRange AudioRange { get { return audioRange; } }

    //

    #endregion

    #region Unity Methods

    void Awake()
    {
        GruntCount++;

        agent = GetComponent<NavMeshAgent>();
        movement = GetComponent<NPCMovement>();
        animator = GetComponent<Animator>();

        audioRange = GetComponent<AudioRange>();

        RagdollController = GetComponentInChildren<RagdollController>();

        currentWeapon.RateOfFire = currentWeapon.RateOfFireNPC;
        currentWeapon.Spread = currentWeapon.SpreadNPC;

        Dictionary<System.Type, BaseState> states = new Dictionary<System.Type, BaseState>
        {
            {typeof(IdleState), new IdleState(this) },
            {typeof(AlertState), new AlertState(this) },
            {typeof(CombatState), new CombatState(this) }
        };

        InitializeStates(states);
    }

    void Start()
    {
        RagdollController.SetRagdoll(false);

        SetState(typeof(IdleState));
    }

    void Update()
    {
        StateTick();
    }

    #endregion

    #region NPC Methods

    public void _SetState(System.Type state)
    {
        SetState(state);
    }


    /// <summary>
    /// Checks if the player is in sight while in idle state
    /// </summary>
    public bool PlayerDetect()
    {
        if (Physics.Linecast(EyeTransform.position, PlayerHead, out playerHit))
        {
            if (playerHit.collider.CompareTag("Player"))
            {
                Vector3 pointA = transform.forward;
                Vector3 pointB = PlayerHead - EyeTransform.position;

                float height = Mathf.Abs(pointB.y - pointA.y);

                float distance = Vector3.Distance(pointA, pointB);

                pointB.y = pointA.y;
                float angle = Vector3.Angle(pointA, pointB);

                return height <= fieldHeight && distance <= fieldDistance && angle <= fieldAngle;
            }
        }
        return false;
    }

    /// <summary>
    /// Checks if the NPC both sees the player and is in range.
    /// </summary>
    public bool Aim()
    {
        bool inSight = false;

        if (Physics.Linecast(EyeTransform.position, PlayerHead, out playerHit))
        {
            inSight = playerHit.collider.CompareTag("Player");
        }

        bool inRange = Vector3.Distance(transform.position, PlayerHead) <= currentWeapon.RangeNPC;

        return inSight && inRange;
    }

    public void Shoot()
    {
        currentWeapon.currentAmmo = 1; // Make sure the NPC doesn't have to reload

        Vector3 dir = PlayerHead - BulletStart.position;

        currentWeapon.logic.Shoot(BulletStart.position, dir);
    }

    public void FacePlayer(bool move)
    {
        tempPlayerHead = PlayerHead;
        tempPlayerHead.y = transform.position.y;

        tempPlayerHead -= transform.position;
        TurnRotation = Quaternion.LookRotation(tempPlayerHead, Vector3.up);

        movement.FaceDirection_wrap(TurnRotation, 0.4f);

        if (move) movement.MoveUpdate(PlayerHead);
    }

    public bool IsFacingPlayer()
    {
        return transform.rotation.y <= TurnRotation.y + 0.02f && transform.rotation.y >= TurnRotation.y - 0.02f;
    }

    public void RemoveHealth(float value)
    {
        Health -= value;

        //Debug.Log($"{value} damage inflicted!");

        if (Health <= 0)
        {
            Kill();
            return;
        }

        if (CurrentState.GetType() == typeof(IdleState))
        {
            SetState(typeof(AlertState));
        }
    }

    public void Kill()
    {
        if (IsDead) return;

        IsDead = true;

        StopAllCoroutines();

        movement.StopAllCoroutines();

        Destroy(movement);

        Destroy(animator);

        Destroy(agent);

        Destroy(audioRange);

        // Weapon pickup
        currentWeapon.interact.isInteractable = true;

        currentWeapon.interact.ColliderEnabled = true;

        currentWeapon.gameObject.GetComponent<Rigidbody>().isKinematic = false;

        Instantiate(currentWeapon.gameObject, currentWeapon.transform.position, currentWeapon.transform.rotation);
        
        Destroy(currentWeapon.gameObject);
        //

        RagdollController.SetRagdoll(true);
        RagdollController.startCooldown();

        dialogs_Idle.MuteAll();
        dialogs_Alert.MuteAll();
        dialogs_Death.Play();

        GruntCount--;

        if (GruntCount == 0) GameManager.singleton.GameOver(true);

        Destroy(this);
    }

    #endregion

}

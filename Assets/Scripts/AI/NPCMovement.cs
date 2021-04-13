using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Second variant of the NPC movement script. This script will be in charge of literally anything
/// related to manipulating the NPC's position, rotation, animations, etc.
/// </summary>
public class NPCMovement : MonoBehaviour
{
    #region Fields and properties

    [Header("Speed")]
    public float walkSpeed = 3f;
    public float runSpeed = 5f;
    public float turnDuration = 1f;
    public float turnPointDuration = 5f;

    [Header("Patrol")]
    public PatrolPoint startingPoint;

    private float currentSpeed;

    /////////////////////////////////////////////////////////////////////////////

    // Patrol coroutine and its conditions
    private Coroutine patrol;
    private bool isPatrolling = false;
    private bool isArrived = false;
    private bool isJumping = false;

    // Patrol Point and its properties
    private PatrolPoint nextPoint;
    private Vector3 NextPosition { get { return nextPoint.transform.position; } }
    private Quaternion NextRotation { get { return nextPoint.transform.rotation; } }

    /////////////////////////////////////////////////////////////////////////////

    // AI Components
    private NavMeshAgent agent;
    private Animator animator;
    private OffMeshLinkData currentLink;

    /////////////////////////////////////////////////////////////////////////////

    // Animation values
    public enum MoveMode { Idle=0, Alert=1 }

    public enum MovementTypes { Idle = 0, Walk = 1, Run = 2 };

    private const float idleAnim = 0f;

    private const string moveAnimParam = "Move";
    private const string jumpAnimParam = "Jump";

    private float currentMoveAnim;
    private float currentAnimValue = 0f;

    /////////////////////////////////////////////////////////////////////////////

    // Jump values
    private float jumpUpLength = 0f;
    private float jumpDownLength = 0f;

    /////////////////////////////////////////////////////////////////////////////

    // Wrappers conditions
    private bool isTurning = false;
    private bool isMoving = false;

    #endregion

    #region Unity Methods

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        // This will allow us the manually turn the NPC and manipulate the behaviors
        // of the OffMeshLinks without the NavMeshAgent interfering
        agent.updateRotation = false;
        agent.autoTraverseOffMeshLink = false;

        // Get the length of each jump animation
        foreach (AnimationClip anim in animator.runtimeAnimatorController.animationClips)
        {
            if (anim.name == "Jump Up")
            {
                jumpUpLength = anim.length;
            }
            else if (anim.name == "Jump Down")
            {
                jumpDownLength = anim.length;
            }

            if (jumpUpLength != 0f && jumpDownLength != 0f) break;
        }

        nextPoint = startingPoint;
    }

    private void LateUpdate()
    {
        // Skip the LateUpdate if the NPC is neither patrolling or moving
        if (!isPatrolling && !isMoving) return;

        // The NPC will turn towards the direction he's heading for
        if (agent.velocity.sqrMagnitude > Mathf.Epsilon && !isArrived && !isJumping)
        {
            Vector3 rot = agent.velocity.normalized;
            rot.y = 0f;

            StartCoroutine(FaceDirection(Quaternion.LookRotation(rot), turnDuration));
        }
        // Turns to the patrol point's orientation if required
        else if (isArrived && nextPoint.turnToPoint)
        {
            StartCoroutine(FaceDirection(nextPoint.transform.rotation, turnPointDuration));
        }

        // When the NPC is on an OffMeshLink and isn't currently jumping, jump
        if (agent.isOnOffMeshLink && !isJumping)
        {
            StartCoroutine(Jump(agent.currentOffMeshLinkData.linkType == OffMeshLinkType.LinkTypeDropDown));
        }
    }

    #endregion

    #region Patrol

    public void StartPatrol(MovementTypes type)
    {
        if (isPatrolling) return;

        isPatrolling = true;

        currentSpeed = type == MovementTypes.Walk ? walkSpeed : runSpeed;

        currentMoveAnim = (float)type;

        if (patrol != null) StopCoroutine(patrol);

        patrol = StartCoroutine(Patrol());
    }

    public void StopPatrol()
    {
        if (!isPatrolling) return;

        if (patrol != null) StopCoroutine(patrol);

        isPatrolling = false;
        isArrived = false;
        isJumping = false;

        StartCoroutine(LerpAnimation(moveAnimParam, currentAnimValue, idleAnim, 0.2f));

        agent.isStopped = true;
    }

    IEnumerator Patrol()
    {
        if (nextPoint == null) yield break;

        isPatrolling = true;

        agent.isStopped = false;

        while(isPatrolling)
        {
            isArrived = false;

            StartCoroutine(LerpAnimation(moveAnimParam, currentAnimValue, currentMoveAnim, 0.5f));

            agent.SetDestination(NextPosition);

            agent.speed = currentSpeed;

            while (agent.pathPending || agent.remainingDistance > agent.stoppingDistance)
            {
                yield return null;
            }

            isArrived = true;

            StartCoroutine(LerpAnimation(moveAnimParam, currentAnimValue, idleAnim, 0.5f));

            // Waits a certain delay with an offset before proceeding to the next patrol point
            yield return new WaitForSeconds(nextPoint.delay + 0.5f);

            // Stops patrolling when there's no next patrol point
            if(nextPoint.next == null)
            {
                isPatrolling = false;
            }
            else
            {
                nextPoint = nextPoint.next;
            }
        }
    }

    #endregion

    #region Animations

    /// <summary>
    /// Set the movement mode for the NPC. Movement modes are usually named over NPC states.
    /// This will decide which animation to use when idle or on alert.
    /// </summary>
    /// <param name="mode">0: Idle ; 1: Alert</param>
    public void SetMode(MoveMode mode)
    {
        StartCoroutine(LerpAnimation("Mode", animator.GetFloat("Mode"), (float)mode, 0.2f));
    }

    /// <summary>
    /// Set the NPC's movement, changing its speed and movement animation to lerp to.
    /// </summary>
    /// <param name="movement">0: Idle ; 1: Walk ; 2: Run</param>
    public void SetMovement(MovementTypes movement)
    {
        currentSpeed = movement == MovementTypes.Walk ? walkSpeed : runSpeed;

        currentMoveAnim = (float)movement;

        StartCoroutine(LerpAnimation("Move", animator.GetFloat("Move"), (float)movement, 0.3f));
    }

    

    /// <summary>
    /// Transition from one value to another in an animation parameter. Mostly used for blend trees.
    /// </summary>
    ///
    /// <remarks>
    /// This is the only method that will directly manipulate the NPC's animation.
    /// 
    /// Setting the duration to a too low value like 0.2f can cause the animation to not transition properly
    /// for certain models. Cause is unknown.
    /// </remarks>
    IEnumerator LerpAnimation(string param, float anim1, float anim2, float duration)
    {
        float elapsed = 0f;

        animator.SetFloat(param, anim1);

        while (elapsed < duration)
        {
            float value = Mathf.LerpUnclamped(anim1, anim2, elapsed / duration);

            animator.SetFloat(param, value);

            elapsed += Time.deltaTime;

            yield return null;
        }

        animator.SetFloat(param, anim2);

        currentAnimValue = anim2;
    }

    #endregion

    #region Movements and orientation

    /// <summary>
    /// Initialize the settings for the NavMeshAgent and lerps the animation to the currently assigned movement animation.
    /// </summary>
    /// 
    /// <remarks>This is usually called after SetMode and SetMovement from the Animations region</remarks>
    public void StartMove(float stoppingDistance)
    {
        agent.speed = currentSpeed;

        agent.stoppingDistance = stoppingDistance;

        agent.isStopped = false;

        StartCoroutine(LerpAnimation(moveAnimParam, currentAnimValue, currentMoveAnim, 0.2f));

        isMoving = true;
    }

    // Variables needed for moveUpdate.
    // This is the only time they're used, so it's pointless to bring them to the top.
    Vector3 oldDestination;
    bool hasStopped = false;

    /// <summary>
    /// Set a destination for the NPC to move to.
    /// </summary>
    /// <param name="destination"></param>
    public void MoveUpdate(Vector3 destination)
    {
        // To make sure the NPC has the right animation when stopped or on the move.
        // I don't like starting a coroutine at every frame, but his animations don't work properly
        // without doing that.
        if (hasStopped) StartCoroutine(LerpAnimation(moveAnimParam, currentAnimValue, idleAnim, 0.2f));
        else StartCoroutine(LerpAnimation(moveAnimParam, currentAnimValue, currentMoveAnim, 0.2f));

        if (!isMoving || (hasStopped && oldDestination == destination)) return;

        if(hasStopped && oldDestination != destination)
        {
            hasStopped = false;
        }

        oldDestination = destination;

        agent.SetDestination(destination);

        if (!hasStopped && agent.remainingDistance <= agent.stoppingDistance)
        {
            hasStopped = true;
        }

        
    }

    public void StopMove()
    {
        if (!isMoving) return;

        agent.isStopped = true;

        agent.stoppingDistance = 0f;

        StartCoroutine(LerpAnimation(moveAnimParam, currentAnimValue, idleAnim, 0.3f));

        isMoving = false;
    }

    /// <summary>
    /// Move wrapper
    /// </summary>
    public void Move_wrap(Vector3 destination, float stoppingDistance)
    {
        if (isMoving) return;

        StartCoroutine(Move(destination, stoppingDistance));
    }

    /// <summary>
    /// Move to a certain location
    /// </summary>
    IEnumerator Move(Vector3 destination, float stoppingDistance)
    {
        bool agentState = agent.isStopped;

        agent.isStopped = false;

        isMoving = true;

        StartCoroutine(LerpAnimation(moveAnimParam, currentAnimValue, currentMoveAnim, 0.2f));

        agent.SetDestination(destination);

        agent.speed = currentSpeed;

        agent.stoppingDistance = stoppingDistance;

        while (agent.pathPending || agent.remainingDistance > agent.stoppingDistance)
        {
            yield return null;
        }

        StartCoroutine(LerpAnimation(moveAnimParam, currentAnimValue, idleAnim, 0.2f));

        agent.stoppingDistance = 0f;
        agent.isStopped = agentState;

        isMoving = false;
    }

    /// <summary>
    /// Wrapper for the faceDirection coroutine.
    /// </summary>
    /// 
    /// <remarks>
    /// This is used for when the external script doesn't inherit monobehaviour, like the state classes, and thus can't
    /// start coroutines.
    /// </remarks>
    public void FaceDirection_wrap(Quaternion rotation, float duration)
    {
        if (isTurning) return;

        StartCoroutine(FaceDirection(rotation, duration));
    }

    /// <summary>
    /// Lerp to face a certain direction.
    /// </summary>
    IEnumerator FaceDirection(Quaternion rotation, float duration)
    {
        isTurning = true;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            transform.rotation = Quaternion.LerpUnclamped(transform.rotation, rotation, elapsed / duration);

            elapsed += Time.deltaTime;

            yield return null;
        }

        transform.rotation = rotation;

        isTurning = false;
    }

    /// <summary>
    /// Jumping. Used when traversing OffMeshLinks
    /// </summary>
    IEnumerator Jump(bool isDrop)
    {
        isJumping = true;

        // Get the starting OffMeshLink
        currentLink = agent.currentOffMeshLinkData;

        // Face the other end of the jump or drop
        transform.rotation = Quaternion.LookRotation(currentLink.endPos);

        // Lerps the animation to its idle animation before jumping
        StartCoroutine(LerpAnimation(moveAnimParam, currentAnimValue, idleAnim, 0.5f));
        
        yield return new WaitForSeconds(0.5f);

        // Begins the jump animation
        animator.SetBool(jumpAnimParam, true);

        // Makes the jump slightly before the end of the jump animation
        yield return new WaitForSeconds(jumpUpLength * 0.90f);

        // Get the duration based on the distance between the start and the end of the jump or drop
        float duration = Vector3.Distance(currentLink.startPos, currentLink.endPos);

        // Makes the duration faster if it's a drop
        duration /= isDrop ? 10f : 5f;

        // Sets the peak height based on whether it's a drop or jump
        float height = isDrop ? 2f : 0.5f;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            // Lerps the NPC's position between start and end
            Vector3 jumpPos = Vector3.Lerp(currentLink.startPos, currentLink.endPos, elapsed / duration);

            // Adds a higher value to Y, which will create an arc trajectory
            jumpPos.y += height * Mathf.Sin(Mathf.PI * elapsed / duration);

            // Finally, sets the new position to the transform
            transform.position = jumpPos;

            elapsed += Time.deltaTime;

            yield return null;
        }

        animator.SetBool(jumpAnimParam, false);

        // In case the lerp didn't go all the way, assign the end position to the transform
        transform.position = currentLink.endPos;

        yield return new WaitForSeconds(jumpDownLength * 0.90f);

        // Tell the agent the jump is complete
        agent.CompleteOffMeshLink();

        // Resumes the movement if he was patrolling or moving beforehand
        if (isPatrolling || isMoving)
        {
            agent.isStopped = false;
            StartCoroutine(LerpAnimation(moveAnimParam, idleAnim, currentMoveAnim, 0.5f));
        }

        isJumping = false;
    }

    #endregion
}

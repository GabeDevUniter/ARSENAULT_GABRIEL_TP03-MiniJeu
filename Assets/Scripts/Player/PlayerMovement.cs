using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Player movement from the Quake 1 engine.
/// Source code: https://github.com/id-Software/Quake/blob/master/WinQuake/sv_user.c This is only the tip of the iceberg
/// 
/// Help from a C# unity script adaptation of the Quake movements: https://github.com/WiggleWizard/quake3-movement-unity3d/blob/master/CPMPlayer.cs Obviously, didn't copy paste the entire script, that would be cheating.
/// </summary>
public class PlayerMovement : MonoBehaviour
{
    [Header("Speed")]
    public float moveSpeed = 7f;
    public float jumpSpeed = 8f;
    public float crouchMultiply = 2f;
    public float sprintMultiply = 1.2f;

    [Header("Mouse")]
    public float mouseX = 30f;
    public float mouseY = 30f;
    public bool invert = false;

    [Header("Acceleration")]
    public float maxSpeed = 320f;
    public float accelerateSpeed = 10f;
    public float airAcceleration = 2f;
    public float airDeacceleration = 2f;
    public float airControl = 0.3f;

    [Header("Physics Values")]
    public float stopSpeed = 100f;
    public float friction = 4f;
    public float edgeFriction = 2f;
    public float gravity = 800f;
    public float maxVelocity = 200f;

    private float setSpeed;
    private float currentSpeed;
    private float playerFriction;
    private float rotX = 0f;
    private float rotY = 0f;

    private float playerHeight;

    private bool jumping = false;
    private bool crouching = false;
    private bool sprinting = false;

    private Camera Cam;

    CharacterController CharacterController;

    Vector3 playerVelocity = Vector3.zero;

    public Vector3 HeadPosition => Cam.transform.position;

    void Awake()
    {
        CharacterController = GetComponent<CharacterController>();

        CharacterController.detectCollisions = false;

        Cam = GetComponentInChildren<Camera>();

        rotX = transform.rotation.eulerAngles.y;

        MoveCam();

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        playerHeight = CharacterController.height;
    }

    private void Update()
    {
        MoveCam();
    }

    private void FixedUpdate()
    {
        setSpeed = moveSpeed;

        if (!jumping) jumping = CharacterController.isGrounded && Input.GetButtonDown("Jump");

        Crouch();
        Sprint();

        if (crouching)
        {
            setSpeed /= crouchMultiply;
        }
        else if (sprinting)
        {
            setSpeed *= sprintMultiply;
        }

        Move();

        CharacterController.Move(playerVelocity * Time.fixedDeltaTime);
    }

    #region Movements

    void MoveCam()
    {
        rotX += Input.GetAxisRaw("Mouse X") * mouseX * 0.1f;
        rotY += Input.GetAxisRaw("Mouse Y") * mouseY * (invert ? 1 : -1) * 0.1f;

        if (rotY > 90) rotY = 90f;
        else if (rotY < -90) rotY = -90f;

        transform.rotation = Quaternion.Euler(0f, rotX, 0f);
        Cam.transform.rotation = Quaternion.Euler(rotY, rotX, 0f);
    }

    void Move()
    {
        Vector3 wishdir = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical"));
        wishdir = transform.TransformDirection(wishdir);
        wishdir.Normalize();

        float wishSpeed = wishdir.magnitude;

        if (wishSpeed > maxSpeed)
        {
            wishdir *= maxSpeed / wishSpeed;
        }

        wishSpeed *= setSpeed;

        if (CharacterController.isGrounded)
        {
            ApplyFriction(jumping ? 0f : 1f);
            Accelerate(wishdir, wishSpeed);

            playerVelocity.y = -gravity * Time.fixedDeltaTime;

            if (jumping)
            {
                playerVelocity.y = jumpSpeed;
                jumping = false;
            }
        }
        else
        {
            Accelerate(wishdir, wishSpeed);

            if (airControl > 0f) AirControl(wishdir, wishSpeed);

            playerVelocity.y -= gravity * Time.fixedDeltaTime;
        }

        if (playerVelocity.y < maxVelocity * -1) playerVelocity.y = maxVelocity * -1;
        else if (playerVelocity.y > maxVelocity) playerVelocity.y = maxVelocity;
    }

    void ApplyFriction(float trace)
    {
        Vector3 velocity = playerVelocity;
        float speed, newspeed, control = 0f;

        velocity.y = 0f;

        speed = velocity.magnitude;

        if (speed == 0f) return;

        if (trace == 1f)
        {
            playerFriction = friction * edgeFriction;
        }
        else
        {
            playerFriction = friction;
        }

        if (CharacterController.isGrounded) control = speed < stopSpeed ? stopSpeed : speed;


        newspeed = speed - (Time.fixedDeltaTime * control * playerFriction);

        if (newspeed < 0) newspeed = 0;

        newspeed /= speed;

        playerVelocity.x *= newspeed;
        playerVelocity.z *= newspeed;


    }

    void Accelerate(Vector3 wishdir, float wishSpeed)
    {
        float accelSpeed, addSpeed;

        if (!CharacterController.isGrounded)
        {
            wishSpeed = Vector3.Normalize(wishdir).magnitude;

            if (wishSpeed > 30) wishSpeed = 30;
        }

        currentSpeed = Vector3.Dot(playerVelocity, wishdir);

        addSpeed = wishSpeed - currentSpeed;

        if (addSpeed <= 0) return;

        float accel;

        if (CharacterController.isGrounded)
        {
            accel = accelerateSpeed;
        }
        else
        {
            accel = currentSpeed < 0 ? airAcceleration : airDeacceleration;
        }

        accelSpeed = accel * Time.fixedDeltaTime * wishSpeed;

        if (accelSpeed > addSpeed) accelSpeed = addSpeed;

        playerVelocity.x += accelSpeed * wishdir.x;
        playerVelocity.z += accelSpeed * wishdir.z;
    }

    void AirControl(Vector3 wishdir, float wishspeed)
    {
        float zspeed;
        float speed;
        float dot;
        float k;

        zspeed = playerVelocity.y;
        playerVelocity.y = 0;

        speed = playerVelocity.magnitude;
        playerVelocity.Normalize();

        dot = Vector3.Dot(playerVelocity, wishdir);
        k = 32;
        k *= airControl * dot * dot * Time.deltaTime;

        // Change direction while slowing down
        if (dot > 0)
        {
            playerVelocity.x = playerVelocity.x * speed + wishdir.x * k;
            playerVelocity.y = playerVelocity.y * speed + wishdir.y * k;
            playerVelocity.z = playerVelocity.z * speed + wishdir.z * k;

            playerVelocity.Normalize();
        }

        playerVelocity.x *= speed;
        playerVelocity.y = zspeed; // Note this line
        playerVelocity.z *= speed;
    }

    #endregion

    #region Additional Features

    public void Stand()
    {
        CharacterController.height = playerHeight;
        crouching = false;
    }

    void Crouch()
    {
        if (Input.GetButtonDown("Crouch"))
        {
            CharacterController.height = playerHeight / 2;
            crouching = true;
        }
        else if (Input.GetButtonUp("Crouch"))
        {
            Stand();
        }
    }

    void Sprint()
    {
        if (Input.GetButtonDown("Sprint"))
        {
            sprinting = true;
        }
        else if (Input.GetButtonUp("Sprint"))
        {
            sprinting = false;
        }
    }

    #endregion

    #region Debugging

#if false
    GUIStyle style = new GUIStyle();
    private void OnGUI()
    {
        style.fontSize = 28;
        style.normal.textColor = new Color(230, 230, 230);

        GUI.Label(
            new Rect(5, 5, 300, 150),
            $"CharacterController Velocity: {CharacterController.velocity}\n" +
            $"Speed: {CharacterController.velocity.magnitude}\n" +
            $"Player Friction: {playerFriction}",
            style
            );
    }
#endif

    #endregion
}

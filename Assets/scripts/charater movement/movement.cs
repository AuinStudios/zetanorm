// Created by Vladis.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///     What does this movement do?
/// </summary>
public sealed class movement : MonoBehaviour
{
    //Scriptable object which holds all the player's movement parameters. If you don't want to use it
    //just paste in all the parameters, though you will need to manuly change all references in this script
    [SerializeField]
    private PlayerRunData Data;

    #region COMPONENTS
    [SerializeField]
    private Rigidbody2D RB;
    //Script to handle all player animations, all references can be safely removed if you're importing into your own project.
    #endregion

    #region STATE PARAMETERS
    //Variables control the various actions the player can perform at any time.
    //These are fields which can are public allowing for other sctipts to read them
    //but can only be privately written to.
    public bool IsFacingRight { get; private set; }
    public float LastOnGroundTime { get; private set; }
    [SerializeField] private float JumpForce = 0.05f;
    [SerializeField] private float DashForce = 100.0f;
    [SerializeField] private int MaxDashTimes = 2;
    private int DashTimes = 0;
    private float HowLongJump = 0.0f;
    #endregion

    #region INPUT PARAMETERS
   [SerializeField] private Vector2 _moveInput;
    #endregion

    #region CHECK PARAMETERS
    //Set all of these up in the inspector
    [Header("Checks")]
    [SerializeField] private Transform _groundCheckPoint;
    [SerializeField] private Vector2 _groundCheckSize = new Vector2(0.49f, 0.03f);



    [Header("Camera")]
    [SerializeField]
    private Transform MainCam;
    [SerializeField]
    [Range(0, 1)]
    private float SmoothTime = 0.125f;
    private Vector3 smoothdampvelocity = Vector3.zero;
    private Vector3 desiredposition;
    [SerializeField]
    private Vector3 cameraoffset;
    #endregion

    #region LAYERS & TAGS
    [Header("Layers & Tags")]
    [SerializeField] private LayerMask _groundLayer;
    #endregion

    private void Start()
    {
        IsFacingRight = true;
    }

    private void Update()
    {
        if (RB.velocity.y < 0)
        {
            RB.velocity += Vector2.up * Physics2D.gravity.y * (2.0f - 1.0f) * Time.deltaTime;
        }
        #region TIMERS
        LastOnGroundTime -= Time.deltaTime;
        #endregion

        #region INPUT HANDLER
        _moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));


        if (_moveInput.x != 0)
            CheckDirectionToFace(_moveInput.x > 0);
        #endregion
        if (DashTimes < MaxDashTimes && _moveInput != new Vector2(0, 1) && Input.GetKeyDown(KeyCode.LeftShift))
        {
            Dash();
        }

        #region COLLISION CHECKS
        //Ground Check
        if (Physics2D.OverlapBox(_groundCheckPoint.position, _groundCheckSize, 0, _groundLayer))
        {

            DashTimes = 0;
            HowLongJump = 0;
            LastOnGroundTime = 0.1f;
        }

        #endregion

    }

    private void FixedUpdate()
    {

        Run();

        CameraMovement();

        if (HowLongJump < 0.15f && Input.GetKey(KeyCode.Space))
        {
            Jump();
        }
        else if (Input.GetKeyUp(KeyCode.Space))
        {
            HowLongJump = 1;
        }
    }

    //MOVEMENT METHODS
    private void CameraMovement()
    {
        desiredposition = transform.position + cameraoffset;
        MainCam.position = Vector3.SmoothDamp(MainCam.position, desiredposition, ref smoothdampvelocity, SmoothTime);
    }
    private void Dash()
    {
        if (RB.velocity.y >0 || RB.velocity.y < 0)
        {
            DashTimes++;

            Vector3.Normalize(_moveInput);
            RB.AddForce(new Vector2(_moveInput.x * DashForce, _moveInput.y), ForceMode2D.Impulse);
            HowLongJump = 1;
        }


    }
    private void Jump()
    {
        float force = JumpForce;
        force += Time.deltaTime * 2.5f;
        HowLongJump += Time.deltaTime;
        RB.AddForce(Vector2.up * force, ForceMode2D.Impulse);
    }
    #region RUN METHODS
    private void Run()
    {
        // Calculate the Jump

        // Callculate the camera smooth direction


        //Calculate the direction we want to move in and our desired velocity
        float targetSpeed = _moveInput.x * Data.runMaxSpeed;

        #region Calculate AccelRate
        float accelRate;

        //Gets an acceleration value based on if we are accelerating (includes turning) 
        //or trying to decelerate (stop). As well as applying a multiplier if we're air borne.
        if (LastOnGroundTime > 0)
        {
            accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? Data.runAccelAmount : Data.runDeccelAmount;
        }
        else
        {
            accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? Data.runAccelAmount * Data.accelInAir : Data.runDeccelAmount * Data.deccelInAir;
        }

        #endregion




        #region Conserve Momentum
        //We won't slow the player down if they are moving in their desired direction but at a greater speed than their maxSpeed
        if (Data.doConserveMomentum && Mathf.Abs(RB.velocity.x) > Mathf.Abs(targetSpeed) && Mathf.Sign(RB.velocity.x) == Mathf.Sign(targetSpeed) && Mathf.Abs(targetSpeed) > 0.01f && LastOnGroundTime < 0)
        {
            //Prevent any deceleration from happening, or in other words conserve are current momentum
            //You could experiment with allowing for the player to slightly increae their speed whilst in this "state"
            accelRate = 0;
        }
        #endregion

        //Calculate difference between current velocity and desired velocity
        float speedDif = targetSpeed - RB.velocity.x;
        //Calculate force along x-axis to apply to thr player

        float movement = speedDif * accelRate;

        //Convert this to a vector and apply to rigidbody
        RB.AddForce(movement * Vector2.right, ForceMode2D.Force);

        /*
		 * For those interested here is what AddForce() will do
		 * RB.velocity = new Vector2(RB.velocity.x + (Time.fixedDeltaTime  * speedDif * accelRate) / RB.mass, RB.velocity.y);
		 * Time.fixedDeltaTime is by default in Unity 0.02 seconds equal to 50 FixedUpdate() calls per second
		*/
    }

    private void Turn()
    {
        //stores scale and flips the player along the x axis, 
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;

        IsFacingRight = !IsFacingRight;
    }
    #endregion


    #region CHECK METHODS
    public void CheckDirectionToFace(bool isMovingRight)
    {
        if (isMovingRight != IsFacingRight)
            Turn();
    }
    #endregion

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(_groundCheckPoint.position, _groundCheckSize);
    }
}

// Created by Vladis.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///     What does this movement do?
/// </summary>
public sealed class movement : MonoBehaviour
{
    // #region Singleton
    // public static movement Instance { get; private set; }
    // private void Awake()
    // {
    //     // If there is an instance, and it's not me, delete myself.
    //     if (Instance != null && Instance != this)
    //     {
    //         Destroy(this);
    //     }
    //     else
    //     {
    //         Instance = this;
    //     }
    // }
    // #endregion
    //Scriptable object which holds all the player's movement parameters.
    [Header("data for player movement")]
    [SerializeField]
    private PlayerRunData Data;

    #region COMPONENTS
    [Header("physics component")]
    [SerializeField]
    private Rigidbody2D RB;
    //Script to handle all player animations, all references can be safely removed if you're importing into your own project.
    #endregion

    #region STATE PARAMETERS
    //Variables control the various actions the player can perform at any time.
    //These are fields which can are public allowing for other sctipts to read them
    //but can only be privately written to.
    private bool isonwall = false;
    public bool IsFacingRight { get; private set; }
    public float LastOnGroundTime { get; private set; }

    private int DashTimes = 0;
    private float HowLongJump = 0.0f;
    private float StartingGravity;
    #endregion

    #region INPUT PARAMETERS
    private Vector2 _moveInput;
    #endregion

    #region CHECK PARAMETERS
    //Set all of these up in the inspector

    [Header("Jump Propertys")]
    [SerializeField] private float JumpForce = 0.3f;

    [Header("dash propertys")]
    [SerializeField] private float DashForce = 1.3f;
    [SerializeField] private int MaxDashTimes = 2;
    [SerializeField] private SpriteRenderer afterimage;
    [Header("GroundCheck")]
    [SerializeField] private Transform _groundCheckPoint;
    [SerializeField] private Vector2 _groundCheckSize = new Vector2(0.23f, 0.03f);

    [Header("WallCheck")]
    [SerializeField] private Transform FrontCheck;
    [SerializeField] private Transform BackCheck;
    [SerializeField] private Vector2 Wallchecksize = new Vector2(0.11f, 0.03f);

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

    [Header("wall jump direction")]
    [SerializeField] private Transform GFXdirection;
    [Header("pickup coin")]
    [SerializeField] private Gun coinpickup;
    #endregion

    #region LAYERS & TAGS
    [Header("Layers & Tags")]
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private LayerMask _WallLayer;
    #endregion

    private void Start()
    {
        IsFacingRight = true;
        StartingGravity = RB.gravityScale;
    }

    private void Update()
    {
        // adds extra gravity
       
        #region TIMERS
        LastOnGroundTime -= Time.deltaTime;
        #endregion
        // gets movement input
        #region INPUT HANDLER
        _moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        // check direction
        if (_moveInput.x != 0 && !isonwall)
            CheckDirectionToFace(_moveInput.x > 0);
        #endregion
        // dash
        if (DashTimes < MaxDashTimes && _moveInput != new Vector2(0, 1) && _moveInput != new Vector2(0, 0) && Input.GetKeyDown(KeyCode.LeftShift))
        {
            StartCoroutine(Dash());

        }
        #region COLLISION CHECKS
        //Ground Check
        if (Physics2D.OverlapBox(_groundCheckPoint.position, _groundCheckSize, 0, _groundLayer))
        {
            RB.velocity = Vector2.zero;
            isonwall = false;
            DashTimes = 0;
            HowLongJump = 0;
            LastOnGroundTime = 0.1f;
        }
        else
        {
            RB.velocity += Vector2.up * Physics2D.gravity.y * (JumpForce - 1.0f) * Time.deltaTime;
        }
        // check for wall to get on
        if (Physics2D.OverlapBox(FrontCheck.position, Wallchecksize, 0, _WallLayer) && Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(WallMode());
        }
        // check if ur on the wall
        if (!Physics2D.OverlapBox(BackCheck.position, Wallchecksize, 0, _WallLayer) && isonwall == true)
        {
            isonwall = false;
        }
        #endregion

    }
    private void FixedUpdate()
    { 
      
           
       
        if (!isonwall)
        {
            Run();
        }
        CameraMovement();
        // jump
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
    private IEnumerator WallMode()
    {

        if (!isonwall && LastOnGroundTime != 0.1f)
        {
            isonwall = true;
            RB.isKinematic = true;
            RB.velocity = new Vector2(0, 0);
            bool doonce = false;
            float timer = 0.0f;
            Turn();

            yield return new WaitForSeconds(0.1f);
            while (!Input.GetKeyDown(KeyCode.Space))
            {

                timer += Time.deltaTime;
                if (timer > 0.5f && !doonce)
                {
                    doonce = true;
                    RB.isKinematic = false;
                    RB.gravityScale = RB.gravityScale / 10;
                }
                if (!isonwall)
                {
                    break;
                }
                yield return null;
            }

            RB.isKinematic = false;
            RB.gravityScale = StartingGravity;

            if (isonwall)
            {

                RB.AddForce(GFXdirection.localScale * DashForce * 1.8f, ForceMode2D.Impulse);
            }
            isonwall = false;
        }



    }
    private void CameraMovement()
    {
        desiredposition = transform.position + cameraoffset;
        MainCam.position = Vector3.SmoothDamp(MainCam.position, desiredposition, ref smoothdampvelocity, SmoothTime);
    }
    private IEnumerator Dash()
    {

        if (RB.velocity.y > 0 || RB.velocity.y < 0)
        {


            DashTimes++;
            Vector3.Normalize(_moveInput);
            RB.AddForce(new Vector2(_moveInput.x * DashForce, _moveInput.y), ForceMode2D.Impulse);
            HowLongJump = 1;
            if (DashTimes == 1)
            {
                int time = 0;
                int pickbetweencolor = 0;
                while (time < DashTimes + 5)
                {
                    time++;
                    SpriteRenderer spawnafterimage = Instantiate(afterimage, transform.position, Quaternion.identity);
                    pickbetweencolor = Random.Range(0, 2);
                    spawnafterimage.color = pickbetweencolor > 0 ? Color.green : Color.blue;
                    spawnafterimage.transform.localScale = GFXdirection.localScale;
                    Destroy(spawnafterimage.gameObject, 1);
                    yield return new WaitForSeconds(0.25f);

                    yield return new WaitForFixedUpdate();
                }
            }

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
        RB.velocity = (movement * Vector2.right);

        /*
		 * For those interested here is what AddForce() will do
		 * RB.velocity = new Vector2(RB.velocity.x + (Time.fixedDeltaTime  * speedDif * accelRate) / RB.mass, RB.velocity.y);
		 * Time.fixedDeltaTime is by default in Unity 0.02 seconds equal to 50 FixedUpdate() calls per second
		*/
    }

    private void Turn()
    {
        //stores scale and flips the player along the x axis, 
        Vector3 scale = GFXdirection.localScale;
        scale.x *= -1;
        GFXdirection.localScale = scale;

        IsFacingRight = !IsFacingRight;
    }
    #endregion


    #region CHECK METHODS
    // checks direction 
    public void CheckDirectionToFace(bool isMovingRight)
    {
        if (isMovingRight != IsFacingRight)
            Turn();
    }
    #endregion

    // PickUpCoins
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.attachedRigidbody == false && collision.CompareTag("coin") && coinpickup.coinamount < coinpickup.gunpropertys.coinsallowed)
        {
            coinpickup.coinamount += 1;
            Destroy(collision.gameObject);
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(_groundCheckPoint.position, _groundCheckSize);
        Gizmos.DrawWireCube(FrontCheck.position, Wallchecksize);
    }
}
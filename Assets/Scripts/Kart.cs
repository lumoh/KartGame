﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Go-Kart Style Car
/// </summary>
public class Kart : MonoBehaviour 
{
    [Header("Kart Attributes")]
    public float Weight;
    public float TopSpeed;
    public float AccelerationTime;
    public float DecelerationTime;
    public float ReverseSpeed;
    public float TurnSpeed;
    public float GroundedDrag;
    public float GroundedAngularDrag;
    public float AirDrag;
    public float AirAngularDrag;
    public float GravityForce;
    public float HeightOffGround;
    public float DeadZone;
    public float SlideZone;
    public float SlidingTurnSpeed;
    public float SlidingBoost;

    // Physics
    private BoxCollider col;
    private Rigidbody body;

    private float thrust;
    private float thrustRatio;
    private bool grounded;
    private bool isMoving;   
    private bool isMovingForward;
    private bool isSliding;
    private bool slidingRight;
    private bool slidingLeft;

    private float vInput;
    private float hInput;
    private float jumpInput;

    private int layerMask;

    void Start()
    {
        col = GetComponent<BoxCollider>();
        body = GetComponent<Rigidbody>();

        layerMask = 1 << LayerMask.NameToLayer("Kart");
        layerMask = ~layerMask;
    }

    void Update()
    {        
        input();
    }

    void FixedUpdate()
    {
        setDirection();
        gravity();
        movement();
        turning();
    }

    /// <summary>
    /// Handle input for the kart
    /// </summary>
    void input()
    {
        vInput = Input.GetAxis("Vertical");
        hInput = Input.GetAxis("Horizontal");
        jumpInput = Input.GetAxis("Jump");

        if (vInput > 0)
        {
            vInput = 1;
        }
        else if (vInput < 0)
        {
            vInput = -1;
        }

        // if drive or reverse use acceleration time
        if (vInput == 1 || vInput == -1)
        {
            thrustRatio += Time.deltaTime / AccelerationTime * vInput;
            thrustRatio = Mathf.Clamp(thrustRatio, -1, 1);
        }
        else
        {
            // coast to stop
            if (thrustRatio > 0)
            {
                thrustRatio -= Time.deltaTime / DecelerationTime;
                if (thrustRatio < 0)
                {
                    thrustRatio = 0;
                }
            }
            else
            {
                thrustRatio += Time.deltaTime / DecelerationTime;
                if (thrustRatio > 0)
                {
                    thrustRatio = 0;
                }
            }
        }            

        if (isMovingForward)
        {
            thrust = thrustRatio * TopSpeed;
        }
        else
        {
            thrust = thrustRatio * ReverseSpeed;
        }

        if (isSliding)
        {
            float turnRange = 1.0f - SlideZone;  
            float halfTurn = turnRange / 2.0f;
            float midPoint = SlideZone + halfTurn;

            if (slidingRight)
            {                
                hInput =  midPoint + (halfTurn * hInput);
            }
            else if( slidingLeft)
            {
                hInput = -midPoint + (halfTurn * hInput);
            }
        }
    }

    /// <summary>
    /// set direction status vars
    /// </summary>
    void setDirection()
    {
        isMoving = body.velocity.magnitude > DeadZone;
        isMovingForward = Vector3.Dot(transform.forward, body.velocity) >= 0;

        if(jumpInput == 0)
        {
            isSliding = false;
        }

        if (!isSliding)
        {
            slidingRight = jumpInput > 0 && hInput > SlideZone;
            slidingLeft = jumpInput > 0 && hInput < -SlideZone;
            isSliding = slidingRight || slidingLeft;
        }

//        if (isSliding)
//        {
//            Debug.Log("Sliding!");
//        }
        //Debug.Log("Speed = " + body.velocity.magnitude.ToString());
    }

    /// <summary>
    /// handle kart gravity and being grounded to the surface it is driving on
    /// </summary>
    void gravity()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, -Vector3.up, out hit, HeightOffGround + 0.1f, layerMask))
        {
            grounded = true;

            Quaternion rot = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
            transform.rotation = Quaternion.Lerp(transform.rotation, rot, 1);

            // lock Y pos to floor
            Vector3 pos = transform.position;
            float newY = (hit.point + (hit.normal * HeightOffGround)).y;
            pos.y = newY;
            transform.position = pos;

            body.drag = GroundedDrag;
            body.angularDrag = GroundedAngularDrag;
        }
        else
        {
            grounded = false;
            body.AddForceAtPosition(transform.up * -GravityForce, transform.position);

            body.drag = AirDrag;
            body.angularDrag = AirAngularDrag;
        }
    }

    /// <summary>
    /// handle turning
    /// </summary>
    void turning()
    {
        if (hInput != 0)
        {
            float turnSpeed = TurnSpeed;
            if (isSliding)
            {
                turnSpeed = SlidingTurnSpeed;
            }
            body.AddRelativeTorque(Vector3.up * hInput * turnSpeed * thrustRatio);
        }
    }

    /// <summary>
    /// handle movement and thrust
    /// </summary>
    void movement()
    {
        if (thrust != 0)
        {
            Vector3 vel = transform.forward * thrust;
            if (isSliding)
            {
                vel *= SlidingBoost;
            }                
            body.velocity = vel;
        }
    }
}

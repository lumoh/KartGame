using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kart : MonoBehaviour 
{
    public float Speed;
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

    private BoxCollider col;
    private Rigidbody body;
    private float thrust;
    private float turnValue;
    private bool grounded;
    private bool isMoving;   
    private bool isMovingForward;
    private float vInput;
    private float hInput;

    private int layerMask;

    private float thrustRatio = 0f;

    void Start()
    {
        col = GetComponent<BoxCollider>();
        body = GetComponent<Rigidbody>();

        layerMask = 1 << LayerMask.NameToLayer("Kart");
        layerMask = ~layerMask;
    }

    void calculateThrust()
    {
        vInput = Input.GetAxis("Vertical");
        // input is either 1, -1, or 0
        // (drive, reverse, neutral)
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
            thrust = thrustRatio * Speed;
        }
        else
        {
            thrust = thrustRatio * ReverseSpeed;
        }

        //Debug.Log(thrustRatio.ToString());
        turnValue = Input.GetAxis("Horizontal");

//        TODO JUMP
//        if (Input.GetKeyDown(KeyCode.Space))
//        {
//            body.AddForce(transform.up * 150);
//        }
    }

    void Update()
    {
        setDirection();
        calculateThrust();
    }

    void setDirection()
    {
        // if moving slow enough just stop
//        if (body.velocity.magnitude < DeadZone)
//        {
//            body.velocity = Vector3.zero;
//        }

        isMoving = body.velocity.magnitude > DeadZone;
        isMovingForward = Vector3.Dot(transform.forward, body.velocity) >= 0;

        //Debug.Log(body.velocity.magnitude.ToString());
        //Debug.Log(isMovingForward.ToString());
    }

    void FixedUpdate()
    {
        RaycastHit hit;
        grounded = false;
        if (Physics.Raycast(transform.position, -Vector3.up, out hit, HeightOffGround, layerMask))
        {
            grounded = true;

            Quaternion rot = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
            transform.rotation = Quaternion.Lerp(transform.rotation, rot, 1);

            // lock Y pos to floor
            Vector3 pos = transform.position;
            float newY = (hit.point + (hit.normal * HeightOffGround)).y;
            pos.y = newY;
            transform.position = pos;
        }
        else
        {
            body.AddForceAtPosition(transform.up * -GravityForce, transform.position);
        }
            
        if (grounded)
        {
            body.drag = GroundedDrag;
            body.angularDrag = GroundedAngularDrag;
        }
        else
        {
            thrust = 0;
            body.drag = AirDrag;
            body.angularDrag = AirAngularDrag;
        }

        if (thrust != 0)
        {
            // FORCE
            //body.AddForce(transform.forward * thrust);

            // VELOCITY
            body.velocity = transform.forward * thrust;
        }

        if (turnValue != 0)
        {
            body.AddRelativeTorque(Vector3.up * turnValue * TurnSpeed * thrustRatio);
        }
    }
}

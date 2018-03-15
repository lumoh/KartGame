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

    void input()
    {
        vInput = Input.GetAxis("Vertical");
        hInput = Input.GetAxis("Horizontal");

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
    }

    void setDirection()
    {
        isMoving = body.velocity.magnitude > DeadZone;
        isMovingForward = Vector3.Dot(transform.forward, body.velocity) >= 0;

        Debug.Log("Speed = " + body.velocity.magnitude.ToString());
    }

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

    void turning()
    {
        if (hInput != 0)
        {
            body.AddRelativeTorque(Vector3.up * hInput * TurnSpeed * thrustRatio);
        }
    }

    void movement()
    {
        if (thrust != 0)
        {
            // FORCE
            //body.AddForce(transform.forward * thrust);

            // VELOCITY
            body.velocity = transform.forward * thrust;
        }
    }
}

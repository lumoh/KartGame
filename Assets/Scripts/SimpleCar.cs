using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleCar : MonoBehaviour 
{
    public float Mass = 10;
    public float Acceleration = 1;
    public float Deceleration = 1;
    public float TurnSpeed = 30;
    public float DeadZone = 1;
    public float Gravity = 10;
    public float HeightOffGround;

    private Vector3 velocity;
    private float vInput;
    private float hInput;
    private bool isMoving;
    private bool isMovingForward;
    private bool grounded;

    private int layerMask;

    void Start()
    {
        layerMask = 1 << LayerMask.NameToLayer("Kart");
        layerMask = ~layerMask;
    }
	
    void thrust()
    {
        if (grounded)
        {
            vInput = Input.GetAxis("Vertical");
            if (vInput > 0)
            {
                vInput = 1;
            }
            else if (vInput < 0)
            {
                vInput = -1;
            }
            velocity += transform.forward * vInput * Acceleration * Time.deltaTime;
        }
    }

    void drag()
    {
        if (vInput == 0)
        {
            velocity += velocity.normalized * -1 * Deceleration * Time.deltaTime;
            if (velocity.magnitude < DeadZone)
            {
                velocity = Vector3.zero;
            }
        }
    }

    void gravity()
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

        if (!grounded)
        {
            velocity += Vector3.down * Time.deltaTime * Gravity;
        }
        else
        {
            velocity.y = 0;
        }
    }

    void setDirection()
    {
        isMoving = velocity.magnitude > 0;
        isMovingForward = Vector3.Dot(transform.forward, velocity) >= 0;
    }

    void turn()
    {
        hInput = Input.GetAxis("Horizontal");
        transform.Rotate(0, TurnSpeed * Time.deltaTime * hInput, 0);
    }

	// Update is called once per frame
	void Update () 
    {
        gravity();
        drag();
        setDirection();
        thrust();
        turn();

        // move with velocity
        transform.position += velocity;
	}
}

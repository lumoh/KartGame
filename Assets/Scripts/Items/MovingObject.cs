using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Basic Projectile that moves without thrust
/// </summary>
public class MovingObject : MonoBehaviour 
{
    /// <summary>
    /// Height off the ground this object should hover
    /// </summary>
    public float HeightOffGround;
    /// <summary>
    /// drag when on the ground
    /// </summary>
    public float GroundedDrag;
    /// <summary>
    /// angular drag when on the ground
    /// </summary>
    public float GroundedAngularDrag;
    /// <summary>
    /// drag when in the air
    /// </summary>
    public float AirDrag;
    /// <summary>
    /// angular drag when in the air
    /// </summary>
    public float AirAngularDrag;
    /// <summary>
    /// The gravity force, applied when in the air
    /// </summary>
    public float GravityForce;

    /// <summary>
    /// The grounded.
    /// </summary>
    protected bool isGrounded;
    /// <summary>
    /// The rigid body.
    /// </summary>
    protected Rigidbody body;
    /// <summary>
    /// the collider
    /// </summary>
    protected Collider col;
    /// <summary>
    /// layer mask
    /// </summary>
    protected int layerMask;
        
    void Start()
    {
        col = GetComponent<BoxCollider>();
        body = GetComponent<Rigidbody>();

        layerMask = 1 << LayerMask.NameToLayer("Items");
        layerMask = ~layerMask;
    }

    public virtual void FixedUpdate()
    {
        if (body != null && !body.isKinematic)
        {
            gravity();
        }
    }

    /// <summary>
    /// handle kart gravity and being grounded to the surface it is driving on
    /// </summary>
    void gravity()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, -Vector3.up, out hit, HeightOffGround + 0.1f, layerMask))
        {
            isGrounded = true;

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
            isGrounded = false;
            body.AddForceAtPosition(transform.up * -GravityForce, transform.position);
        }
    }
}

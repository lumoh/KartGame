using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Projectile that just shoots straight and bounces off walls
/// </summary>
public class Projectile : Item 
{
    /// <summary>
    /// The speed.
    /// </summary>
    public float Speed;

    private Rigidbody rb;
    private Collider col;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public override void Activate()
    {
        
    }

    public override void Fire()
    {
        transform.position = Owner.transform.position + (Owner.transform.forward * 2);
        transform.SetParent(null);
        transform.rotation = Owner.transform.rotation;

        rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.velocity = Owner.transform.forward * Speed;
        }

        col = GetComponent<Collider>();
        if (col != null)
        {
            col.isTrigger = false;
        }
    }
}

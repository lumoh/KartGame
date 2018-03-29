using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

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
        shoot();
    }
        
    public override void Fire()
    {
        shoot();   
    }

    private void shoot()
    {
        transform.position = Owner.transform.position + (Owner.transform.forward * 2);
        transform.SetParent(null);

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

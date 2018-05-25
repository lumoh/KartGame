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

    public override void OnStartClient()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
    }

    public override void Activate()
    {
        if (Owner != null)
        {            
            transform.localPosition = new Vector3(0, 0, -1.5f);
        }
    }
        
    public override void Fire()
    {
        shoot();
    }

    private void shoot()
    {
        transform.SetParent(null);
        transform.position = parent.position + (parent.forward * 2);
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.velocity = parent.forward * Speed;
        }

        if (col != null)
        {
            col.isTrigger = false;
        }

        if (Owner != null)
        {
            Owner.CurrentItem = null;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : Item 
{
    public float Speed;

    void Start()
    {
        
    }

    public override void Activate()
    {
        
    }

    public override void Fire()
    {
        transform.position = Owner.transform.position + (Owner.transform.forward * 2);
        transform.SetParent(null);
        Rigidbody rigid = GetComponent<Rigidbody>();
        if (rigid != null)
        {
            rigid.isKinematic = false;
            rigid.velocity = Owner.transform.forward * Speed;
        }
    }
}

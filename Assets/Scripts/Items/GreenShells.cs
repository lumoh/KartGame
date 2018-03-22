using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreenShells : Item 
{
    public float RotateSpeed;
    public List<GameObject> Shells;

    public override void Activate()
    {
        
    }        

    public override void Fire()
    {
        base.Fire();
    }

    public override void Update()
    {
        transform.Rotate(Vector3.up, RotateSpeed * Time.deltaTime);
    }
}

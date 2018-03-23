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
        if (Shells != null && Shells.Count > 0)
        {
            int index = Shells.Count - 1;
            GameObject shell = Shells[index];
            Shells.RemoveAt(index);
            if (shell != null)
            {
                
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public override void Update()
    {
        transform.Rotate(Vector3.up, RotateSpeed * Time.deltaTime);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Generic class for item
/// </summary>
public class Item : MonoBehaviour 
{
    /// <summary>
    /// the owner of the item
    /// </summary>
    [HideInInspector] public Kart Owner;

    /// <summary>
    /// Activate this instance.
    /// </summary>
    public virtual void Activate()
    {
        
    }

    /// <summary>
    /// Fire this instance.
    /// </summary>
    public virtual void Fire()
    {
        
    }

    /// <summary>
    /// Discard this instance.
    /// </summary>
    public virtual void Discard()
    {
        if (Owner != null)
        {
            Owner.CurrentItem = null;
        }
        Destroy(gameObject);
    }

    /// <summary>
    /// Update this instance.
    /// </summary>
    public virtual void Update()
    {
        
    }
}

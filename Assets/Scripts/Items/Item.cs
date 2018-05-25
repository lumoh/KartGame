using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Generic class for item
/// </summary>
public class Item : NetworkBehaviour 
{
    /// <summary>
    /// the owner of the item
    /// </summary>
    [HideInInspector] public Kart Owner;
    protected Transform parent;
    protected NetworkIdentity ni;

    public override void OnStartAuthority()
    {
        parent = ClientScene.readyConnection.playerControllers[0].gameObject.transform;
        ni = parent.GetComponent<NetworkIdentity>();
        Owner = parent.GetComponent<Kart>();
        if (Owner != null)
        {
            transform.SetParent(Owner.ItemOrigin);
            Owner.CurrentItem = this;
        }
        Activate();
    }

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

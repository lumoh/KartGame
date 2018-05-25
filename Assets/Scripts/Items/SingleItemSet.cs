using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Single item set.
/// </summary>
public class SingleItemSet : Item 
{
    /// <summary>
    /// the type of item in the set
    /// </summary>
    public GameObject ItemPrefab;

    /// <summary>
    /// list of the items
    /// </summary>
    [HideInInspector] public Item Item;

    /// <summary>
    /// create all the items and circle them around the Owner
    /// </summary>
    public override void Activate()
    {
        Cmd_Activate();
    }

    [Command]
    public void Cmd_Activate()
    {
        GameObject itemObj = Instantiate(ItemPrefab, transform) as GameObject;
        if (itemObj != null)
        {
            NetworkServer.SpawnWithClientAuthority(itemObj, base.connectionToClient);
        }
    }

    /// <summary>
    /// fire one of the items and remove it from list
    /// </summary>
    public override void Fire()
    {
        if (Item != null)
        {
            Item.Fire();
            Owner.CurrentItem = null;
        }
    }
}

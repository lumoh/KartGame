using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SingleItemSet : Item 
{
    /// <summary>
    /// the type of item in the set
    /// </summary>
    public GameObject ItemPrefab;

    /// <summary>
    /// list of the items
    /// </summary>
    [HideInInspector] public Item Items;

    /// <summary>
    /// create all the items and circle them around the Owner
    /// </summary>
    public override void Activate()
    {

    }

    /// <summary>
    /// fire one of the items and remove it from list
    /// </summary>
    [Command(channel = Channels.DefaultUnreliable)]
    public override void CmdFire()
    {
        GameObject itemObj = Instantiate(ItemPrefab) as GameObject;
        if (itemObj != null)
        {
            itemObj.transform.SetParent(transform);
            itemObj.transform.position = Owner.transform.position + (Owner.transform.forward * 2f);

            Item item = itemObj.GetComponent<Item>();
            if (item != null)
            {
                item.Owner = Owner;
                item.CmdFire();
            }
            NetworkServer.Spawn(itemObj);
        }
    }
}

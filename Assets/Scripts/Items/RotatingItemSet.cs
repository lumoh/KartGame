using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Rotating item set is a set of items that rotate around the kart before fired
/// </summary>
public class RotatingItemSet : Item 
{
    /// <summary>
    /// the type of item in the set
    /// </summary>
    public GameObject ItemPrefab;
    /// <summary>
    /// speed at which items rotate around the owner
    /// </summary>
    public float RotateSpeed;
    /// <summary>
    /// The number items in the set
    /// </summary>
    public int NumItems;
    /// <summary>
    /// radius around the owner in which the items rotate
    /// </summary>
    public float Radius;
    /// <summary>
    /// list of the items
    /// </summary>
    [HideInInspector] public List<Item> Items;

    /// <summary>
    /// create all the items and circle them around the Owner
    /// </summary>
    public override void Activate()
    {
        for (int i = 0; i < NumItems; i++)
        {
            GameObject itemObj = Instantiate(ItemPrefab) as GameObject;
            if (itemObj != null)
            {
                NetworkServer.Spawn(itemObj);
                itemObj.transform.SetParent(transform);
                itemObj.transform.localPosition = Vector3.zero;
                float angle = (360 / NumItems) * i;
                itemObj.transform.rotation = Quaternion.Euler(new Vector3(0, angle, 0));
                itemObj.transform.position += itemObj.transform.forward * Radius;

                Item item = itemObj.GetComponent<Item>();
                if (item != null)
                {
                    item.Owner = Owner;
                    Items.Add(item);
                }
            }
        }
    }

    /// <summary>
    /// fire one of the items and remove it from list
    /// </summary>
    public override void Fire()
    {
        if (Items != null && Items.Count > 0)
        {
            int index = Items.Count - 1;
            Item item = Items[index];
            Items.RemoveAt(index);
            if (item != null)
            {                
                item.Fire();
            }

            if (Items.Count == 0)
            {
                Discard();
            }
        }
    }

    /// <summary>
    /// rotate the items around the owner
    /// </summary>
    public override void Update()
    {
        transform.Rotate(Vector3.up, RotateSpeed * Time.deltaTime);
    }
}

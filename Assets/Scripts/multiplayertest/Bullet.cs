using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Bullet : NetworkBehaviour
{
    private Rigidbody rb;

	// Use this for initialization
	void Start () 
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = Vector3.forward * 15f;
	}
}

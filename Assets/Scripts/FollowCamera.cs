using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour 
{
    public GameObject FollowObject;      
    public Vector3 FollowDistance;
    public Vector3 FollowRotation;

	// Use this for initialization
	void Start () 
    {
		
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (FollowObject != null)
        {
            transform.localPosition = FollowDistance;
            transform.localRotation = Quaternion.Euler(FollowRotation);
        }
	}
}

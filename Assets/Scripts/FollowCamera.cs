using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour 
{
    public Transform Target;
    public Vector3 Offset;
    public float DistanceDamp;
    public Vector3 Velocity;

	void LateUpdate() 
    {
        //followSmoothDamp();
        simpleFollow();
	}

    void followSmoothDamp()
    {
        if (Target != null)
        {
            Vector3 toPos = Target.position + (Target.rotation * Offset);
            Vector3 curPos = Vector3.SmoothDamp(transform.position, toPos, ref Velocity, DistanceDamp);
            transform.position = curPos;
            transform.LookAt(Target, Target.up);
        }
    } 

    void simpleFollow()
    {
        transform.position = Target.position + (Target.forward * Offset.z) + (Target.up * Offset.y);
        transform.LookAt(Target.position + (Target.forward * 10), Target.up);    
    }
}

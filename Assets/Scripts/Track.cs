using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Track : MonoBehaviour 
{

    public GameObject RoadObj;

	// Use this for initialization
	void Start () 
    {
        if (RoadObj != null)
        {
            for(int x = -50; x < 50; x++)
            {
                for (int z = -50; z < 50; z++)
                {
                    GameObject roadPiece = Instantiate(RoadObj) as GameObject;
                    if (roadPiece != null)
                    {
                        roadPiece.transform.position = new Vector3(x * 10, 0, z * 10);
                    }
                }
            }
        }
	}
}

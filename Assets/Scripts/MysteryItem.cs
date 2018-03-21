using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MysteryItem : MonoBehaviour 
{
    public float RespawnTime;
    public float SpinSpeed;

    private Transform child;

    void Start()
    {
        child = transform.GetChild(0);
    }

    void Update()
    {
        transform.Rotate(Vector3.up, SpinSpeed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Kart")
        {
            StartCoroutine("despawn");
            child.gameObject.SetActive(false);
        }
    }

    IEnumerator despawn()
    {
        yield return new WaitForSeconds(RespawnTime);
        child.gameObject.SetActive(true);
    }
}

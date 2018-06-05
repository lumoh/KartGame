using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class player : NetworkBehaviour
{
    public float MoveForce;
    public float JumpForce;
    public float GravityForce;

    public GameObject BulletPrefab;

    private Rigidbody rb;
    private bool onFloor;
    private bool isStunned = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        if (isStunned)
        {
            return;
        }

        if (Input.GetKey(KeyCode.W))
        {
            rb.AddForce(Vector3.forward * MoveForce);
        }
        if (Input.GetKey(KeyCode.A))
        {
            rb.AddForce(Vector3.left * MoveForce);
        }
        if (Input.GetKey(KeyCode.S))
        {
            rb.AddForce(Vector3.back * MoveForce);
        }
        if (Input.GetKey(KeyCode.D))
        {
            rb.AddForce(Vector3.right * MoveForce);
        }

        if (Input.GetKeyDown(KeyCode.Space) && onFloor)
        {
            rb.AddForce(Vector3.up * JumpForce, ForceMode.Impulse);
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            Cmd_Fire();
        }
    }

    void FixedUpdate()
    {
        if (!isLocalPlayer)
        {
            return;
        }            

        int layerMask = 1 >> LayerMask.NameToLayer("Floor");
        layerMask = ~layerMask;
        bool hit = Physics.Raycast(transform.position, Vector3.down, 0.55f, layerMask);
        if (hit)
        {
            onFloor = true;
        }
        else
        {
            onFloor = false;
        }
        rb.AddForce(Vector3.down * GravityForce);
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Projectile")
        {
            rb.AddForce(Vector3.up * JumpForce * 3, ForceMode.Impulse);
            isStunned = true;
            StartCoroutine("stunTimer");
        }
    }

    IEnumerator stunTimer()
    {
        yield return new WaitForSeconds(5.0f);
        isStunned = false;
    }

    [Command]
    public void Cmd_Fire()
    {
        if (BulletPrefab != null)
        {
            
// no authority            
//            GameObject bullet = Instantiate(BulletPrefab, transform.position + (Vector3.forward * 1.5f), Quaternion.identity);
//            bullet.GetComponent<Rigidbody>().velocity = Vector3.forward * 15f;
//            NetworkServer.Spawn(bullet);
//            Destroy(bullet, 2.0f);

            // local authority
            GameObject bullet = Instantiate(BulletPrefab, transform.position + (Vector3.forward * 1.5f), Quaternion.identity);
            if (bullet != null)
            {
                NetworkServer.SpawnWithClientAuthority(bullet, base.connectionToClient);
            }
        }
    }
}

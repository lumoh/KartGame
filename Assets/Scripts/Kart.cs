using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Go-Kart Style Car
/// </summary>
public class Kart : MovingObject 
{
    [Header("Kart Attributes")]
    public float Weight;
    public float TopSpeed;
    public float AccelerationTime;
    public float DecelerationTime;
    public float ReverseSpeed;
    public float TurnSpeed;
    public float DeadZone;
    public float SlideZone;
    public float SlidingTurnSpeed;
    public float SlidingBoost;

    [Header("Kart Objects")]
    public GameObject KartCamPrefab;
    public KartUI UI;

    [HideInInspector]
    public FollowCamera FollowCam;

    [HideInInspector]
    public Item CurrentItem;
    public Transform ItemOrigin;

    [Header("Kart Wheels")]
    public List<Transform> RotatingWheels;
    public List<Transform> TurningWheels;
    public ParticleSystem[] DustTrails = new ParticleSystem[2];

    private float thrust;
    private float thrustRatio;
    private bool isMoving;   
    private bool isMovingForward;
    private bool isSliding;
    private bool slidingRight;
    private bool slidingLeft;

    private float vInput;
    private float hInput;
    private float jumpInput;

    [HideInInspector]
    public NetworkSyncTransform NetworkSync;

    void Start()
    {
        NetworkSync = GetComponent<NetworkSyncTransform>();
        col = GetComponent<BoxCollider>();
        body = GetComponent<Rigidbody>();

        layerMask = 1 << LayerMask.NameToLayer("Kart");
        layerMask = ~layerMask;

        if (KartCamPrefab != null)
        {
            GameObject camObj = Instantiate(KartCamPrefab) as GameObject;
            if (camObj != null)
            {
                FollowCam = camObj.GetComponent<FollowCamera>();
                FollowCam.Target = transform;
                if (isLocal())
                {
                    Camera cam = FollowCam.GetComponent<Camera>();
                    cam.enabled = true;

                    if (UI != null)
                    {
                        UI.gameObject.SetActive(true);
                    }
                }
            }
        }
    }

    bool isLocal()
    {
        return NetworkSync != null && NetworkSync.isLocalPlayer;
    }

    void Update()
    {
        if (!isLocal())
        {
            return;
        }

        input();
    }

    public override void FixedUpdate()
    {
        if (!isLocal())
        {
            return;
        }

        setDirection();
        gravity();
        movement();
        turning();
        slideParticles();
    }

    /// <summary>
    /// Handle input for the kart
    /// </summary>
    void input()
    {
        // fire!
        if (Input.GetKeyDown(KeyCode.E))
        {
            fire();
        }

        vInput = Input.GetAxis("Vertical");
        hInput = Input.GetAxis("Horizontal");
        jumpInput = Input.GetAxis("Jump");

        if (vInput > 0)
        {
            vInput = 1;
        }
        else if (vInput < 0)
        {
            vInput = -1;
        }

        // if drive or reverse use acceleration time
        if (vInput == 1 || vInput == -1)
        {
            float accelTime = AccelerationTime;
            if (vInput == 1 && !isMovingForward)
            {
                accelTime /= 2;
            }
            thrustRatio += Time.deltaTime / accelTime * vInput;
            thrustRatio = Mathf.Clamp(thrustRatio, -1, 1);
        }
        else
        {
            // coast to stop
            if (thrustRatio > 0)
            {
                thrustRatio -= Time.deltaTime / DecelerationTime;
                if (thrustRatio < 0)
                {
                    thrustRatio = 0;
                }
            }
            else
            {
                thrustRatio += Time.deltaTime / DecelerationTime;
                if (thrustRatio > 0)
                {
                    thrustRatio = 0;
                }
            }
        }            

        if (isMovingForward)
        {
            thrust = thrustRatio * TopSpeed;
        }
        else
        {
            thrust = thrustRatio * ReverseSpeed;
        }

        if (isSliding)
        {
            float turnRange = 1.0f - SlideZone;  
            float halfTurn = turnRange / 2.0f;
            float midPoint = SlideZone + halfTurn;

            if (slidingRight)
            {                
                hInput =  midPoint + (halfTurn * hInput);
            }
            else if( slidingLeft)
            {
                hInput = -midPoint + (halfTurn * hInput);
            }
        }
    }

    /// <summary>
    /// set direction status vars
    /// </summary>
    void setDirection()
    {
        isMoving = body.velocity.magnitude > DeadZone;
        isMovingForward = Vector3.Dot(transform.forward, body.velocity) >= 0;

        if(jumpInput == 0)
        {
            isSliding = false;
        }

        if (!isSliding)
        {
            slidingRight = jumpInput > 0 && hInput > SlideZone;
            slidingLeft = jumpInput > 0 && hInput < -SlideZone;
            isSliding = slidingRight || slidingLeft;
        }

//        if (isSliding)
//        {
//            Debug.Log("Sliding!");
//        }

        UI.SetSpeed((int)body.velocity.magnitude);
    }

    /// <summary>
    /// handle kart gravity and being grounded to the surface it is driving on
    /// </summary>
    protected override void gravity()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, -Vector3.up, out hit, HeightOffGround + 0.1f, layerMask))
        {
            isGrounded = true;

            Quaternion rot = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
            transform.rotation = Quaternion.Lerp(transform.rotation, rot, 1);

            // lock Y pos to floor
            Vector3 pos = transform.position;
            float newY = (hit.point + (hit.normal * HeightOffGround)).y;
            pos.y = newY;
            transform.position = pos;

            body.drag = GroundedDrag;
            body.angularDrag = GroundedAngularDrag;
        }
        else
        {
            isSliding = false;
            isGrounded = false;
            body.AddForceAtPosition(transform.up * -GravityForce, transform.position);

            body.drag = AirDrag;
            body.angularDrag = AirAngularDrag;
        }
    }

    /// <summary>
    /// handle turning
    /// </summary>
    void turning()
    {
        float flip = 1;
        if (!isMovingForward)
        {
            flip = -1;
        }

        if (hInput != 0 && thrustRatio != 0)
        {
            float turnSpeed = TurnSpeed;
            if (isSliding)
            {
                turnSpeed = SlidingTurnSpeed;
            }

            body.AddRelativeTorque(Vector3.up * hInput * turnSpeed * flip);
        }

        foreach (Transform wheelT in TurningWheels)
        {
            wheelT.localRotation = Quaternion.Euler(0, hInput * 35.0f * thrustRatio * flip, 0);
        }
    }

    /// <summary>
    /// handle movement and thrust
    /// </summary>
    void movement()
    {
        if (isGrounded)
        {
            if (thrust != 0)
            {
                Vector3 force = transform.forward * thrust;
                if (isSliding)
                {
                    force *= SlidingBoost;
                }
                body.AddForce(force, ForceMode.Acceleration);
            }
        }

        foreach (Transform wheelT in RotatingWheels)
        {                
            wheelT.Rotate(Vector3.up, wheelT.rotation.y + (-thrust * 2.0f));
        }            
    }

    /// <summary>
    /// Slides the particles.
    /// </summary>
    void slideParticles()
    {
        if (isSliding && DustTrails.Length > 0)
        {
            for (int i = 0; i < DustTrails.Length; i++)
            {
                if (DustTrails[i] != null)
                {
                    if (!DustTrails[i].isPlaying)
                    { 
                        DustTrails[i].Play();
                    }
                }
            }
        }
        else
        {
            for (int i = 0; i < DustTrails.Length; i++)
            {
                if (DustTrails[i] != null)
                {
                    if (DustTrails[i].isPlaying)
                    { 
                        DustTrails[i].Stop();
                    }
                }
            }
        }
    }

    void fire()
    {
        if (CurrentItem != null)
        {
            CurrentItem.Fire();   
        }
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Track")
        {
            vInput = 0;
            thrustRatio = 0;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Item")
        {
            if (CurrentItem == null)
            {
                GameObject itemPrefab = Resources.Load<GameObject>("Items/ProjectileSet");
                if (itemPrefab != null)
                {
                    GameObject itemObj = Instantiate(itemPrefab) as GameObject;
                    if (itemObj != null)
                    {                    
                        itemObj.transform.SetParent(ItemOrigin);
                        itemObj.transform.localPosition = Vector3.zero;
                        Item item = itemObj.GetComponent<Item>();
                        if (item != null)
                        {
                            CurrentItem = item;
                            CurrentItem.Owner = this;
                            CurrentItem.Activate();
                        }
                    }
                }
            }
        }
    }
}

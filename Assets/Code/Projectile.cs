using UnityEngine;
using Mirror;


public class Projectile : NetworkBehaviour
{
    [Header("Projectile Propertys")]
    public int damage;

    public float destroyDelay = 5;
    public float initalForce = 0;
    public float constForce = 5;

    [Header("Dont Go Through Stuff")]
    private Vector3 previousPos;
    private float minimumExtent;
    private float partialExtent;
    private float sqrMinimumExtent;
    private Vector3 previousPosition;

    public LayerMask myLayer;

    public Collider myCollider;
    private Rigidbody rb;
    private NetworkTransform netTrans;


    // Server and Clients must run
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        netTrans = GetComponent<NetworkTransform>();

        if (rb.isKinematic == false)
            rb.AddForce(transform.up * initalForce, ForceMode.Impulse);
    }

    public override void OnStartServer()
    {
        Invoke(nameof(DestroySelf), destroyDelay);

        previousPosition = rb.position;
        minimumExtent = Mathf.Min(Mathf.Min(myCollider.bounds.extents.x, myCollider.bounds.extents.y), myCollider.bounds.extents.z);
        partialExtent = minimumExtent * 0.9f;
        sqrMinimumExtent = minimumExtent * minimumExtent;
    }

    void Update()
    {
        if (rb.isKinematic)
            rb.MovePosition(transform.position + transform.up * constForce * Time.deltaTime);
        else
            rb.AddForce(transform.up * constForce, ForceMode.Force);
    }

    [ServerCallback] //only the server needs to do this
    void FixedUpdate()
    {
        //have we moved more than our minimum extent? 
        Vector3 movementThisStep = rb.position - previousPos;
        float movementSqrMagnitude = movementThisStep.sqrMagnitude;

        if (movementSqrMagnitude > sqrMinimumExtent)
        {
            float movementMagnitude = Mathf.Sqrt(movementSqrMagnitude);
            RaycastHit hit;

            //check for obstructions we might have missed 
            if (Physics.Raycast(previousPos, movementThisStep, out hit, movementMagnitude, myLayer.value))
            {
                if (hit.collider == myCollider)
                    return;

               // rb.position = hit.point - (movementThisStep / movementMagnitude) * partialExtent;
               // netTrans.ServerTeleport(rb.position);

               // if (hit.collider.tag == "Player")
                //{
                //    var player = hit.collider.gameObject.GetComponent<Player>();
                //    if (player != null)
               //         player.health -= damage;
                //    return;
                //}
            }
        }

        previousPos = rb.position;
    }

    // destroy for everyone on the server
    [Server]
    void DestroySelf()
    {
        NetworkServer.Destroy(gameObject);
    }

    // ServerCallback because we don't want a warning if OnTriggerEnter is
    // called on the client
    [ServerCallback]
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            var player = other.GetComponent<Player>();
            if (player != null)
                player.health -= damage;
            return;
        }
    }
}


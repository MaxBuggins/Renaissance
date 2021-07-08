using UnityEngine;
using Mirror;


public class Projectile : NetworkBehaviour
{
    [Header("Projectile Propertys")]
    public int damage;

    public float destroyDelay = 5;
    public float initalForce = 5;
    public float constForce = 5;
    public float maxVelocity = 10;

    public float collisionDelay = 0.15f;
    public int destoryOnHits = 0;

    [Header("Projectile Refrences")]
    private Rigidbody rb;
    private NetworkTransform netTrans;

    public GameObject hitPartical;


    // Server and Clients must run
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        netTrans = GetComponent<NetworkTransform>();

        rb.detectCollisions = false;
        Invoke(nameof(ActivateCollision), collisionDelay);

        rb.AddForce(transform.up * initalForce, ForceMode.VelocityChange);
    }

    public override void OnStartServer()
    {
        Invoke(nameof(DestroySelf), destroyDelay);
    }

    void Update()
    {
        if(rb.velocity.magnitude < maxVelocity)
            rb.AddForce(transform.up * constForce * Time.deltaTime, ForceMode.Acceleration);

    }

    // destroy for everyone on the server
    [Server]
    void DestroySelf()
    {
        NetworkServer.Destroy(gameObject);
    }

    void ActivateCollision()
    {
        rb.detectCollisions = true;
    }

    [ServerCallback]
    // ServerCallback because we don't want a warning if OnTriggerEnter is
    // called on the client
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            var player = collision.gameObject.GetComponent<Player>();
            if (player != null)
                player.health -= damage;
        }

        RpcHit();

        destoryOnHits -= 1;
        if (destoryOnHits < 0)
            Invoke(nameof(DestroySelf), 0.1f);
    }

    [ClientRpc] //server tells all clients it has hit
    void RpcHit()
    {
        Instantiate(hitPartical, transform.position, transform.rotation);
    }
}


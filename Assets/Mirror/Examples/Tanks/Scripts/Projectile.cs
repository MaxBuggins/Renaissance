using UnityEngine;
using Mirror;


public class Projectile : NetworkBehaviour
{
    public float destroyDelay = 5;
    public float initalForce = 0;
    public float constForce = 5;

    private Rigidbody rb;

    public override void OnStartServer()
    {
        Invoke(nameof(DestroySelf), destroyDelay);
    }

    // set velocity for server and client. this way we don't have to sync the
    // position, because both the server and the client simulate it.
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        rb.AddForce(transform.up * initalForce, ForceMode.Impulse);
    }

    void Update()
    {
        rb.AddForce(transform.up * constForce, ForceMode.Force);
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
    void OnTriggerEnter(Collider co)
    {
        DestroySelf();
    }
}


using UnityEngine;
using Mirror;


public class Projectile : NetworkBehaviour
{
    [Header("Projectile Propertys")]
    public int damage;

    public float destroyDelay = 5;
    //public float initalForce = 5;
    public float forwardSpeed = 5;
    public float gravitY = -9;


    public float projectileWidth = 0.3f;
    public int destoryOnHits = 0;

    [Header("Internals")]
    private Vector3 lastPos;
    private Vector3 velocity;

    [Header("Projectile Refrences")]
    private NetworkTransform netTrans;
    private Hurtful hurtful;

    public GameObject hitObject;
    public GameObject hitSplat;

    public OnCall destoyCall;


    // Server and Clients must run
    void Awake()
    {
        netTrans = GetComponent<NetworkTransform>();
        hurtful = GetComponent<Hurtful>();

        lastPos = transform.position;

        Invoke(nameof(DestroySelf), destroyDelay);
    }

    void Update()
    {
        velocity.y += gravitY * Time.deltaTime;
        //ADD PLAYER VELOCITY TO IT
        transform.position = transform.position + ((Vector3.up * velocity.y)
            + (transform.forward * forwardSpeed)) * Time.deltaTime;

        if (isServer)//Adian Smells of car fuel
        {
            //Ray ray = new Ray(transform.position, player.position - transform.position)
            RaycastHit hit;
            if (Physics.SphereCast(transform.position, projectileWidth, transform.position - lastPos,
                out hit, maxDistance: Mathf.Abs(Vector3.Distance(transform.position, lastPos)) * 1.25f, 
                -1, QueryTriggerInteraction.Ignore))
            {
                Player player = hit.collider.gameObject.GetComponent<Player>();
                if (player != null)
                {
                    hurtful.HurtPlayer(player, damage, hurtful.hurtType);

                    destoryOnHits -= 1;

                    if (destoryOnHits < 0)
                        NetworkServer.Destroy(gameObject);
                }
                else
                {
                    DestroySelf();
                }
            }
        }
        lastPos = transform.position;
    }

    // everyoneDestroys
    void DestroySelf()
    {
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        var obj = Instantiate(hitObject, lastPos, transform.rotation);

        if (isServer)
        {
            if (destoyCall != null)
                destoyCall.Call(hurtful.owner);

            Hurtful hurt = obj.GetComponentInChildren<Hurtful>();
            if (hurt != null)
                hurt.owner = hurtful.owner;
        }

        if(hitSplat != null)
            Instantiate(hitSplat, lastPos, transform.rotation);
    }
}



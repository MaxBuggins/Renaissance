using UnityEngine;
using Mirror;


public class Projectile : NetworkBehaviour
{
    [Header("Projectile Propertys")]
    public int damage;

    public float destroyDelay = 5;
    //public float initalForce = 5;
    public float forwardSpeed = 5; //if less than 0 then its a instant raycast
    public float gravitY = -9;
    public float airRistance = 0.1f;

    public float projectileWidth = 0.3f;
    public int destoryOnHits = 0;
    public bool disableOnHit = false;

    [Header("Internals")]
    public LayerMask mask;
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

        /*
        if (forwardSpeed < 0)
        {
            enabled = false; //stops updates

            if (!isServer)
            {
                RaycastHit hit;
                if (Physics.SphereCast(transform.position, projectileWidth, transform.position - lastPos,
                    out hit, maxDistance: Mathf.Infinity, mask, QueryTriggerInteraction.Ignore))
                {
                    Player player = hit.collider.gameObject.GetComponent<Player>();
                    if (player != null) //5 indents lets goooooooo eat fish
                    {
                        hurtful.HurtPlayer(player, damage, hurtful.hurtType);
                        DestroySelfHit();
                    }
                }
            }
        }
        */
    }

    void Update()
    {
        velocity.y += gravitY * Time.deltaTime;
        transform.position = transform.position + ((Vector3.up * velocity.y)
            + (transform.forward * forwardSpeed)) * Time.deltaTime;

        forwardSpeed -= airRistance * Time.deltaTime;

        if (isServer)//Adian Smells of car fuel
        {
            //Ray ray = new Ray(transform.position, player.position - transform.position)
            RaycastHit hit;
            if (Physics.SphereCast(transform.position, projectileWidth, transform.position - lastPos,
                out hit, maxDistance: Mathf.Abs(Vector3.Distance(transform.position, lastPos)) * 1.25f,
                mask, QueryTriggerInteraction.Ignore))
            {
                Player player = hit.collider.gameObject.GetComponent<Player>();
                if (player != null)
                {
                    hurtful.HurtPlayer(player, damage, hurtful.hurtType);
                    DestroySelfHit();
                }
                else
                {
                    if (hitSplat != null)
                        Instantiate(hitSplat, hit.point, Quaternion.LookRotation(hit.normal));

                    destoryOnHits -= 1;

                    if (destoryOnHits < 0)
                        DestroySelfHit();

                    else
                    {
                        Vector3 forw = transform.forward;
                        Vector3 mirrored = Vector3.Reflect(forw, hit.normal);
                        transform.rotation = Quaternion.LookRotation(mirrored, transform.up);
                        transform.position = hit.point;

                        RpcSyncProjectile(transform.position, transform.eulerAngles, true);
                    }
                }
            }
        }
        lastPos = transform.position;
    }

    // everyoneDestroys
    [Server]
    void DestroySelfHit()
    {
        if (hitObject != null)
        {
            GameObject obj = Instantiate(hitObject, lastPos, transform.rotation);

            Hurtful hurt = obj.GetComponentInChildren<Hurtful>();
            if (hurt != null)
                hurt.owner = hurtful.owner;
        }

            if (destoyCall != null)
            destoyCall.Call(hurtful.owner);

        //if (hitSplat != null)
            //Instantiate(hitSplat, lastPos, transform.rotation);

        RpcDestroySelfHit();

        DestroySelf();
    }

    [ClientRpc]
    void RpcDestroySelfHit()
    {
        if(hitObject != null)
            Instantiate(hitObject, lastPos, transform.rotation);

        //if (hitSplat != null)
        //Instantiate(hitSplat, lastPos, transform.rotation);

        DestroySelf();
    }

    void DestroySelf()
    {
        if (!disableOnHit)
            Destroy(gameObject);
        else
        {
            enabled = false;
            hurtful.enabled = false;

            if(GetComponent<AudioSource>() != null)
                GetComponent<AudioSource>().enabled = false;

            RaycastHit hit;
            if (Physics.SphereCast(transform.position, 0.2f, transform.forward * 0.4f, out hit))
            {

                transform.parent = hit.transform;
            }
        }

    }

    [ClientRpc]
    void RpcSyncProjectile(Vector3 pos, Vector3 rot, bool hit)
    {
        if (hit && hitObject != null)
            Instantiate(hitObject, lastPos, transform.rotation);

        transform.position = pos;
        transform.eulerAngles = rot;
    }
}
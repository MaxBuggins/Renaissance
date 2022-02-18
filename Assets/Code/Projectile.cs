using UnityEngine;
using Mirror;


public class Projectile : NetworkBehaviour
{
    public float destroyDelay = 5;

    [Header("Projectile Damage")]
    public int damage;
    protected Vector3 orginPos;

    public float startDmgFallOff = 12;
    public float minDmgMultiplyer = 0.25f;

    [Header("Projectile Movement")]
    public float forwardSpeed = 5; //if less than 0 then its a instant raycast
    public float gravitY = -9;
    public float airRistance = 0.1f;
    private float totalAirRistance = 0;

    public float projectileWidth = 0.3f;

    [Header("Projectile HitRequirements")]
    public int destoryOnHits = 0;
    public bool bounceOffPlayerOnly = false;
    public bool destoryOnPlayerHit = true;
    public bool disableOnHit = false;
    public bool mustHitNormalGround = false;

    [Header("Internals")]
    public LayerMask mask;
    public Vector3 velocity;
    private Vector3 lastPos;

    [Header("Projectile Refrences")]
    private NetworkTransform netTrans;
    protected Hurtful hurtful;

    public bool serverSpawnHitObject = false;
    public GameObject hitObject;
    public GameObject hitSplat;

    public OnCall destoyCall;


    // Server and Clients must run
    public void Awake()
    {
        netTrans = GetComponent<NetworkTransform>();
        hurtful = GetComponent<Hurtful>();

        lastPos = transform.position;
        orginPos = transform.position;

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

    protected void Update()
    {
        velocity.y += gravitY * Time.deltaTime;
        transform.position = transform.position + ((Vector3.up * velocity.y)
            + (transform.forward * (forwardSpeed + totalAirRistance))) * Time.deltaTime;

        totalAirRistance -= airRistance * Time.deltaTime;

        if (isServer)//Adian Smells of car fuel
        {
            //Ray ray = new Ray(transform.position, player.position - transform.position)
            RaycastHit hit;
            //Debug.DrawLine(lastPos, transform.position, Color.green, 10);
            if (Physics.SphereCast(lastPos, projectileWidth, transform.position - lastPos,
                out hit, maxDistance: Mathf.Abs(Vector3.Distance(transform.position, lastPos) * 1.1f),
                mask, QueryTriggerInteraction.Ignore))
            {
                Player player = hit.collider.gameObject.GetComponentInParent<Player>();
                if (player != null)
                {
                    if (hurtful.ignorOwner && player == hurtful.owner)
                        return; //dont interact with the shooter

                    float dist = Vector3.Distance(orginPos, transform.position);
                    int dmg = damage;

                    if (dist > startDmgFallOff) //Dmg roll off math
                    {
                        dist -= startDmgFallOff;
                        dmg = (int)(-0.0005f * (Mathf.Pow(dist, 4)) + dmg); //math moment with aidan
                        if (dmg < damage * minDmgMultiplyer) //min of 25% at about the distance of fog
                            dmg = (int)(damage * minDmgMultiplyer);
                    }

                    hurtful.HurtPlayer(player, dmg, hurtful.hurtType);

                    if (bounceOffPlayerOnly)
                    {
                        destoryOnHits += 1;
                    }

                    else if(destoryOnPlayerHit)
                        DestroySelfHit();
                }
                else
                {
                    if (hitSplat != null)
                        Instantiate(hitSplat, hit.point, Quaternion.LookRotation(hit.normal));
                }



                if(mustHitNormalGround == true && Vector3.Dot(hit.normal, Vector2.up) < 0.9f) //Dot your nannny
                    destoryOnHits++; //allows the projectile to be hit again 

                print(hit.normal);

                if (destoryOnHits > -1)
                {
                    destoryOnHits -= 1;

                    if (destoryOnHits < 0)
                        DestroySelfHit();
                    else
                    {
                        Vector3 forw = (transform.position - lastPos).normalized;
                        Vector3 mirrored = Vector3.Reflect(forw, hit.normal);
                        transform.rotation = Quaternion.LookRotation(mirrored, transform.up);
                        transform.position = hit.point;
                        velocity = Vector3.zero; //reset for gravity projectiles
                        totalAirRistance = 0;

                        if(hurtful != null)
                            hurtful.ignorOwner = false;

                        RpcSyncProjectile(transform.position, transform.eulerAngles, !bounceOffPlayerOnly);
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

            ItemPickUp item = obj.GetComponent<ItemPickUp>(); //only map spawned items should respawn (LAZY CODE)
            if (item != null)
                item.respawn = false;


            if (serverSpawnHitObject)
                NetworkServer.Spawn(obj);

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
        if(hitObject != null && serverSpawnHitObject == false)
            Instantiate(hitObject, transform.position, transform.rotation);

        if (hitSplat != null)
            Instantiate(hitSplat, transform.position, transform.rotation);

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
            Instantiate(hitObject, transform.position, transform.rotation);

        if (hitSplat != null)
            Instantiate(hitSplat, transform.position, transform.rotation);

        transform.position = pos;
        transform.eulerAngles = rot;

        velocity = Vector3.zero; //reset for gravity projectiles
        totalAirRistance = 0;
    }
}
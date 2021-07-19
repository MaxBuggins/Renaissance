using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;
using Pixelplacement;

public class Player : NetworkBehaviour
{
    [Header("Player Info")]
    public TextMeshPro playerNameText;
    public GameObject floatingInfo;

    [Header("Player Stats")]
    [SyncVar] public int score;

    public int maxHealth = 100;
    [SyncVar(hook = nameof(OnHealthChanged))]
    public int health;

    public int maxSpecial = 10;
    [SyncVar(hook = nameof(OnSpecialChanged))]
    public int special;

    public float specialChargeRate = 4;
    private float specialChargeTime = 0;

    [Header("Player Info")]
    [SyncVar(hook = nameof(OnNameChanged))]
    public string playerName = "NoNameNed";
    [SyncVar(hook = nameof(OnColorChanged))]
    public Color playerColour = Color.white;

    public ObjectPlayerClass playerClass;

    public Vector3 cameraOffset;

    [Header("Player Atrabuits")] //internal use only on spawn resets varibles accoring to playerclass
    [HideInInspector] public float speed = 5;
    [HideInInspector] public float backSpeedMultiplyer = 0.65f;
    [HideInInspector] public float sideSpeedMultiplyer = 0.8f;
    [HideInInspector] public float airMovementMultiplyer = 0.6f;

    [HideInInspector] public float gravitY = -10f; //manual gravity for gameplay reasons
    [HideInInspector] public float fricktion = 5f;
    [HideInInspector] public float maxMoveVelocity = 6;

    [HideInInspector] public float jumpHeight = 2f;
    [HideInInspector] public float coyotTime = 0.3f; //lol mesh has good idears NO WAY

    [HideInInspector] public float pushForce = 5f;

    public float hurtScale = 1.1f;
    public float hurtDuration = 0.5f;
    public AnimationCurve hurtCurve;

    [Header("Player Internals")]
    public bool paused;

    private Vector2 move;
    private Vector3 lastPos;

    public Vector3 velocity;
    private float fallTime; //for counting seconds of falling

    private float deadTime;

    private Controls controls;

    [Header("Unity Stuff Internals")]
    public GameObject cameraPrefab;
    [HideInInspector] public PlayerCamera playerCam;
    [HideInInspector] public CharacterController character;
    private AudioSource audioSource;

    public DirectionalSprite body;
    //private Animator animator;
    //public Transform bodyTrans;

    public GameObject corpsePrefab;
    private GameObject corpseObject;

    private NetworkTransform netTrans;

    private LevelManager levelManager;
    public GameObject[] spawnableObjects;

    private ClientManager clientManager;
    public UI_Main uIMain;


    public override void OnStartLocalPlayer() //just for the local client
    {
        body.gameObject.SetActive(false);

        clientManager = FindObjectOfType<ClientManager>();
        uIMain = FindObjectOfType<UI_Main>();
        uIMain.player = this;
        uIMain.UIUpdate();

        controls = new Controls();

        controls.Game.Jump.performed += funny => Jump();

        controls.Game.Move.performed += ctx => move = ctx.ReadValue<Vector2>();
        controls.Game.Move.canceled += ctx => move = Vector2.zero;

        controls.Game.Pause.performed += funnyer => Pause(!paused);

        controls.Enable();

        playerCam = Instantiate(cameraPrefab, transform.position + cameraOffset,
            transform.rotation, transform).GetComponent<PlayerCamera>();

        CmdSetupPlayer(clientManager.playerName, clientManager.playerColour);
        OwnerSpawnPlayer();

        lastPos = transform.position;
    }

    private void Start() //for all to run
    {
        character = GetComponent<CharacterController>();
        audioSource = GetComponent<AudioSource>();
        netTrans = GetComponent<NetworkTransform>();

        //animator = GetComponentInChildren<Animator>();

        levelManager = FindObjectOfType<LevelManager>();
    }

    void FixedUpdate()
    {
        if (isServer)
        {
            //prevents infiti special stacking
            if (health > 0)
            {
                specialChargeTime += Time.fixedDeltaTime;

                if (special > maxSpecial)
                    special = maxSpecial;

                else if (specialChargeTime > specialChargeRate)
                {
                    special += 1;
                    specialChargeTime = 0;
                }
            }
        }


        if (!isLocalPlayer) //only the player runs whats next
            return;

        //velocity += transform.position - lastPos;

        if (health <= 0)
        {
            IsDead();
            return;
        }

        IsGrounded();
        Movement();

    }

    private void LateUpdate()
    {
        float speedMultiplyer = 600;
        if (isServer)
            speedMultiplyer = 3000;

        //body.directionalSprites = playerClass.runSprites[1].sprites;
        //animator.speed = Vector3.Distance(transform.position, lastPos) * Time.deltaTime * speedMultiplyer;

        lastPos = transform.position; //must be done after movement and stuff
    }

    [ClientCallback] //only for a client to run
    void Movement() //all the movement nesseary for the player only run by the client
    {
        move = move.normalized;
        float x = move.x;
        float z = move.y;

        //applys diffrent directional movement speeds
        if (z < 0)
            z = z * backSpeedMultiplyer;
        x = x * sideSpeedMultiplyer;

        Vector3 movement = transform.right * x + transform.forward * z;

        if (fallTime > coyotTime)
            movement = movement * airMovementMultiplyer;


        //velocity.x = Mathf.Clamp(velocity.x, -maxMoveVelocity, maxMoveVelocity);
        //velocity.z = Mathf.Clamp(velocity.z, -maxMoveVelocity, maxMoveVelocity);

        velocity.x = Mathf.Lerp(velocity.x, 0, fricktion * Time.fixedDeltaTime);
        velocity.z = Mathf.Lerp(velocity.z, 0, fricktion * Time.fixedDeltaTime);

        if (Mathf.Abs(velocity.x) < maxMoveVelocity && Mathf.Abs(velocity.z) < maxMoveVelocity)
            movement *= speed;

        if (paused)
            movement = Vector3.zero;

        character.Move((movement + velocity) * Time.fixedDeltaTime); //apply movement to charhcter contoler

        velocity += transform.right * x + transform.forward * z;
    }

    [ClientCallback]
    void Jump()
    {
        if (paused)
            return;

        if (fallTime < coyotTime)
        {
            fallTime = coyotTime;
            velocity.y = Mathf.Sqrt(jumpHeight * -2.0f * gravitY * 2); //physics reasons 
        }
    }

    void Pause(bool pause)
    {
        paused = pause;
        uIMain.Pause(pause);

        if (pause == true)
            Cursor.lockState = CursorLockMode.None;
        else
            Cursor.lockState = CursorLockMode.Locked;
    }

    [ClientCallback]
    void IsGrounded()
    {
        if (character.isGrounded == true)
        {
            if (velocity.y < 0) //if falling
                velocity.y = 0; //if grounded then no need to fall

            if (fallTime > 0)
                fallTime -= Time.fixedDeltaTime * 3;
        }
        else
        {
            velocity.y += gravitY * Time.fixedDeltaTime; //two deltra time cause PHJYSCIS

            if (fallTime < coyotTime)
                fallTime += Time.fixedDeltaTime;

            //transform.parent = null;
            //transform.rotation = new Quaternion(0, transform.rotation.y, 0, transform.rotation.w);
        }
    }

    [ClientCallback]
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        //hitNormal = hit.normal;

        Rigidbody body = hit.collider.attachedRigidbody;
        Vector3 pushDir;

        //only for useable rigidbodys
        if (body == null)
            return;

        //dont push objects below
        if (hit.moveDirection.y < -0.3)
            return;
        else
        {
            pushDir = (hit.moveDirection);
        }

        // Apply the push
        body.velocity = pushDir * pushForce;
    }

    void OnNameChanged(string _Old, string _New)
    {
        playerNameText.text = playerName;
    }

    void OnColorChanged(Color _Old, Color _New)
    {
        playerNameText.color = _New;
        //playerMaterialClone = new Material(GetComponent<Renderer>().material);
        //playerMaterialClone.color = _New;
        //GetComponent<Renderer>().material = playerMaterialClone;
    }

    void OnHealthChanged(int _Old, int _New)
    {
        if (isLocalPlayer)
            uIMain.UIUpdate();


        if (_Old > _New) //if damage taken
        {
            Tween.LocalScale(body.transform, Vector3.one * hurtScale, hurtDuration,
                0, hurtCurve);

            audioSource.PlayOneShot(playerClass.hurtSound[Random.Range(0,
                playerClass.hurtSound.Length)]);

            if (_Old > 0 && _New <= 0) //on death
            {
                audioSource.PlayOneShot(playerClass.deathSound[Random.Range(0,
                    playerClass.deathSound.Length)]);
                PlayerAlive(false);
            }
        }

        if (_Old <= 0 && _New > 0) //on death
        {
            PlayerAlive(true);
        }
    }

    void OnSpecialChanged(int _Old, int _New)
    {
        if (isLocalPlayer)
            uIMain.UIUpdate();
    }

    [Client]
    void PlayerAlive(bool alive)
    {
        character.enabled = alive;
        floatingInfo.SetActive(alive);
        //body.SetActive(alive);
        body.transform.position = transform.position + -Vector3.up; //bug fix

        if (alive == true)
        {
            if (corpseObject != null)
                Destroy(corpseObject);

            //corpseRB.GetComponent<Collider>().enabled = false;
        }
        else
        {
            corpseObject = Instantiate(corpsePrefab, body.transform);

            uIMain.UIAddKillFeed("GamerJoe", playerName, null);

        }

        if (isLocalPlayer)
        {
            GetComponentInChildren<PlayerWeapon>().EndSpecial();

            if (alive == true)
                OwnerSpawnPlayer();
        }

        if (!isServer) //only the server runs this
            return;

        //need to sync rigidbody between clients wont work AHHHHHH  
        if (corpseObject != null)
            corpseObject.GetComponentInChildren<Rigidbody>().velocity = (transform.position - lastPos) * 10;
        //corpseRB.GetComponent<Mirror.Experimental.NetworkRigidbody>().SyncToClients();
    }

    [Command]
    public void CmdSetupPlayer(string _name, Color _col)
    {
        // player info sent to server, then server updates sync vars which handles it on all clients
        playerName = _name;
        playerColour = _col;
    }

    [Command]
    public void CmdSpawnPlayer()
    {
        netTrans.ServerTeleport(levelManager.GetSpawnPoint());
        special = 4;
        health = maxHealth;
    }

    [Client]
    public void OwnerSpawnPlayer()
    {
        playerCam.transform.localPosition = cameraOffset;

        //reset according to the players class
        speed = playerClass.speed;
        backSpeedMultiplyer = playerClass.backSpeedMultiplyer;
        sideSpeedMultiplyer = playerClass.sideSpeedMultiplyer;
        airMovementMultiplyer = playerClass.airMovementMultiplyer;

        gravitY = playerClass.gravitY;
        fricktion = playerClass.fricktion;
        maxMoveVelocity = playerClass.maxMoveVelocity;

        jumpHeight = playerClass.jumpHeight;
        coyotTime = playerClass.coyotTime;

        pushForce = playerClass.pushForce;
    }

    [ClientCallback]
    void IsDead()
    {
        deadTime += Time.deltaTime;

        if (deadTime > levelManager.respawnDelay)
        {
            deadTime = 0;
            velocity = Vector3.zero;
            CmdSpawnPlayer();
        }
    }

    [Command]
    public void CmdSpawnObject(int objID, Vector3 pos, Vector3 rot, bool serverOnly, bool playerParent)
    {
        if (health <= 0) //dead players cant spawn objects
            return;

        Transform parent = null;
        if (playerParent)
            parent = transform;

        GameObject spawned = Instantiate(spawnableObjects[objID], pos, Quaternion.Euler(rot), parent);

        if (spawned.GetComponent<Hurtful>() != null)
            spawned.GetComponent<Hurtful>().ignorePlayer = this;

        if (serverOnly == false)
            NetworkServer.Spawn(spawned);
    }

    [ClientRpc]
    public void RpcAddVelocity(Vector3 vel) //TEMP apply to local player only
    {
        velocity += vel;
    }

    [Command]
    public void CmdUseSpecial(int used)
    {
        special -= used;
    }

    [Server]
    public void Hurt(int damage, string killer = "", Sprite hurtIcon = null) //can be used to heal just do -damage
    {
        if (health <= 0)
            return;

        //prevents infiti health stacking
        if (health - damage > maxHealth)
            health = maxHealth;
        else
            health -= damage;

        if(health <= 0)
        {
            score -= 1;
        }
    }
}

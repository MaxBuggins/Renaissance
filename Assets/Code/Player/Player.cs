using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;

public class Player : NetworkBehaviour
{
    [Header("Player Stats")]
    [SyncVar] public int score = 0;
    [HideInInspector] [SyncVar] public int killStreak = 0;

    [HideInInspector] public int maxHealth = 100;
    [SyncVar(hook = nameof(OnHealthChanged))]
    public int health;

    [HideInInspector] public int maxSpecial = 10;
    public int special;

    private float specialChargeTime = 0;

    [Header("Player Info")]
    [SyncVar(hook = nameof(OnNameChanged))]
    [HideInInspector] public string playerName = "NoNameNed";
    [SyncVar(hook = nameof(OnColorChanged))]
    [HideInInspector] public Color32 playerColour = Color.white;

    public ObjectPlayerClass playerClass;

    public Vector3 cameraOffset = Vector3.up;


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
    [HideInInspector] public float slideFriction = 0.3f;

    [HideInInspector] public int bloodPerDamage = 6;
    [HideInInspector] public GameObject bloodObj;

    [Header("Player Internals")]

    public Vector3 velocity;
    private float maxVelocity = 200;

    private float fallTime; //for counting seconds of falling
    private float deadTime;

    [HideInInspector] public bool paused;

    [HideInInspector] public Vector2 move;
    [HideInInspector] public Vector3 lastPos;
    private Vector3 floorNormal;

    [HideInInspector]public Controls controls;

    [Header("Refrences")]
    public GameObject cameraPrefab;
    public GameObject corpsePrefab;
    public GameObject[] spawnableObjects;

    [Header("Unity Stuff Internals")]
    public DirectionalSprite body;

    [HideInInspector] public PlayerCamera playerCam;
    [HideInInspector] public PlayerWeapon playerWeapon;
    [HideInInspector] public CharacterController character;

    private AudioSource audioSource;
    private Hurtful hurtful;

    private NetworkTransform netTrans;

    private MyNetworkManager myNetworkManager;
    private ClientManager clientManager;
    private LevelManager levelManager;

    [HideInInspector] public UI_Main uIMain;
    private PlayerAbove playerAbove;

    public override void OnStartLocalPlayer() //just for the local client
    {
        //so the player doesnt see own body but still can see its shadow
        body.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;

        clientManager = FindObjectOfType<ClientManager>();
        uIMain = FindObjectOfType<UI_Main>();
        uIMain.player = this;

        controls = new Controls();

        controls.Game.Jump.performed += funny => Jump();

        controls.Game.Move.performed += ctx => move = ctx.ReadValue<Vector2>();
        controls.Game.Move.canceled += ctx => move = Vector2.zero;

        controls.Game.Pause.performed += funnyer => Pause(!paused);

        controls.Game.ChangeClass.performed += funnyiest => ChangeClass(Random.Range(0,3));

        controls.Game.React.performed += moreFUNNY => CmdReact((int)moreFUNNY.ReadValue<float>());

        controls.Enable();

        playerCam = Instantiate(cameraPrefab, transform.position + cameraOffset,
            transform.rotation, transform).GetComponent<PlayerCamera>();

        playerWeapon = playerCam.GetComponentInChildren<PlayerWeapon>(); //this is a solution

        CmdSetupPlayer(clientManager.playerName, clientManager.playerColour);

        OwnerSpawnPlayer();
        //get the server to spawn player aswell
        CmdSpawnPlayer();

        lastPos = transform.position;

        uIMain.UIUpdate(); //update the ui now that everything is set
    }

    private void Start() //for all to run
    {
        character = GetComponent<CharacterController>();
        audioSource = GetComponent<AudioSource>();
        netTrans = GetComponent<NetworkTransform>();
        hurtful = GetComponent<Hurtful>();
        playerAbove = GetComponentInChildren<PlayerAbove>();

        myNetworkManager = FindObjectOfType<MyNetworkManager>();
        levelManager = FindObjectOfType<LevelManager>();

        //clients need to run to sync up with gamers allready gameing
        playerAbove.topLight.intensity = killStreak * 1.5f;
    }

    void FixedUpdate()
    {
        if (isServer)
        {
            if (health > 0) //only gain special when alive / gameing
            {
                if (special < maxSpecial) //only gain special when not at max
                {
                    specialChargeTime += Time.fixedDeltaTime;

                    if (specialChargeTime > playerClass.specialChargeRate)
                    {
                        CmdAddSpecial(1);
                        specialChargeTime = 0;
                    }
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

        Movement();
        IsGrounded();

    }

    //all run
    private void LateUpdate()
    {
        //if (Mathf.Abs(Vector3.Distance(transform.position, lastPos)) > 0.1f)
        //{
        //   if (audioSource.isPlaying)
        //       return;

        //   audioSource.clip = playerClass.walkCycle;
        //    audioSource.Play();
        //}
        //For all clients to run so they can hear really great sounds like RUN,RUN run from the sanvwich

        float fallSpeed = (transform.position.y - lastPos.y);


        if (fallSpeed > 0.26f)
        {
            if (audioSource.isPlaying)
                return;

            audioSource.PlayOneShot(playerClass.jumpSound[Random.Range
                (0,playerClass.jumpSound.Length)]);
        }

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

        if (Mathf.Abs(velocity.x) < maxMoveVelocity && Mathf.Abs(velocity.z) < maxMoveVelocity)
            movement *= speed;
        else
        {
            x = 0;
            z = 0;
        }

        if (paused) //Temp
            movement = Vector3.zero;

        character.Move((movement + velocity) * Time.fixedDeltaTime); //apply movement to charhcter contoler

        velocity += transform.right * x + transform.forward * z;

        //isGrounded = (Vector3.Angle(Vector3.up, floorNormal) <= slopeLimit);

        velocity.x = Mathf.Lerp(velocity.x, 0, fricktion * Time.fixedDeltaTime);
        velocity.z = Mathf.Lerp(velocity.z, 0, fricktion * Time.fixedDeltaTime);

        //Character sliding of surfaces
        if (character.isGrounded)
        {
            velocity.x += (1f - floorNormal.y) * floorNormal.x * (1f - slideFriction);
            velocity.z += (1f - floorNormal.y) * floorNormal.z * (1f - slideFriction);
        }

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
        //Disable controls somehow without disabling pause control
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
        bool canStand = (Vector3.Angle(Vector3.up, floorNormal) <= character.slopeLimit);

        if (character.isGrounded == true && canStand)
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


    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        floorNormal = hit.normal;

        if (!isServer)
            return;

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
        //playerAbove.playerNameText.text = playerName;
        if(isLocalPlayer)
            uIMain.UIUpdate();
    }

    void OnColorChanged(Color32 _Old, Color32 _New) //fixed colours to 32 bits 0-255 int, while listening to Miitopia soundtrack 
    {
        //playerAbove.playerNameText.color = _New;
        if (isLocalPlayer)
            uIMain.UIUpdate();
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
            int damage = _Old - _New;

            if (isLocalPlayer)
                playerCam.Shake((float)damage / maxHealth);
            //Tween.LocalScale(body.transform, body.transform.localScale * hurtScale, hurtDuration,
                //0, hurtCurve);

            //hurtSoud
            audioSource.PlayOneShot(playerClass.hurtSound[Random.Range(0,
                playerClass.hurtSound.Length)]);

            for (int i = 0; i < damage && i < maxHealth; i += 5) //creats multiple blood pieces
            {
                GameObject blood = Instantiate(bloodObj, transform.position, transform.rotation, null);
                blood.GetComponentInChildren<Renderer>().material.color = Color.red;
                blood.GetComponent<Rigidbody>().AddForce(Random.insideUnitSphere * Random.Range(3, 9) + Vector3.up * 2, ForceMode.VelocityChange);
            }

            if (_Old > 0 && _New <= 0) //on death
            {
                PlayerAlive(false);
            }
        }

        if (_Old <= 0 && _New > 0) //on death
        {
            PlayerAlive(true);
        }
    }

    [Client]
    void PlayerAlive(bool alive) //I will run Mesh.PlayerAlive(false)
    {
        transform.parent = null; //incase on moveing platform   
        character.enabled = alive;
        playerAbove.gameObject.SetActive(alive);
        body.gameObject.SetActive(alive);
        body.transform.position = transform.position + -Vector3.up; //bug fix

        if (alive == true)
        {
            audioSource.PlayOneShot(playerClass.spawnSound[Random.Range(0,
        playerClass.spawnSound.Length)]);
        }
        else
        {
            audioSource.PlayOneShot(playerClass.deathSound[Random.Range(0,
        playerClass.deathSound.Length)]);

            GameObject corpse = Instantiate(corpsePrefab, body.transform.position, transform.rotation);
            corpse.GetComponent<Rigidbody>().velocity = (transform.position - lastPos) * 2;
        }

        if (isLocalPlayer)
        {
            playerCam.onDeath();
            GetComponentInChildren<PlayerWeapon>().EndSpecial();

            if (alive == true)
                OwnerSpawnPlayer();
        }
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
        if (netTrans != null)
        {
            Transform sPoint = levelManager.GetSpawnPoint();
            netTrans.ServerTeleport(sPoint.position, sPoint.rotation); //spawns player across the server at point
        }

        maxHealth = playerClass.maxHealth;
        maxSpecial = playerClass.maxSpecial;

        special = playerClass.spawnSpecial;
        health = playerClass.maxHealth;
    }

    [Client]
    public void OwnerSpawnPlayer()
    {
        playerCam.transform.localPosition = cameraOffset;

        //reset client values according to the players class
        maxHealth = playerClass.maxHealth;
        maxSpecial = playerClass.maxSpecial;

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
        slideFriction = playerClass.slideFriction;

        bloodPerDamage = playerClass.bloodPerDamage;
        bloodObj = playerClass.bloodObj;
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
        {
            parent = transform;
            pos += transform.position;
        }

        GameObject spawned = Instantiate(spawnableObjects[objID], pos, Quaternion.Euler(rot), parent);

        Hurtful hurt = spawned.GetComponentInChildren<Hurtful>();
        if (hurt != null)
        {
            hurt.owner = this;
            hurt.ownerID = netIdentity.netId;
        }

        if (serverOnly == false) //spawns on clients as well
            NetworkServer.Spawn(spawned);
    }

    [TargetRpc]
    public void TargetAddVelocity(NetworkConnection target, Vector3 vel) //TEMP apply to local player only
    {
        velocity += vel;
        velocity = Vector3.ClampMagnitude(velocity, maxVelocity); //no more infinit death demension
    }


    [Command(requiresAuthority = false)] //oh no hackers are gonna hack it ahhhHHHHhhhHHHHhH
    public void CmdAddSpecial(int amount) //can take away special as well Server will update owner special ONLY
    {
        AddSpecial(amount);
    }

    [Server]
    public void AddSpecial(int amount) //negative works too
    {
        special += amount;
        special = Mathf.Clamp(special, 0, maxSpecial); //TOOO SPECIAL am i right ladeys
        TargetUpdateSpecial(netIdentity.connectionToClient, special);
    }
    [Server]
    public void ServerAddSpecial(int amount) //Same as CmdAddSpecial except if called from server
    {
        if (special > maxSpecial) //TOOO SPECIAL am i right ladeys
            return;

        special += amount;

        if (special > maxSpecial)
            special = maxSpecial;

        TargetUpdateSpecial(netIdentity.connectionToClient, special);
    }

    [TargetRpc] //should only be called on owner
    public void TargetUpdateSpecial(NetworkConnection target, int newSpecial)
    {
        // This will appear on the opponent's client, not other players
        special = newSpecial;
        uIMain.UIUpdate();
    }

    [Server]
    public void Hurt(int damage, HurtType hurtType = HurtType.Death, string killer = "") //can be used to heal just do -damage
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
            levelManager.sendKillMsg(killer, playerName, hurtType);
            playerAbove.topLight.intensity = 0;
            score -= 1;

            //if (dropOnDeath == null)
                //return;

            //GameObject spawned = Instantiate(dropOnDeath, transform.position, transform.rotation, null);

            //NetworkServer.Spawn(spawned);
        }
    }

    [ClientRpc]
    public void ConfirmedHit(bool kill) //when the players succesfully hurts something
    {
        if (kill)
        {
            playerAbove.topLight.intensity = killStreak * 1.5f;
            audioSource.PlayOneShot(playerClass.killPlayerSound[Random.Range(0, playerClass.killPlayerSound.Length)]);
            //playerCam.focus = 
        }
        else
        {
            audioSource.PlayOneShot(playerClass.hurtPlayerSound[Random.Range(0, playerClass.hurtPlayerSound.Length)]);
        }

        if (!isLocalPlayer)
            return;

        if (playerWeapon.specialIsActive)
        {
            playerWeapon.EndSpecial();
            velocity = Vector3.zero;
        }
    }

    [Client]
    void ChangeClass(int classObjIndex)
    {
        playerWeapon.controls.Dispose();

        CmdChangeClass(classObjIndex);
    }

    [Command]
    void CmdChangeClass(int classObjIndex)
    {
        myNetworkManager.ChangePlayer(connectionToClient, myNetworkManager.spawnPrefabs[classObjIndex]);
        //Hurt(9999);
    }

    [Command]
    void CmdReact(int index)
    {
        RPCReact(index);
    }

    [ClientRpc]
    void RPCReact(int index)
    {
        playerAbove.StartReaction(index);
    }
}

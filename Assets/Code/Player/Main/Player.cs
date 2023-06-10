using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;

public class Player : PlayerBase
{
    [Header("Player Charteristics")]
    public PlayerStats playerStats;

    [HideInInspector] public int maxHealth = 100;
    [SyncVar(hook = nameof(OnHealthChanged))]
    public int health;

    public List<StatusEffect> statusEffects = new List<StatusEffect>();

    [HideInInspector] public int maxSpecial = 10;
    public int special;

    private float specialChargeTime = 0;


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

    [HideInInspector] public float specialChargeRate;

    [Header("Player Internals")]

    public Vector3 velocity;
    private float maxVelocity = 200;

    public float angleVelocity;

    [Header("Player Grounded")]
    [Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
    public bool Grounded = true;
    public bool canStand = true;
    [Tooltip("Useful for rough ground")]
    public float GroundedOffset = 0.85f;
    [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
    public float GroundedRadius = 0.4f;
    [Tooltip("What layers the character uses as ground")]
    public LayerMask GroundLayers;

    private float distanceToFeet;

    private float jumpDelay;
    [Tooltip("Time you have been falling")]
    public float fallTime = 0; //for counting seconds of falling downwards

    private float deadTime;

    [HideInInspector] public bool paused;

    [HideInInspector] public Vector2 move;
    [HideInInspector] public Vector3 lastPos;
    [HideInInspector] public float lastYRot;

    private Vector3 floorNormal = Vector3.up;

    [HideInInspector] public Controls controls;

    [Header("Refrences")]
    public GameObject cameraPrefab;
    public GameObject corpsePrefab;
    public GameObject[] spawnableObjects;

    public Material iceMat;

    [Header("Unity Stuff Internals")]
    public Renderer body;
    public Renderer equipmentRender;
    [HideInInspector] public PlayerAnimator playerAnimator;

    [HideInInspector] public PlayerCamera playerCam;
    [HideInInspector] public PlayerWeapon playerWeapon;
    [HideInInspector] public CharacterController character;

    private AudioSource audioSource;
    private Hurtful hurtful;

    private NetworkTransform netTrans;

    private MyNetworkManager myNetworkManager;
    private ClientManager clientManager;
    private LevelManager levelManager;

    private PlayerAbove playerAbove;

    public float spawnRandomRot = 0;
    public GameObject spawnOnDeath;

    public override void OnStartLocalPlayer() //just for the local client
    {
        playerAnimator = GetComponentInChildren<PlayerAnimator>();
        //so the player doesnt see own body but still can see its shadow
        body.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
        foreach(Renderer render in body.GetComponentsInChildren<Renderer>())
        {
            render.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
        }

        if (equipmentRender != null)
        {
            equipmentRender.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;

            foreach (Renderer render in equipmentRender.transform.GetComponentsInChildren<Renderer>())
            {

                render.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
            }
        }

        clientManager = FindObjectOfType<ClientManager>();
        clientManager.player = this;

        playerStats.CmdSetUpPlayer(clientManager.playerName, clientManager.playerColour);

        uIMain = FindObjectOfType<UI_Main>();
        uIMain.player = this;

        controls = new Controls();

        controls.Game.Jump.performed += funny => Jump();

        controls.Game.Move.performed += ctx => move = ctx.ReadValue<Vector2>();
        controls.Game.Move.canceled += ctx => move = Vector2.zero;

        controls.Game.Pause.performed += funnyer => Pause(!paused, 1);

        controls.Game.ChangeClass.performed += funnyiest => Pause(!paused, 2);

        controls.Game.ShowScore.performed += funnyiest => uIMain.DisplayScoreBoard();

        controls.Game.Spectate.performed += funnyiestestest => Spectate();

        controls.Game.React.performed += moreFUNNY => CmdReact((int)moreFUNNY.ReadValue<float>());

        controls.Enable();

        playerCam = Instantiate(cameraPrefab, transform.position + cameraOffset,
            transform.rotation, transform).GetComponent<PlayerCamera>();

        playerWeapon = playerCam.GetComponentInChildren<PlayerWeapon>(); //this is a solution

        OwnerSpawnPlayer();
        //get the server to spawn player aswell
        CmdSpawnPlayer();

        lastPos = transform.position;

        uIMain.UIUpdate(); //update the ui now that everything is set

        base.OnStartLocalPlayer();
    }

    private void Start() //for all to run
    {
        base.Start();
        //stats = GetComponent<PlayerStats>();
        character = GetComponent<CharacterController>();
        audioSource = GetComponent<AudioSource>();
        netTrans = GetComponent<NetworkTransform>();
        hurtful = GetComponent<Hurtful>();
        playerAbove = GetComponentInChildren<PlayerAbove>();
        playerAnimator = GetComponentInChildren<PlayerAnimator>();
        //body = playerAnimator.GetComponent<DirectionalSprite>();

        levelManager = FindObjectOfType<LevelManager>();
        myNetworkManager = FindObjectOfType<MyNetworkManager>();

        //clients need to run to sync up with gamers allready gameing
        playerAbove.topLight.intensity = playerStats.killStreak * 1.5f;
        distanceToFeet = character.bounds.extents.y - character.center.y;
    }

    void Update()
    {
        if (isServer)
        {
            if (health > 0 & specialChargeRate > 0) //only gain special when alive / gameing
            {
                if (special < maxSpecial) //only gain special when not at max
                {
                    specialChargeTime += Time.deltaTime;

                    if (specialChargeTime > specialChargeRate)
                    {
                        ServerAddSpecial(1);
                        specialChargeTime = 0;
                    }
                }
            }
        }

        Grounded = IsGrounded();

        if (Grounded == false && velocity.y < 0)
            fallTime += Time.deltaTime;

        else
            fallTime = 0;

    }

    void FixedUpdate()
    {

        if (!isLocalPlayer) //only the player runs whats next
            return;

        if (health <= 0)
        {
            IsDead();
            return;
        }

        Movement();
        ApplyGravity();

        lastPos = transform.position;
    }

    //all run
    private void LateUpdate()
    {
        angleVelocity = transform.eulerAngles.y - lastYRot;

        if (!isLocalPlayer) //only the player runs whats next
        {
            velocity = transform.position - lastPos;
        }

        if (velocity.y > 0.26f)
        {
            if (audioSource.isPlaying)
                return;

            audioSource.PlayOneShot(playerClass.jumpSound[Random.Range
                (0,playerClass.jumpSound.Length)]);
        }

        //lastPos = transform.position; //must be done after movement and stuff
        lastYRot = transform.eulerAngles.y;
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
        float airMult = 1f;

        if (jumpDelay > coyotTime)
        {
            movement = movement * airMovementMultiplyer;
            airMult = airMovementMultiplyer;
        }

        movement *= speed;
        Vector3 localVelocity = transform.InverseTransformDirection(velocity);

        if (Mathf.Abs(localVelocity.x) > maxMoveVelocity * sideSpeedMultiplyer * airMult)
            movement *= 0;

        else if (localVelocity.z > maxMoveVelocity * airMult)
            movement *= 0;

        else if (localVelocity.z < -maxMoveVelocity * backSpeedMultiplyer * airMult)
            movement *= 0;


        if (paused) //Temp
            movement = Vector3.zero;

        //Character sliding of surfaces
        if (canStand == false)
        {
            //movement.x += (1f - floorNormal.y) * floorNormal.x * (1f - slideFriction);
            //movement.z += (1f - floorNormal.y) * floorNormal.z * (1f - slideFriction);
        }

        character.Move((movement + velocity) * Time.fixedDeltaTime); //apply movement to charhcter contoler

        velocity += movement;

        //if(Grounded)
        velocity.x = Mathf.LerpUnclamped(velocity.x, 0, fricktion * Time.fixedDeltaTime);
        velocity.z = Mathf.LerpUnclamped(velocity.z, 0, fricktion * Time.fixedDeltaTime);
    }

    [ClientCallback]
    void Jump()
    {
        if (paused)
            return;

        if (jumpDelay < coyotTime)
        {
            jumpDelay = coyotTime;
            float jumpForce = Mathf.Sqrt(jumpHeight * -2.0f * gravitY * 1.8f);
            Vector3 jumpVelocity = floorNormal * jumpForce;
            velocity.x += jumpVelocity.x; //x and z are added for political reasons
            velocity.y = jumpForce; //y gets set
            velocity.z += jumpVelocity.z;
        }
    }

    public void Pause(bool pause, int menuType)
    {
        //Disable controls somehow without disabling pause control
        paused = pause;

        if (menuType == 1)
            uIMain.Pause(pause);

        else if (menuType == 2) //stupid ui code is 
        {
            if (pause)
                uIMain.DisplayClassDetails(0);
            else
                uIMain.Pause(false);
        }

        if (pause == true)
            Cursor.lockState = CursorLockMode.None;
        else
            Cursor.lockState = CursorLockMode.Locked;
    }


    bool IsGrounded()
    {
        // set sphere position, with offset
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
        bool isGrounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);

        if (isGrounded || character.isGrounded) //final check if the player can stand
        {
            isGrounded = true;
        }

        RaycastHit hit;
        if (Physics.SphereCast(transform.position, 0.5f, -transform.up, out hit, maxDistance: distanceToFeet * 2, GroundLayers, QueryTriggerInteraction.Ignore))
        {
            if (Mathf.Approximately(floorNormal.y, 1))
            {
                floorNormal = Vector3.up;
                canStand = true;
            }

            floorNormal = hit.normal;
            canStand = (Vector3.Angle(Vector3.up, floorNormal) <= character.slopeLimit);
        }

        return (isGrounded);
    }

    [ClientCallback]
    void ApplyGravity()
    {
        if (Grounded)
        {
            if (velocity.y < 0) //if falling
                velocity.y = 0; //if grounded then no need to fall

            if (jumpDelay > 0)
                jumpDelay -= Time.fixedDeltaTime * 3;
        }
        else
        {
            velocity.y += gravitY * Time.fixedDeltaTime; //two deltra time cause PHJYSCIS


            if (jumpDelay < coyotTime)
                jumpDelay += Time.fixedDeltaTime;

        }
    }


    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
/*        floorNormal = hit.normal;

        float standAngle = Vector3.Angle(Vector3.up, floorNormal);

        if (Mathf.Approximately(standAngle, 90))
        {
            floorNormal = Vector3.up;
            canStand = true;
        }
        else
        {
            //canStand = standAngle < character.slopeLimit;
        }*/

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

    void OnHealthChanged(int _Old, int _New)
    {
        if (isLocalPlayer)
            uIMain.UIUpdate();


        if (_Old > _New) //if damage taken
        {
            int damage = _Old - _New;

            playerAnimator.Hurt();

            if (isLocalPlayer)
                playerCam.Shake((float)damage / maxHealth);
            //Tween.LocalScale(body.transform, body.transform.localScale * hurtScale, hurtDuration,
                //0, hurtCurve);

            //hurtSoud
            audioSource.PlayOneShot(playerClass.hurtSound[Random.Range(0,
                playerClass.hurtSound.Length)]);

            for (int i = 0; i < damage && i < maxHealth; i += 5) //creats multiple blood pieces
            {
                GameObject blood = Instantiate(playerClass.bloodObj, transform.position, transform.rotation, null);
                blood.GetComponentInChildren<Renderer>().material.color = Color.red;
                blood.GetComponent<Rigidbody>().AddForce(Random.insideUnitSphere * Random.Range(3, 9) + Vector3.up * 2, ForceMode.VelocityChange);
            }

            if (_Old > 0 && _New <= 0) //on death
            {
                PlayerAlive(false);
            }
        }

        if (_Old <= 0 && _New > 0) //on alive
        {
            ApplyEffect(StatusEffect.EffectType.immunity, 3);
            PlayerAlive(true);
        }
    }

    [Client]
    void PlayerAlive(bool alive) //I will run Mesh.PlayerAlive(false)
    {
        transform.parent = null; //incase on moveing platform   
        playerAbove.gameObject.SetActive(alive);
        body.transform.position = transform.position + -Vector3.up; //bug fix
        character.enabled = alive;
        body.transform.parent.gameObject.SetActive(alive);

        if (alive == true)
        {
            audioSource.PlayOneShot(playerClass.spawnSound[Random.Range(0,
        playerClass.spawnSound.Length)]);
        }
        else
        {
            audioSource.PlayOneShot(playerClass.deathSound[Random.Range(0,
        playerClass.deathSound.Length)]);


            if (hasEffect(StatusEffect.EffectType.sneeze))
            {
                Transform frozenCorpse = Instantiate(body.transform.parent, transform.position, transform.rotation, null);

                frozenCorpse.GetChild(0).gameObject.SetActive(true);

                Destroy(frozenCorpse.GetComponent<PlayerAnimator>());
                Destroy(frozenCorpse.GetComponent<Animator>());

                frozenCorpse.GetComponentInChildren<Collider>().enabled = false;

                Renderer[] frozenCorpseRenders = frozenCorpse.GetComponentsInChildren<Renderer>();

                foreach (Renderer render in frozenCorpseRenders)
                {
                    render.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;

                    Material[] matArray = new Material[render.materials.Length];
                    for (int m = 0; m < render.materials.Length; m++)
                    {
                        matArray[m] = iceMat;

                        //render.materials[m].surface
                        //render.materials[m].color = iceColour;
                        //render.materials[m].SetFloat("Smoothness", 1);
                    }

                    render.materials= matArray;
                }
            }
            else
            {
                GameObject corpse = Instantiate(corpsePrefab, body.transform.position, transform.rotation);
                corpse.GetComponentInChildren<Rigidbody>().velocity = (transform.position - lastPos) * 4;
            }
        }

        if (isLocalPlayer)
        {
            playerCam.onDeath();

            uIMain.UIUpdate();
            uIMain.OnDeathUI(!alive);
            GetComponentInChildren<PlayerWeapon>().EndSpecial();

            if (alive == true)
                OwnerSpawnPlayer();
        }
    }

    [Command]
    public void CmdSpawnPlayer()
    {
        Transform sPoint = levelManager.GetSpawnPoint(); //THIS WILL ERROR IF NO SPAWN POINTS
        netTrans.RpcTeleport(sPoint.position); //spawns player across the server at point
        transform.position = sPoint.position;

        maxHealth = playerClass.maxHealth;
        maxSpecial = playerClass.maxSpecial;

        special = playerClass.spawnSpecial;
        health = playerClass.maxHealth;

        specialChargeRate = playerClass.specialChargeRate;

        playerStats.killStreak = 0;

        character.enabled = true;
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

        specialChargeRate = playerClass.specialChargeRate;

        velocity = Vector3.zero;
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

    [Command]
    public void CmdShootObject(int objID, Vector3 pos, Vector3 rot, bool serverOnly, bool addVelocity)
    {
        if (health <= 0) //dead players cant spawn objects
            return;

        Transform parent = null;

        GameObject spawned = Instantiate(spawnableObjects[objID], pos, Quaternion.Euler(rot), parent);

        Hurtful hurt = spawned.GetComponentInChildren<Hurtful>();
        if (hurt != null)
        {
            hurt.owner = this;
            hurt.ownerID = netIdentity.netId;
        }

        if (addVelocity)
        {
            Projectile projectile = spawned.GetComponentInChildren<Projectile>();

            if (projectile != null)
            {
                projectile.velocity += velocity;;
            }
        }


        if (serverOnly == false) //spawns on clients as well
            NetworkServer.Spawn(spawned);
    }

    [TargetRpc]
    public void TargetAddVelocity(NetworkConnection target, Vector3 vel) //TEMP apply to local player only
    {
        velocity += vel;
        velocity = Vector3.ClampMagnitude(velocity, maxVelocity); //no more infinit death demension

        Grounded = false; //reset the check
        jumpDelay = 3;
    }

    [TargetRpc]
    public void TargetSetVelocity(NetworkConnection target, Vector3 vel) //TEMP apply to local player only
    {
        velocity = vel;
        velocity = Vector3.ClampMagnitude(velocity, maxVelocity); //no more infinit death demension
    }


    [Command(requiresAuthority = false)] //oh no hackers are gonna hack it ahhhHHHHhhhHHHHhH
    public void CmdAddSpecial(int amount) //can take away special as well Server will update owner special ONLY
    {
        ServerAddSpecial(amount);
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
    public void addScore(int points)
    {
        playerStats.bonusScore += points;
        TargetUpdateUI(connectionToClient); //update the UI of this user
    }

    [TargetRpc]
    public void TargetUpdateUI(NetworkConnection target)
    {
        uIMain.UIUpdate();
    }

    [Server]
    public void Hurt(int damage, HurtType hurtType = HurtType.Death, string killer = "") //can be used to heal just do -damage
    {
        print(hurtType);

        //convert healing to special and added it
        if (playerClass.convertHealingToSpecial && damage < 0 && hurtType != HurtType.Gambling)
        {
            special -= (damage / 2);
            return;
        }

        if (health <= 0 || hasEffect(StatusEffect.EffectType.immunity))
            return;

        //prevents infiti health stacking
        if (health - damage > maxHealth)
            health = maxHealth;
        else
            health -= damage;


        if(health <= 0)
        {
            character.enabled = false;
            levelManager.sendKillMsg(killer, playerStats.userName, hurtType);
            playerAbove.topLight.intensity = 0;
            playerStats.deaths += 1;

            if (spawnOnDeath == null)
                return;

            Vector3 spawnRot = spawnOnDeath.transform.eulerAngles;

            spawnRot += new Vector3(Random.Range(-spawnRandomRot, spawnRandomRot),
                Random.Range(-spawnRandomRot, spawnRandomRot),
                Random.Range(-spawnRandomRot, spawnRandomRot));

            GameObject spawned = Instantiate(spawnOnDeath, transform.position, Quaternion.Euler(spawnRot), null);
/*            ItemPickUp item = spawned.GetComponent<ItemPickUp>();

            if (item != null)
            {
                item.respawn = false;
                SelfDestruct sD = item.gameObject.AddComponent<SelfDestruct>();
                sD.destoryOnServer = true;
                sD.destroyDelay = 7;
            }*/

            NetworkServer.Spawn(spawned);
        }
    }

    [Server]
    public void ServerApplyEffect(StatusEffect.EffectType effect, float duration = Mathf.Infinity, float magnitude = 1)
    {
        ClientApplyEffect(effect, duration, magnitude);
        ApplyEffect(effect, duration, magnitude);
    }

    [ClientRpc]
    public void ClientApplyEffect(StatusEffect.EffectType effect, float duration, float magnitude)
    {
        if(!isServer)
            ApplyEffect(effect, duration, magnitude);
    }

    public void ApplyEffect(StatusEffect.EffectType effect, float duration = Mathf.Infinity, float magnitude = 1)
    {
        StatusEffect sEffect = (gameObject.AddComponent(typeof(StatusEffect)) as StatusEffect);
        sEffect.effectType = effect;
        sEffect.duration = duration; //applys the stats and stuff
        sEffect.magnitude = magnitude;

        statusEffects.Add(sEffect);
    }

    [ClientRpc]
    public void ConfirmedHit(bool kill) //when the players succesfully hurts something
    {
        if (kill)
        {
            playerAbove.topLight.intensity = playerStats.killStreak * 1.5f;
            audioSource.PlayOneShot(playerClass.killPlayerSound[Random.Range(0, playerClass.killPlayerSound.Length)]);
            //playerCam.focus = 
        }
        else
        {
            if(Random.Range(0, 15) == 2)
                audioSource.PlayOneShot(playerClass.hurtPlayerSound[Random.Range(0, playerClass.hurtPlayerSound.Length)]);
        }

        if (!isLocalPlayer)
            return;
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

    bool hasEffect(StatusEffect.EffectType effectType)
    {

        foreach(StatusEffect effect in statusEffects)
        {
            if (effect.effectType == effectType)
                return (true);
        }

        return (false);
    }

    [ServerCallback]
    public void OnCrush()
    {
        Hurt(1000, HurtType.Squash);
    }

    public float DistanceFromGround()
    {
        RaycastHit hit = new RaycastHit();
        if (Physics.Raycast(transform.position, -Vector3.up,out hit, 100, layerMask: 17))
        {
            var distanceToGround = hit.distance;
            return (distanceToGround);
        }

        return (Mathf.Infinity);
    }

    private void OnDrawGizmosSelected()
    {
        Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
        Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

        if (Grounded) Gizmos.color = transparentGreen;
        else Gizmos.color = transparentRed;

        // when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
        Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z), GroundedRadius);
    }
}

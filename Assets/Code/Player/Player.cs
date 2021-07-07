using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;

public class Player : NetworkBehaviour
{
    [Header("Player Info")]
    public TextMeshPro playerNameText;
    public GameObject floatingInfo;

    [Header("Player Stats")]
    [SyncVar(hook = nameof(OnHealthChanged))]
    public int health = 100;

    [SyncVar(hook = nameof(OnNameChanged))]
    public string playerName;
    [SyncVar(hook = nameof(OnColorChanged))]
    public Color playerColor = Color.white;

    public Vector3 cameraOffset;

    [Header("Player Movement")]
    public float speed = 5;
    public float backSpeedMultiplyer = 0.65f;
    public float sideSpeedMultiplyer = 0.8f;
    public float airMovementMultiplyer = 0.6f;

    public float gravitY = -10f; //manual gravity for gameplay reasons
    public float fricktion = 5f;
    public float maxMoveVelocity = 6;

    private float fallTime; //for counting seconds of falling
    public float jumpHeight = 2f;
    public float coyotTime = 0.3f; //lol mesh has good idears NO WAY

    public float pushForce = 5f;


    [Header("Player Internals")]
    private Vector2 move;
    private Vector3 lastPos;
    private Vector3 velocity;

    private float deadTime;

    private Controls controls;

    [Header("Unity Stuff Internals")]
    public GameObject cameraObj;
    private CharacterController character;
    public Rigidbody corpseRB;
    private NetworkTransform netTrans;

    private LevelManager levelManager;
    public GameObject[] spawnableObjects;

    public override void OnStartLocalPlayer() //just for the local client
    {
        controls = new Controls();

        controls.Game.Jump.performed += funny => Jump();

        controls.Game.Move.performed += ctx => move = ctx.ReadValue<Vector2>();
        controls.Game.Move.canceled += ctx => move = Vector2.zero;

        controls.Enable();

        Instantiate(cameraObj, transform.position + cameraOffset, transform.rotation, transform);

        string name = "NoNameNed" + Random.Range(0, 99);
        Color color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));

        CmdSetupPlayer(name, color);

        lastPos = transform.position;
    }

    private void Start() //for all to run
    {
        character = GetComponent<CharacterController>();
        netTrans = GetComponent<NetworkTransform>();

        levelManager = FindObjectOfType<LevelManager>();
    }

    void FixedUpdate()
    {
        if (!isLocalPlayer) //only the player runs this
            return;

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


        velocity.x = Mathf.Clamp(velocity.x, -maxMoveVelocity, maxMoveVelocity);
        velocity.z = Mathf.Clamp(velocity.z, -maxMoveVelocity, maxMoveVelocity);

        velocity.x = Mathf.Lerp(velocity.x, 0, fricktion * Time.fixedDeltaTime);
        velocity.z = Mathf.Lerp(velocity.z, 0, fricktion * Time.fixedDeltaTime);

        movement *= speed;

        character.Move((movement + velocity) * Time.fixedDeltaTime); //apply movement to charhcter contoler

        velocity += transform.right * move.x + transform.forward * move.y;
    }

    [ClientCallback]
    void Jump()
    {
        if (fallTime < coyotTime)
        {
            fallTime = coyotTime;
            velocity.y = Mathf.Sqrt(jumpHeight * -2.0f * gravitY * 2); //physics reasons 
        }
    }

    [ClientCallback]
    void IsGrounded()
    {
        if (character.isGrounded == true)
        {
            if(velocity.y < 0) //if falling
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
        if (_Old > 0 && _New <= 0) //on death
        {
            PlayerAlive(false);
        }
        if (_Old <= 0 && _New > 0) //on death
        {
            PlayerAlive(true);
        }
    }

    void PlayerAlive(bool alive)
    {
        character.enabled = alive;
        floatingInfo.SetActive(alive);
        corpseRB.isKinematic = alive;

        if (alive == true)
        {
            corpseRB.transform.parent = transform;
            corpseRB.transform.eulerAngles = Vector3.zero;
            corpseRB.transform.localPosition = Vector3.zero;
        }
        else
            corpseRB.transform.parent = null;

        if (!isServer) //only the server runs this
            return;

        //need to sync rigidbody between clients   
        corpseRB.velocity = (transform.position - lastPos) * 10;
        corpseRB.GetComponent<Mirror.Experimental.NetworkRigidbody>().SyncToClients();
    }

    [Command]
    public void CmdSetupPlayer(string _name, Color _col)
    {
        // player info sent to server, then server updates sync vars which handles it on all clients
        playerName = _name;
        playerColor = _col;
    }

    

    [ClientCallback]
    void IsDead()
    {
        deadTime += Time.deltaTime;

        if(deadTime > levelManager.respawnDelay)
        {
            CmdSpawnPlayer();
            deadTime = 0;
        }
    }

    [Command]
    void CmdSpawnPlayer()
    {
        netTrans.ServerTeleport(levelManager.GetSpawnPoint());
        health = 100;
    }

    [Command]
    public void CmdSpawnObject(int objID, Vector3 pos, Vector3 rot)
    {
        GameObject spawned = Instantiate(spawnableObjects[objID], pos, Quaternion.Euler(rot));
        NetworkServer.Spawn(spawned);
    }

}

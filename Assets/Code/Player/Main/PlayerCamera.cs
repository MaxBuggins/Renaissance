using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pixelplacement;
using UnityEngine.Rendering;

public class PlayerCamera : MonoBehaviour
{
    [Header("Variables")]
    public float mouseLookSensitivty = 18;
    public float gamepadLookSensitivty = 50;

    private float xRotation = 0f;

    [Header("HeadBob")]
    public float bobSpeed = 1;
    public float bobAmount = 0.25f;
    private float bobTime = 0;

    [Header("Shake")]
    public float shakeDuration = 0.5f;
    public float magnatude = 0.3f;

    public float rotResetSpeed = 1;

    [Header("Internal Variables")]
    public Vector2 look;
    private Vector3 lastPos;
    [HideInInspector]public Vector3 currentOffset = Vector3.zero;
    [HideInInspector] public Transform focus;

    [Header("Unity Things")]
    public Player player;
    private Controls controls;
    private Camera cam;
    private SkyCam skyCam;



    void Start()
    {
        cam = GetComponent<Camera>();
        player = GetComponentInParent<Player>();

        controls = new Controls();

        controls.Game.Look.performed += ctx => look = ctx.ReadValue<Vector2>();
        controls.Game.Look.canceled += ctx => look = Vector2.zero;

        controls.Enable();
        Cursor.lockState = CursorLockMode.Locked;


        skyCam = FindObjectOfType<SkyCam>();

        if (skyCam != null)
            cam.clearFlags = CameraClearFlags.Nothing;

        //audioSource.Play();
    }

    void FixedUpdate()
    {
        float mouseX = look.x * mouseLookSensitivty * Time.fixedDeltaTime;
        float mouseY = look.y * mouseLookSensitivty * Time.fixedDeltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -85f, 85f);

        transform.localRotation = Quaternion.Euler(xRotation + currentOffset.x, currentOffset.y, currentOffset.z);

        if (player.health > 0)
        {
            player.transform.Rotate(Vector3.up * mouseX);

        }
        else if (focus != null)
        {
            transform.LookAt(focus);
        }
        

        lastPos = transform.position;


        if (Mathf.Abs(player.move.x) > 0.1f || Mathf.Abs(player.move.y) > 0.1f)
        {
            //Player is moving
            bobTime += Time.deltaTime * bobSpeed;
            transform.localPosition = new Vector3(transform.localPosition.x, player.cameraOffset.y + Mathf.Sin(bobTime) * bobAmount, transform.localPosition.z);
        }
        else
        {
            //Idle
            bobTime = 0;
            transform.localPosition = new Vector3(transform.localPosition.x, Mathf.Lerp(transform.localPosition.y, player.cameraOffset.y, Time.deltaTime * bobSpeed), transform.localPosition.z);
        }
    }

    private void Update()
    {
        if(Vector3.Distance(Vector3.zero, currentOffset) > 0.15f)
            currentOffset += (Vector3.zero - currentOffset).normalized * rotResetSpeed * Time.deltaTime;
    }


    public void Shake(float amount = 1)
    {
        //better than 2 if statements (but thats probs what the fuction does so000.
        amount = Mathf.Clamp(amount, 0.15f, 1.8f); //stop BIG SHAKEs (only at hungry hacks)

        if (player == null)
            return;

        Vector3 orignalPosition = player.cameraOffset;

        Tween.Shake(transform, orignalPosition, Vector3.one * magnatude * amount, shakeDuration, 0);
    }

    public void onDeath()
    {
        Player[] players = FindObjectsOfType<Player>();
        List<Transform> trans = new List<Transform>(); //rights

        foreach (Player _player in players)
        {
            if(_player != player)
            trans.Add(_player.transform);
        }

        focus = GetClosestPlayer(trans.ToArray());
    }


    Transform GetClosestPlayer(Transform[] players)
    {
        Transform bestTarget = null;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = transform.position;
        foreach (Transform potentialTarget in players)
        {
            Vector3 directionToTarget = potentialTarget.position - currentPosition;
            float dSqrToTarget = directionToTarget.sqrMagnitude;
            if (dSqrToTarget < closestDistanceSqr)
            {
                closestDistanceSqr = dSqrToTarget;
                bestTarget = potentialTarget;
            }
        }

        return bestTarget;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [Header("Variables")]
    public float mouseLookSensitivty = 25;
    public float gamepadLookSensitivty = 50;

    private float xRotation = 0f;

    [Header("HeadBob")]
    public float headBobHeight;
    public float headBobDuration;
    public AnimationCurve headBobCurve;
    private bool bobbing = true;

    [Header("Shake")]
    public float shakeDuration = 0.5f;
    public float magnatude = 0.3f;

    [Header("Internal Variables")]
    public Vector2 look;
    private Vector3 lastPos;

    [Header("Unity Things")]
    public Player player;
    private Controls controls;
    private Camera cam;

    private AudioSource audioSource;


    void Start()
    {
        cam = GetComponent<Camera>();
        audioSource = GetComponent<AudioSource>();
        player = GetComponentInParent<Player>();

        controls = new Controls();

        controls.Game.Look.performed += ctx => look = ctx.ReadValue<Vector2>();
        controls.Game.Look.canceled += ctx => look = Vector2.zero;

        controls.Enable();
        Cursor.lockState = CursorLockMode.Locked;

        //audioSource.Play();
    }

    void FixedUpdate()
    {
        float mouseX = look.x * mouseLookSensitivty * Time.fixedDeltaTime;
        float mouseY = look.y * mouseLookSensitivty * Time.fixedDeltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -85f, 85f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        if(player.health > 0)
            player.transform.Rotate(Vector3.up * mouseX);
        else
            transform.Rotate(Vector3.up * mouseX);

        lastPos = transform.position;
    }
}

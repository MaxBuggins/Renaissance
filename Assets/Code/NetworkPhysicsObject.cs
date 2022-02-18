using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkPhysicsObject : MonoBehaviour
{
    public bool effectParent = false;

    public Vector3 gravity;
    public Vector3 velocity;

    public bool disableObjectOnCollision = false;

    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        velocity += gravity * Time.fixedDeltaTime;

        if (effectParent)
            transform.parent.position = transform.position + (velocity) * Time.fixedDeltaTime;
        else
            transform.position = transform.position + (velocity) * Time.fixedDeltaTime;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer != 0 || collision.collider.isTrigger)
            return;

        if (collision.transform == transform.parent)
            return;

        rb.isKinematic = true;
        transform.parent = collision.transform;
        this.enabled = false;
        if(disableObjectOnCollision)
            gameObject.SetActive(false);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player : MonoBehaviour {

    public event System.Action ReachedEnd;

    public float moveSpeed = 7;
    public float smoothMoveTime = .1f;
    public float turnSpeed = 8;
    Vector3 velocity;

    float angle;
    float smoothInputMagnitude;
    float smoothMoveVelocity;

    Rigidbody rb;
    public bool disabled;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Guard.OnSpotted += Disable;
    }

	// Update is called once per frame
	void Update () {
        print(disabled);
        Vector3 inputDirection = Vector3.zero;
        if (!disabled)
        {
            inputDirection = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
        }
        float inputMagnitude = inputDirection.magnitude;
        smoothInputMagnitude = Mathf.SmoothDamp(smoothInputMagnitude, inputMagnitude, ref smoothMoveVelocity, smoothMoveTime);

        float targetAngle = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg;
        angle = Mathf.LerpAngle(angle, targetAngle, Time.deltaTime * turnSpeed * inputMagnitude);

        velocity = transform.forward * moveSpeed * smoothInputMagnitude;
	}

    void OnTriggerEnter(Collider col)
    {
        if(col.tag == "Finish")
        {
            Disable();
            if(ReachedEnd != null)
            {
                ReachedEnd();
            }
        }
    }

    void Disable()
    {
        disabled = true;
    }

    void FixedUpdate()
    {
        rb.MoveRotation(Quaternion.Euler(Vector3.up * angle));
        rb.MovePosition(rb.position + velocity * Time.deltaTime);
        
    }

    void OnDestroy()
    {
        Guard.OnSpotted -= Disable;
    }
}

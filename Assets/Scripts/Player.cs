using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static event System.Action reachedFinish;

    public float speed = 7; 
    public float smoothMoveTime = .1f;
    public float turnSpeed = 8;
    Vector3 velocity;

    float angle;
    float smoothInputMagnitude;
    float smoothMoveVelocity;
    bool disabled;

    Rigidbody rigidBodyComponent;

    void Start()
    {
        rigidBodyComponent = GetComponent<Rigidbody>();
        Guard.onGuardHasSpottedPlayer += Disable;
        Player.reachedFinish += Disable;
    }

    void Update()
    {
        Vector3 direction = Vector3.zero;

        if (!disabled)
            direction = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;

        smoothInputMagnitude = Mathf.SmoothDamp(smoothInputMagnitude, direction.magnitude, ref smoothMoveVelocity, smoothMoveTime);

        float targetAngle = 90 - Mathf.Atan2(direction.z, direction.x) * Mathf.Rad2Deg;
        angle = Mathf.LerpAngle(angle, targetAngle, Time.deltaTime * turnSpeed * direction.magnitude);
        velocity = transform.forward * speed * smoothInputMagnitude;
    }

    void FixedUpdate() {
        rigidBodyComponent.MoveRotation(Quaternion.Euler(Vector3.up * angle));
        rigidBodyComponent.MovePosition(transform.position + velocity * Time.deltaTime);
    }

    void OnTriggerEnter(Collider triggerCollider) {
        if (triggerCollider.name == "FinishPoint")
            if (reachedFinish != null)
                reachedFinish();
    }

    void Disable() {
        disabled = true;
    }

    void OnDestroy() {
        Guard.onGuardHasSpottedPlayer -= Disable;
        Player.reachedFinish -= Disable;
    }


}

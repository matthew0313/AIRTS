using System;
using System.Collections.Generic;
using TMPro.EditorUtilities;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Player : Entity
{
    protected override string GetEntityName() => "Player";

    [SerializeField] float moveSpeed = 5.0f;
    [SerializeField] float gravity = -9.81f;
    [SerializeField] Transform modelRotator;
    [SerializeField] float modelRotateRate = 0.5f;

    Camera mainCam;
    CharacterController controller;
    float velocityY = 0.0f;

    private void Awake()
    {
        mainCam = Camera.main;
        controller = GetComponent<CharacterController>();
    }
    public event Action<Entity, string> onMessageReceive;
    public override void ReceiveMessage(Entity speaker, string message)
    {
        base.ReceiveMessage(speaker, message);
        onMessageReceive?.Invoke(speaker, message);
    }
    new public void SendMessage(string message) => EntityManager.Instance.SendMessage(transform.position, this, message);
    float targetRotY = 0.0f;
    void Update()
    {
        Vector3 camForward = mainCam.transform.forward, camRight = mainCam.transform.right;
        camForward.y = 0.0f; camRight.y = 0.0f;
        camForward.Normalize(); camRight.Normalize();
        Vector3 move = camRight * InputManager.Instance.MoveInput().x + camForward * InputManager.Instance.MoveInput().z;
        if(move.magnitude > 0.1f) targetRotY = Mathf.Atan2(move.x, move.z) * Mathf.Rad2Deg + 90.0f;
        controller.Move(move * moveSpeed * Time.deltaTime);

        if (!controller.isGrounded && controller.velocity.y <= 0.1f) velocityY = -2.0f;
        else velocityY += gravity * Time.deltaTime;
        controller.Move(Vector3.up * velocityY * Time.deltaTime);

        modelRotator.rotation = Quaternion.Slerp(modelRotator.rotation, Quaternion.Euler(0, targetRotY, 0), modelRotateRate * Time.deltaTime);
    }
}
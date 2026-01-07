using System;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class PlayerCam : MonoBehaviour
{
    [SerializeField] Transform rotator, cam;
    [SerializeField] float sensitivity = 5.0f;
    [SerializeField] float rotXMin = 0.0f, rotXMax = 60.0f;
    private void Update()
    {
        if(InputManager.Instance.CamInput(out Vector2 rotateValue))
        {
            rotator.eulerAngles = new Vector3(Mathf.Clamp(rotator.eulerAngles.x - rotateValue.y * sensitivity, rotXMin, rotXMax), rotator.eulerAngles.y + rotateValue.x * sensitivity, 0.0f);
        }
    }
}
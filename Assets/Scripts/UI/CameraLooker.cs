using UnityEngine;
using Unity.InferenceEngine;
using System.Collections.Generic;
using Unity.InferenceEngine.Tokenization;
using TMPro;
using UnityEngine.EventSystems;

public class CameraLooker : MonoBehaviour
{
    Camera mainCam;
    private void OnEnable()
    {
        mainCam = Camera.main;
    }
    private void Update()
    {
        transform.forward = mainCam.transform.forward;
    }
}
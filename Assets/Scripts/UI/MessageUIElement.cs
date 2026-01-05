using UnityEngine;
using Unity.InferenceEngine;
using System.Collections.Generic;
using Unity.InferenceEngine.Tokenization;
using TMPro;

public class MessageUIElement : MonoBehaviour
{
    [SerializeField] TMP_Text messageText;
    public void Set(string message)
    {
        messageText.text = message;
    }
}
using UnityEngine;
using Unity.InferenceEngine;
using System.Collections.Generic;
using Unity.InferenceEngine.Tokenization;
using TMPro;
using UnityEngine.EventSystems;

public class NPCEntityStatusUI : MonoBehaviour
{
    [SerializeField] NPCEntity entity;
    [SerializeField] TMP_Text nameText;
    [SerializeField] GameObject thinking;
    private void Update()
    {
        nameText.text = entity.entityName;
        thinking.SetActive(entity.thinking);
    }
}
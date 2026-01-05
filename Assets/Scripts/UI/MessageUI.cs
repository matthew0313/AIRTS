using UnityEngine;
using Unity.InferenceEngine;
using System.Collections.Generic;
using Unity.InferenceEngine.Tokenization;
using TMPro;
using UnityEngine.EventSystems;

public class MessageUI : MonoBehaviour
{
    [SerializeField] MessageUIElement elementPrefab;
    [SerializeField] Transform elementAnchor;
    [SerializeField] int maxElements = 40;
    [SerializeField] TMP_InputField inputField;
    readonly List<MessageUIElement> elements = new();
    Player player;
    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        player.onMessageReceive += OnMessageReceive;
        inputField.onSubmit.AddListener(message =>
        {
            player.SendMessage(message);
            OnMessageReceive(player, message);
            inputField.text = string.Empty;
        });
    }
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Slash) && (EventSystem.current.currentSelectedGameObject == null || !EventSystem.current.currentSelectedGameObject.TryGetComponent<TMP_InputField>(out _)))
        {
            inputField.Select();
        }
    }

    private void OnMessageReceive(Entity speaker, string message)
    {
        MessageUIElement element;
        if (elements.Count >= maxElements)
        {
            element = elements[0];
            elements.RemoveAt(0);
        }
        else
        {
            element = Instantiate(elementPrefab, elementAnchor);
            element.gameObject.SetActive(true);
        }
        element.Set($"<b>{speaker.entityName}</b>: {message}");
    }
}
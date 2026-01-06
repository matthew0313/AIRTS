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
    [SerializeField] TTSMessage tts;
    readonly List<MessageUIElement> elements = new();
    Player player;
    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        player.onMessageReceive += OnMessageReceive;
        inputField.onSubmit.AddListener(message =>
        {
            if(message != string.Empty) player.SendMessage(message);
            OnMessageReceive(player, message);
            inputField.text = string.Empty;
            EventSystem.current.SetSelectedGameObject(null);
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
        if(speaker!=player) tts.PlayTTS(message);

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
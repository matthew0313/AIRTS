using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System;

public class AIRequest : MonoBehaviour
{
    public static AIRequest Instance { get; private set; }
    private void Awake()
    {
        Instance = this;
    }
    const string API_KEY = "";
    const string URL_FORMAT = "https://generativelanguage.googleapis.com/v1beta/models/{0}:generateContent?key={1}";
    [SerializeField] string model_name = "gemini-1.5-flash";

    public void GenerateContent(string prompt, Action<string> onSuccess, Action<string> onFailure)
                    => StartCoroutine(SendRequest(prompt, onSuccess, onFailure));

    public IEnumerator SendRequest(string prompt, Action<string> onSuccess, Action<string> onFailure)
    {
        if (string.IsNullOrEmpty(API_KEY) || string.IsNullOrEmpty(URL_FORMAT))
        {
            onFailure?.Invoke("API키 또는 요청 URL 없음");
            yield break;
        }

        string url = string.Format(URL_FORMAT, model_name, API_KEY);
        print(url);
        string systemInstruction = "";
        // JSON 형식이랑 여러 규칙 예시
        //string systemInstruction = @"
        // You must reply in strict JSON format with no markdown formatting.
        // Schema:
        // {
        //   ""dialogue"": ""Your response"",
        //   ""action"": ""ActionKeyword"", 
        //   ""target"": ""TargetName""
        // }
        // Valid Actions:
        // Valid Targets:
        // Keep dialogue natural and consistent with the action.
        // Keep your answers concise, using just one or two sentences.
        // ";
        string finalPrompt = $"{systemInstruction}\n\nMessage: {prompt}";

        // 이제 요청 보내고 받은 데이터 파싱 코드 여기
    }
}

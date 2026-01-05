using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using MEC;
using UnityEngine.Networking;
using System.Text;
using Newtonsoft.Json;
using JetBrains.Annotations;
public class GeminiAPI_Prompter : AIPrompter
{
    const string api_key = "AIzaSyCRVMVsxg_BIHOQYReHm2MXytk3BSs6rqU";
    const string request_url = "https://generativelanguage.googleapis.com/v1beta/models/gemini-3-pro-preview:generateContent?key=";
    public override void Prompt(string prompt, Action<string> onRespond) => Timing.RunCoroutine(Prompting(prompt, onRespond));
    IEnumerator<float> Prompting(string prompt, Action<string> onRespond)
    {
        using (UnityWebRequest request = new(request_url + api_key, "POST"))
        {
            var a = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new
                            {
                                text = prompt
                            }
                        }
                    }
                }
            };
            //string json = "{\"contents\":[{\"parts\":[{\"text\":\"" + prompt + "\"}]}]}";
            string json = JsonConvert.SerializeObject(a);
            request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(json));
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return Timing.WaitUntilDone(request.SendWebRequest());

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(request.error);
                Debug.LogError(request.downloadHandler.text);
                onRespond?.Invoke(null);
            }
            else
            {
                GeminiResponse response = JsonConvert.DeserializeObject<GeminiResponse>(request.downloadHandler.text);
                string text = response.candidates[0].content.parts[0].text;
                onRespond?.Invoke(text);
            }
        }
    }
}
[Serializable]
public class GeminiResponse
{
    public List<Candidate> candidates;
    [Serializable]
    public struct Candidate
    {
        public Content content;
    }

    [Serializable]
    public struct Content
    {
        public List<Part> parts;
    }

    [Serializable]
    public struct Part
    {
        public string text;
    }
}
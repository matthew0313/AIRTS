using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

public class TTSMessage : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;
    readonly List<AudioSource> sources = new();
    void Start()
    {
        if(audioSource==null)
            audioSource = gameObject.GetComponent<AudioSource>();
    }
    private void OnDisable()
    {
        foreach (var i in sources) i.Stop();
    }
    AudioSource GetSource()
    {
        foreach(var i in sources) if (!i.isPlaying) return i;
        AudioSource tmp = new GameObject("TTS").AddComponent<AudioSource>();
        sources.Add(tmp);
        return tmp;
    }
    public void PlayTTS(string message) => StartCoroutine(_PlayTTS(message));

    IEnumerator _PlayTTS(string message)
    {
        string url =
            "https://translate.google.com/translate_tts" +
            "?ie=UTF-8" +
            "&client=tw-ob" +
            "&tl=ko" +
            "&q=" + UnityWebRequest.EscapeURL(message);
        
        using (UnityWebRequest req =
            UnityWebRequestMultimedia.GetAudioClip(url, AudioType.MPEG))
        {
            yield return req.SendWebRequest();

            if (req.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("TTS Error: " + req.error);
                yield break;
            }

            var source = GetSource();
            source.clip = DownloadHandlerAudioClip.GetContent(req);
            source.Play();
        }
    }
}
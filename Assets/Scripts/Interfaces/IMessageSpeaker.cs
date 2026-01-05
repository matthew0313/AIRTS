using UnityEngine;
using Unity.InferenceEngine;
using System.Collections.Generic;
using Unity.InferenceEngine.Tokenization;

public interface IMessageSpeaker
{
    public string speakerName { get; }
}
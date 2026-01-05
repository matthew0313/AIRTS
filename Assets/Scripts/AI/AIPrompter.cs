using UnityEngine;
using Unity.InferenceEngine;
using System.Collections.Generic;
using Unity.InferenceEngine.Tokenization;
using System;

public abstract class AIPrompter
{
    public abstract void Prompt(string prompt, Action<string> onAnswer);
}
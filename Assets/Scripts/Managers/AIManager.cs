using UnityEngine;
using Unity.InferenceEngine;
using System.Collections.Generic;
using Unity.InferenceEngine.Tokenization;

public class AIManager : MonoBehaviour
{
    public static AIManager Instance { get; private set; }

    AIPrompter prompter;
    private void Awake()
    {
        Instance = this;
        prompter = new GeminiAPI_Prompter();
    }
    public void RequestAction(NonPlayerEntity entity, List<string> messageLog)
    {
        string prompt = $"You are an AI controlling {entity.entityName}.\n" +
            $"These are your available actions:\n";
        foreach(var action in entity.GetAvailableActions())
        {
            prompt += $"- {action.actionName}: {action.actionDesc}\n";
        }
        prompt += "These are your surroundings:\n";
        foreach(var i in EntityManager.Instance.entityList)
        {
            if (i == entity) continue;
            float distance = EntityManager.Instance.GetDistance(entity, i);
            prompt += $"- {entity.entityName}: {distance:F1} units away.\n";
        }
        prompt += "This is your message log:\n";
        foreach (var message in messageLog)
        {
            prompt += $"- {message}\n";
        }
        prompt += "Based on the above information, decide your next sequence of actions and provide them in the following JSON format:\n" +
            "{" +
            "   actions: {\n" +
            "       actionName : (actionName),\n" +
            "       variables : [\n" +
            "           {\n" +
            "               key : (variable1Name),\n" +
            "               value : (variable1Value)\n" +
            "           },\n" +
            "           {\n" +
            "               key : (variable2Name),\n" +
            "               value : (variable2Value),\n" +
            "           },\n" +
            "           ...\n" +
            "       ]\n" +
            "   },\n" +
            "   ...\n" +
            "}\n" + 
            "Do not include anything else in your answer.\n";
        prompter.Prompt(prompt, (answer) =>
        {

        });
    }
}
[System.Serializable]
public struct ActionList
{
    public List<ActionList> list;
}
[System.Serializable]
public struct ActionElement
{
    public string actionName;
}
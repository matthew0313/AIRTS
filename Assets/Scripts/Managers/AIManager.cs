using UnityEngine;
using Unity.InferenceEngine;
using System.Collections.Generic;
using Unity.InferenceEngine.Tokenization;
using Newtonsoft.Json;
using MEC;
using Unity.VisualScripting;

public class AIManager : MonoBehaviour
{
    public static AIManager Instance { get; private set; }

    AIPrompter prompter;
    private void Awake()
    {
        Instance = this;
        prompter = new GeminiAPI_Prompter();
    }
    [SerializeField] List<string> gameInfo;
    public IEnumerator<float> RequestAction(NPCEntity entity, List<string> messageLog)
    {
        string prompt = $"You are an AI controlling {entity.entityName}.\n";
        prompt += "These are extra things you should know about the game:\n";
        foreach(var i in gameInfo)
        {
            prompt += $"- {i}\n";
        }
        prompt += $"These are your available actions:\n";
        foreach(var action in entity.GetAvailableActions())
        {
            prompt += $"- {action.actionName}: {action.actionDesc}\n";
        }
        prompt += "These are your surroundings:\n";
        foreach(var i in EntityManager.Instance.entityList)
        {
            if (i == entity) continue;
            float distance = EntityManager.Instance.GetDistance(entity, i);
            prompt += $"- {i.entityName}: {distance:F1} units away.\n";
        }
        prompt += "This is your message log:\n";
        foreach (var message in messageLog)
        {
            prompt += $"- {message}\n";
        }
        prompt += "These are your previous actions:\n";
        foreach(var command in entity.prevCommands)
        {
            prompt += $"- {command.actionName} with variables: ";
            foreach (var variable in command.variables)
            {
                prompt += $"{variable.Key} = {variable.Value}, ";
            }
            prompt = prompt.TrimEnd(',', ' ');
            prompt += "\n";
        }
        prompt += "Based on the above information, decide your next sequence of actions and provide them in the following JSON format (leave 'actions' as empty array if you wish to do nothing):\n" +
            "{\n" +
            "   actions: [\n" +
            "       {\n" +
            "           actionName : (actionName),\n" +
            "           variables : {\n" +
            "               content: [\n" +
            "                   {\n" +
            "                       key : (variable1Name),\n" +
            "                       value : (variable1Value)\n" +
            "                   },\n" +
            "                   {\n" +
            "                       key : (variable2Name),\n" +
            "                       value : (variable2Value),\n" +
            "                   },\n" +
            "               ...\n" +
            "               ]\n" +
            "           }\n" +
            "       },\n" +
            "   ...\n" +
            "   ]\n" +
            "}\n" +
            "Do not include anything else in your answer, not even ```json.\n";
        string answer = null;
        Debug.Log(prompt);
        prompter.Prompt(prompt, text => answer = text);
        while (answer == null) yield return Timing.WaitForOneFrame;
        Debug.Log(answer);
        ActionList tmp = JsonUtility.FromJson<ActionList>(answer);
        foreach (var i in tmp.actions)
        {
            string log = i.actionName;
            foreach (var k in i.variables)
            {
                log += $"\n{k.Key}: {k.Value}";
            }
            Debug.Log(log);
        }
        entity.ExecuteCommands(tmp.actions);
    }
}
[System.Serializable]
public class ActionList
{
    public List<RTSActionCommand> actions = new();
}
using MEC;
using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class NPCEntity : Entity
{
    const int maxLogSize = 50;
    protected readonly List<string> messageLog = new();

    public bool thinking { get; private set; } = false;
    CoroutineHandle requestAction;
    public override void ReceiveMessage(Entity speaker, string message)
    {
        base.ReceiveMessage(speaker, message);
        CancelExecution();
        messageLog.Add($"{speaker.entityName}: {message}");
        if (messageLog.Count > maxLogSize) messageLog.RemoveAt(0);
        thinking = true;
        Timing.KillCoroutines(requestAction);
        requestAction = Timing.RunCoroutine(AIManager.Instance.RequestAction(this, messageLog).CancelWith(gameObject));
    }
    public virtual IEnumerable<RTSAction> GetAvailableActions()
    {
        yield return new()
        {
            actionName = "ClearMessageLog",
            actionDesc = "Clears the message log."
        };
        yield return new()
        {
            actionName = "WaitForSeconds",
            actionDesc = "Waits for a specified number of seconds. variables: duration"
        };
        yield return new()
        {
            actionName = "RequestInfo",
            actionDesc = "You can use this to request additional information. The information will be added into your message log. If you plan to use this action, use only this type of action and nothing else, as you will re-think with the informations provided. You can use this multiple times to request more than one information at once. Information types you can request are as following:\n" +
            "EntityPosition: returns the absolute XYZ position of an entity. This entity can be yourself. needed variables: entityName\n" +
            "variables: InfoType (e.g: 'EntityPosition') + needed variables for that type"
        };
    }
    CoroutineHandle executingCommands, waitForSeconds;
    public void ExecuteCommands(List<RTSActionCommand> commands)
    {
        thinking = false;
        executingCommands = Timing.RunCoroutine(ExecutingCommands(commands).CancelWith(gameObject));
    }
    protected virtual void CancelExecution()
    {
        Timing.KillCoroutines(waitForSeconds);
        Timing.KillCoroutines(executingCommands);
    }
    public readonly List<RTSActionCommand> prevCommands = new();
    IEnumerator<float> ExecutingCommands(List<RTSActionCommand> commands)
    {
        for(int i = 0; i < commands.Count; i++)
        {
            bool finished = false;
            Debug.Log($"{i}, {commands.Count}");
            Execute(commands[i], () => finished = true, i == commands.Count - 1);
            while (!finished) yield return Timing.WaitForOneFrame;
            prevCommands.Add(commands[i]);
        }
    }
    protected virtual void Execute(RTSActionCommand command, Action onFinish, bool last = false)
    {
        if (command.actionName == "ClearMessageLog")
        {
            messageLog.Clear();
            onFinish?.Invoke();
        }
        else if(command.actionName == "WaitForSeconds")
        {
            waitForSeconds = Timing.RunCoroutine(WaitForSeconds(float.Parse(command.variables["duration"]), onFinish));
        }
        else if(command.actionName == "RequestInfo")
        {
            string infoType = command.variables["InfoType"];
            if (infoType == "EntityPosition")
            {
                string entityName = command.variables["entityName"];
                Entity targetEntity = EntityManager.Instance.entityList.Find(item => item.entityName == entityName);
                if (targetEntity != null)
                {
                    Vector3 pos = targetEntity.transform.position;
                    messageLog.Add($"Info - Position of {entityName}: X={pos.x:F1}, Y={pos.y:F1}, Z={pos.z:F1}");
                }
                else
                {
                    messageLog.Add($"Info - Entity {entityName} not found.");
                }
                if (messageLog.Count > maxLogSize) messageLog.RemoveAt(0);
            }
            onFinish?.Invoke();
            Debug.Log(last);
            if (last)
            {
                thinking = true;
                Timing.KillCoroutines(requestAction);
                requestAction = Timing.RunCoroutine(AIManager.Instance.RequestAction(this, messageLog).CancelWith(gameObject));
            }
        }
    }
    IEnumerator<float> WaitForSeconds(float seconds, Action onFinish)
    {
        yield return Timing.WaitForSeconds(seconds);
        onFinish?.Invoke();
    }
}
public struct RTSAction
{
    public string actionName;
    public string actionDesc;
}
[System.Serializable]
public struct RTSActionCommand
{
    public string actionName;
    public SerializableDictionary<string, string> variables;
}
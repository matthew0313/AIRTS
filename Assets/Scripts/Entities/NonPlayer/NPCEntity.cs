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
    public abstract IEnumerable<RTSAction> GetAvailableActions();
    CoroutineHandle executingCommands;
    public void ExecuteCommands(List<RTSActionCommand> commands)
    {
        thinking = false;
        executingCommands = Timing.RunCoroutine(ExecutingCommands(commands).CancelWith(gameObject));
    }
    protected virtual void CancelExecution() { Timing.KillCoroutines(executingCommands); }
    IEnumerator<float> ExecutingCommands(List<RTSActionCommand> commands)
    {
        foreach (var command in commands)
        {
            bool finished = false;
            Execute(command, () => finished = true);
            while (!finished) yield return Timing.WaitForOneFrame;
        }
    }
    protected abstract void Execute(RTSActionCommand command, Action onFinish);
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
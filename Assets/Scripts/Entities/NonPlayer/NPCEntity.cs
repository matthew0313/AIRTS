using MEC;
using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class NPCEntity : Entity
{
    const int maxLogSize = 50;
    protected readonly List<string> messageLog = new();
    public override void ReceiveMessage(Entity speaker, string message)
    {
        base.ReceiveMessage(speaker, message);
        messageLog.Add($"{speaker.entityName}: {message}");
        if (messageLog.Count > maxLogSize) messageLog.RemoveAt(0);
        AIManager.Instance.RequestAction(this, messageLog);
    }
    public abstract IEnumerable<RTSAction> GetAvailableActions();
    CoroutineHandle executingCommands;
    public void ExecuteCommands(List<RTSActionCommand> commands)
    {
        CancelExecution();
        executingCommands = Timing.RunCoroutine(ExecutingCommands(commands).CancelWith(gameObject));
    }
    protected virtual void CancelExecution() { Timing.KillCoroutines(executingCommands); }
    IEnumerator<float> ExecutingCommands(List<RTSActionCommand> commands)
    {
        foreach (var command in commands)
        {
            bool finished = false;
            Execute(command, () => finished = true);
            while (!finished) yield return 0f;
        }
    }
    protected abstract void Execute(RTSActionCommand command, Action onFinish);
}
public struct RTSAction
{
    public string actionName;
    public string actionDesc;
}
public struct RTSActionCommand
{
    public string actionName;
    public SerializableDictionary<string, string> variables;
}
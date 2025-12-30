using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class NonPlayerEntity : Entity
{
    const int maxLogSize = 50;
    protected readonly List<string> messageLog = new();
    public virtual void ReceiveMessage(Entity speaker, string message)
    {
        messageLog.Add($"{speaker.entityName}: {message}");
        if (messageLog.Count > maxLogSize) messageLog.RemoveAt(0);
        AIManager.Instance.RequestAction(this, messageLog);
    }
    public abstract IEnumerable<RTSAction> GetAvailableActions();
    public void ExecuteCommands(List<RTSActionCommand> commands)
    {
        CancelExecution();
    }
    protected virtual void CancelExecution() { }
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
    public Dictionary<string, string> variables;
}
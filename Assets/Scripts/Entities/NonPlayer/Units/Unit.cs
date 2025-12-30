using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class Unit : NonPlayerEntity
{
    public override IEnumerable<RTSAction> GetAvailableActions()
    {
        yield return new()
        {
            actionName = "SendMessage",
            actionDesc = $"Sends a message to non-player entities within hearing range. variables: message"
        };
        yield return new()
        {
            actionName = "MoveToPosition",
            actionDesc = "Moves the unit to specified location. variables: posX, posY"
        };
        yield return new()
        {
            actionName = "MoveToEntity",
            actionDesc = "Moves the unit towards specific entity. Once the distance between the unit and the entity becomes smaller than minDistance, this action stops. variables: targetEntityName, minDistance"
        };
        yield return new()
        {
            actionName = "FollowEntity",
            actionDesc = "Keeps moving the unit towards specific entity, maintaining a distance of minDistance. variables: targetEntityName, minDistance"
        };
    }
    protected override void Execute(RTSActionCommand command, Action onFinish)
    {
        if (command.actionName == "SendMessage")
        {
            string message = command.variables["message"];
            EntityManager.Instance.SendMessage(transform.position, this, message);
            onFinish?.Invoke();
        }
        else if (command.actionName == "MoveToPosition")
        {

        }
        else if (command.actionName == "MoveToEntity")
        {

        }
        else if (command.actionName == "FollowEntity")
        {

        }
        else
        {
            Debug.LogError("Unknown action command: " + command.actionName);
            onFinish?.Invoke();
        }
    }
}
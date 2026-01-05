using MEC;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public abstract class Unit : NPCEntity
{
    protected NavMeshAgent agent;
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }
    public override IEnumerable<RTSAction> GetAvailableActions()
    {
        yield return new()
        {
            actionName = "SendMessage",
            actionDesc = $"Sends a message to entities within hearing range. variables: message"
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
        else if (command.actionName == "MoveToEntity")
        {
            moveToEntity = Timing.RunCoroutine(MoveToEntity(
                EntityManager.Instance.entityList.Find(item => item.entityName == command.variables["targetEntityName"]),
                float.Parse(command.variables["minDistance"]),
                onFinish));
        }
        else if (command.actionName == "FollowEntity")
        {
            followEntity = Timing.RunCoroutine(FollowEntity(
                EntityManager.Instance.entityList.Find(item => item.entityName == command.variables["targetEntityName"]),
                float.Parse(command.variables["minDistance"]),
                onFinish));
        }
        else
        {
            Debug.LogError("Unknown action command: " + command.actionName);
            onFinish?.Invoke();
        }
    }
    protected override void CancelExecution()
    {
        base.CancelExecution();
        agent.isStopped = true;
        Timing.KillCoroutines(moveToEntity);
        Timing.KillCoroutines(followEntity);
    }
    const float pathRefreshRate = 0.5f;
    CoroutineHandle moveToEntity, followEntity;
    IEnumerator<float> MoveToEntity(Entity targetEntity, float minDistance, Action onFinish)
    {
        agent.destination = targetEntity.transform.position;
        agent.stoppingDistance = minDistance - 0.1f;
        agent.isStopped = false;
        float counter = 0.0f;
        while(agent.remainingDistance > minDistance)
        {
            yield return Timing.WaitForOneFrame;
            counter += Timing.DeltaTime;
            if(counter >= pathRefreshRate)
            {
                agent.destination = targetEntity.transform.position;
                counter -= pathRefreshRate;
            }
        }
        agent.isStopped = true;
        onFinish?.Invoke();
    }
    IEnumerator<float> FollowEntity(Entity targetEntity, float minDistance, Action onFinish)
    {
        agent.stoppingDistance = minDistance;
        agent.isStopped = false;
        while (true)
        {
            agent.destination = targetEntity.transform.position;
            yield return Timing.WaitForSeconds(pathRefreshRate);
        }
    }
}
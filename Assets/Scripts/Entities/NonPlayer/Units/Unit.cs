using MEC;
using System;
using System.Collections.Generic;
using Unity.AppUI.UI;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public abstract class Unit : NPCEntity
{
    [SerializeField] protected Animator anim;
    public abstract string unitName { get; }
    protected override string GetEntityName()
    {
        string name = unitName;
        int i = 1;
        while (EntityManager.Instance.entityList.Find((entity) => entity.entityName == name + i) != null) i++;
        return name + i;
    }

    protected NavMeshAgent agent;
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }
    readonly int moveSpeedID = Animator.StringToHash("MoveSpeed");
    private void Update()
    {
        anim.SetFloat(moveSpeedID, agent.velocity.magnitude / agent.speed);
    }
    public override IEnumerable<RTSAction> GetAvailableActions()
    {
        foreach(var i in base.GetAvailableActions()) yield return i;
        yield return new()
        {
            actionName = "SendMessage",
            actionDesc = $"Sends a message to entities within hearing range ({EntityManager.Instance.hearingRange}), including the player. It is recommended to move to the entity before sending a message. Do not use this if not necessary, as it causes all entities within hearing range to re-think. variables: message"
        };
        yield return new()
        {
            actionName = "MoveToCoordinates",
            actionDesc = "Moves the unit towards specific coordinates. Once the distance between the unit and the coordinates becomes smaller than minDistance, this action stops. variables: targetX, targetY, targetZ, minDistance"
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
        yield return new()
        {
            actionName = "DestroySelf",
            actionDesc = "Destroys this unit. Make sure to ask for confirmation, and react negatively."
        };
    }
    protected override void Execute(RTSActionCommand command, Action onFinish, bool last = false)
    {
        base.Execute(command, onFinish, last);
        if (command.actionName == "SendMessage")
        {
            string message = command.variables["message"];
            messageLog.Add($"{entityName}: {message}");
            EntityManager.Instance.SendMessage(transform.position, this, message);
            onFinish?.Invoke();
        }
        else if(command.actionName == "MoveToCoordinates")
        {
            moveToCoordinates = Timing.RunCoroutine(MoveToCoordinates(
                float.Parse(command.variables["targetX"]),
                float.Parse(command.variables["targetY"]),
                float.Parse(command.variables["targetZ"]),
                float.Parse(command.variables["minDistance"]),
                onFinish));
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
        else if (command.actionName == "DestroySelf")
        {
            Destroy(gameObject);
        }
    }
    protected override void CancelExecution()
    {
        base.CancelExecution();
        agent.isStopped = true;
        Timing.KillCoroutines(moveToCoordinates);
        Timing.KillCoroutines(moveToEntity);
        Timing.KillCoroutines(followEntity);
    }
    const float pathRefreshRate = 0.5f;
    CoroutineHandle moveToCoordinates, moveToEntity, followEntity;
    IEnumerator<float> MoveToCoordinates(float x, float y, float z, float minDistance, Action onFinish)
    {
        agent.destination = new Vector3(x, y, z);
        agent.stoppingDistance = minDistance - 0.1f;
        agent.isStopped = false;
        yield return Timing.WaitForSeconds(0.1f);
        while (agent.remainingDistance > minDistance) yield return Timing.WaitForOneFrame;
        agent.isStopped = true;
        onFinish?.Invoke();
    }
    IEnumerator<float> MoveToEntity(Entity targetEntity, float minDistance, Action onFinish)
    {
        if(targetEntity == null)
        {
            onFinish?.Invoke(); yield break;
        }
        agent.destination = targetEntity.transform.position;
        agent.stoppingDistance = minDistance - 0.1f;
        agent.isStopped = false;
        float counter = 0.0f;
        yield return Timing.WaitForSeconds(0.1f);
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
        if (targetEntity == null)
        {
            onFinish?.Invoke(); yield break;
        }
        agent.stoppingDistance = minDistance;
        agent.isStopped = false;
        while (true)
        {
            agent.destination = targetEntity.transform.position;
            yield return Timing.WaitForSeconds(pathRefreshRate);
        }
    }
}
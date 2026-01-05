using UnityEngine;
using Unity.InferenceEngine;
using System.Collections.Generic;
using UnityEngine.AI;

public class EntityManager : MonoBehaviour
{
    public static EntityManager Instance { get; private set; }
    private void Awake()
    {
        Instance = this;
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }
    [field: SerializeField] public float hearingRange { get; private set; } = 10.0f;

    public readonly List<Entity> entityList = new();
    Player player;
    public void AddEntity(Entity entity)
    {
        if (entityList.Find(item => item.entityName == entity.entityName))
        {
            Debug.LogError("Tried to add entity with same name"); return;
        }
        entityList.Add(entity);
    }
    public void RemoveEntity(Entity entity)
    {
        if (!entityList.Contains(entity)) return;
        entityList.Remove(entity);
    }
    public void SendMessage(Vector3 position, Entity speaker, string message)
    {
        player.ReceiveMessage(speaker, message);
        foreach (Entity entity in entityList)
        {
            if (entity == speaker || entity == player) continue;
            if (Vector3.Distance(position, entity.transform.position) > hearingRange) continue;
            entity.ReceiveMessage(speaker, message);
        }
    }
    public float GetDistance(Entity entity1, Entity entity2)
    {
        NavMeshPath path = new();
        if (NavMesh.CalculatePath(entity1.transform.position, entity2.transform.position, NavMesh.AllAreas, path))
        {
            float distance = 0f;
            for (int i = 1; i < path.corners.Length; i++)
            {
                distance += Vector3.Distance(path.corners[i - 1], path.corners[i]);
            }
            return distance;
        }
        else
        {
            return float.MaxValue;
        }
    }
}
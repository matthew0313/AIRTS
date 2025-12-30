using UnityEngine;
using Unity.InferenceEngine;
using System.Collections.Generic;

public class EntityManager : MonoBehaviour
{
    public static EntityManager Instance { get; private set; }
    private void Awake()
    {
        Instance = this;
    }
    [field: SerializeField] public float hearingRange { get; private set; } = 10.0f;

    public readonly List<Entity> entityList = new();
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
        foreach (Entity entity in entityList)
        {
            if (entity == speaker || !(entity is NonPlayerEntity nonPlayerEntity)) continue;
            if (Vector3.Distance(position, entity.transform.position) > hearingRange) continue;
            nonPlayerEntity.ReceiveMessage(speaker, message);
        }
    }
    public float GetDistance(Entity entity1, Entity entity2) => Vector3.Distance(entity1.transform.position, entity2.transform.position);
}
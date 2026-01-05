using System.Collections.Generic;
using UnityEngine;

public abstract class Entity : MonoBehaviour, IMessageSpeaker
{
    public string entityName;
    public string speakerName => entityName;
    protected abstract string GetEntityName();
    public virtual void ReceiveMessage(Entity speaker, string message) { }
    private void OnEnable()
    {
        entityName = GetEntityName();
        EntityManager.Instance.AddEntity(this);
    }
    private void OnDisable()
    {
        EntityManager.Instance.RemoveEntity(this);
    }
}

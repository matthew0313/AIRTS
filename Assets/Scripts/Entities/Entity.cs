using System.Collections.Generic;
using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    public string entityName { get; private set; }
    protected abstract string GetEntityName();
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

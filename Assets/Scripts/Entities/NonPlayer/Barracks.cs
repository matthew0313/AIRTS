using MEC;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Barracks : NPCEntity
{
    [SerializeField] List<Unit> unitPrefabs;
    [SerializeField] Transform spawnPoint;
    protected override string GetEntityName()
    {
        string name = "Barracks";
        int i = 1;
        while (EntityManager.Instance.entityList.Find((entity) => entity.entityName == name + i) != null) i++;
        return name + i;
    }
    public override IEnumerable<RTSAction> GetAvailableActions()
    {
        foreach (var i in base.GetAvailableActions()) yield return i;
        string tmp = "Spawns a unit. Unit types you can spawn are: ";
        for(int i = 0; i < unitPrefabs.Count; i++)
        {
            tmp += unitPrefabs[i].unitName;
            if (i < unitPrefabs.Count - 1) tmp += ", ";
        }
        tmp += ". Do not use this action unless you are explicitly told to do so. variables: unitType";
        yield return new()
        {
            actionName = "SpawnUnit",
            actionDesc = tmp
        };
    }
    protected override void Execute(RTSActionCommand command, Action onFinish, bool last = false)
    {
        base.Execute(command, onFinish, last);
        if(command.actionName == "SpawnUnit")
        {
            string unitType = command.variables["unitType"];
            Unit prefab = unitPrefabs.Find(item => item.unitName == unitType);
            if (prefab != null)
            {
                Instantiate(prefab, spawnPoint.position, Quaternion.identity);
            }
            onFinish?.Invoke();
        }
    }
}
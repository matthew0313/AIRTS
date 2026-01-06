using MEC;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Barracks : NPCEntity
{
    [SerializeField] List<UnitSpawnInfo> spawnOptions;
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
        for(int i = 0; i < spawnOptions.Count; i++)
        {
            tmp += $"- {spawnOptions[i].unitPrefab.unitName}, cost: {spawnOptions[i].spawnCost}\n";
        }
        tmp += "Do not use this action unless you are explicitly told to do so. This action will be skipped if there is not enough money. variables: unitType";
        yield return new()
        {
            actionName = "SpawnUnit",
            actionDesc = tmp
        };
        yield return new()
        {
            actionName = "MessageLowMoney",
            actionDesc = $"Sends the message 'Not enough money' to entities within hearing range ({EntityManager.Instance.hearingRange}), including the player."
        };
    }
    protected override void Execute(RTSActionCommand command, Action onFinish, bool last = false)
    {
        base.Execute(command, onFinish, last);
        if(command.actionName == "SpawnUnit")
        {
            string unitType = command.variables["unitType"];
            UnitSpawnInfo spawnInfo = spawnOptions.Find(item => item.unitPrefab.unitName == unitType);
            if (spawnInfo.unitPrefab != null && GameManager.Instance.money >= spawnInfo.spawnCost)
            {
                GameManager.Instance.AddMoney(-spawnInfo.spawnCost);
                Instantiate(spawnInfo.unitPrefab, spawnPoint.position, Quaternion.identity);
            }
            onFinish?.Invoke();
        }
        else if(command.actionName == "MessageLowMoney")
        {
            EntityManager.Instance.SendMessage(transform.position, this, "Not enough money");
            onFinish?.Invoke();
        }
    }
    [System.Serializable]
    public struct UnitSpawnInfo
    {
        public Unit unitPrefab;
        public int spawnCost;
    }
}
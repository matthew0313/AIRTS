using MEC;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Scout : Unit
{
    protected override string GetEntityName()
    {
        string name = "Scout";
        int i = 1;
        while (EntityManager.Instance.entityList.Find((entity) => entity.entityName == name + i) != null) i++;
        return name + i;
    }
}
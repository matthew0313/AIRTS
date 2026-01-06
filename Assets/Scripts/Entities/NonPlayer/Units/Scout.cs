using MEC;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Scout : Unit
{
    public override string unitName => "Scout";
    protected override IEnumerable<RequestableInfo> GetRequestableInfoTypes()
    {
        foreach (var i in base.GetRequestableInfoTypes()) yield return i;
        yield return new()
        {
            infoType = "EntityPosition",
            description = "Gets the absolute XYZ position of a specific entity. variables: targetEntityName"
        };
    }
}
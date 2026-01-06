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
        yield return new()
        {
            infoType = "GetPath",
            description = "Gets the path from start point to end point, given in series of XYZ coordinates. variables: startX, startY, startZ, endX, endY, endZ"
        };
    }
    protected override void RequestInfo(string infoType, Dictionary<string, string> variables)
    {
        base.RequestInfo(infoType, variables);
        if (infoType == "EntityPosition")
        {
            string targetEntityName = variables["targetEntityName"];
            Entity targetEntity = EntityManager.Instance.entityList.Find(item => item.entityName == targetEntityName);
            if(targetEntity != null)
            {
                messageLog.Add($"Info - The position of entity '{targetEntityName}' is X:{targetEntity.transform.position.x}, Y:{targetEntity.transform.position.y}, Z:{targetEntity.transform.position.z}");
            }
        }
        else if(infoType == "GetPath")
        {
            Vector3 startPos = new(
                float.Parse(variables["startX"]),
                float.Parse(variables["startY"]),
                float.Parse(variables["startZ"]));
            Vector3 endPos = new(
                float.Parse(variables["endX"]),
                float.Parse(variables["endY"]),
                float.Parse(variables["endZ"]));
            NavMeshPath path = new();
            if (NavMesh.CalculatePath(startPos, endPos, NavMesh.AllAreas, path))
            {
                string pathString = "Info - Path coordinates: ";
                foreach (var corner in path.corners)
                {
                    pathString += $"X:{corner.x:F1}, Y:{corner.y:F1}, Z:{corner.z:F1} | ";
                }
                messageLog.Add(pathString);
            }
            else
            {
                messageLog.Add("Info - No valid path could be found between the specified points.");
            }
        }
    }
}
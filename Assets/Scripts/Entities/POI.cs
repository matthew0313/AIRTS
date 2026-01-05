using MEC;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class POI : Entity
{
    [SerializeField] string poiName;
    protected override string GetEntityName() => poiName;
}
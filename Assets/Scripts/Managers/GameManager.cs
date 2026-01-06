using UnityEngine;
using Unity.InferenceEngine;
using System.Collections.Generic;
using UnityEngine.AI;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    private void Awake()
    {
        Instance = this;
    }
    [field:SerializeField] public int money { get; private set; } = 0;
    public void AddMoney(int amount) => money += amount;
}
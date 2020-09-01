using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generator : MonoBehaviour
{
    [Header("Basic Variables")]
    public float generatorQuantity;
    public float startGenQuantity;
    public float baseIncome;
    public float startingCost;
    [HideInInspector] public float currentCost;
    [HideInInspector] public float currentIncome;
    public float costMultiplier;
    public float generatorRate;

    [Header("Ownership")]
    public float ownershipStep;
    public float ownershipBonus;

    [Header("Misc")]
    public string generatorName;
    public bool autoBuy;
    public bool maxBuy;
    public float autoclickUnlockCost;
}

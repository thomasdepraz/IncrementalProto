using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.Mathematics;

public class ExponentialManager : MonoBehaviour
{
    //Variables
    public double currency = 0;
    private double alltimeCurrency = 0;

    public float prestigeBonus;
    private float currentPrestigeBonus;

    public List<Generator> generators;
    public List<GeneratorUIManager> uiManagers;

    //Text
    public TextMeshProUGUI currencyText;
    public TextMeshProUGUI prestigeBoostText;


    //Buttons
    public Toggle maxBuyToggle;
    public Button prestigeButton;


    //Methods

    private void Start()
    {
        Debug.Log(math.floor(currency / math.pow(10, 12)));
        //initialyze generators currentcost
        for (int i = 0; i < generators.Count; i++)
        {
            generators[i].currentCost = generators[i].startingCost;
        }
    }

    private void FixedUpdate()
    {
        currencyText.text = "Currency : \n" + SciFormat(currency) + " λ";
        if(currentPrestigeBonus != 0)
        {
            //prestigeBoostText.text = "x" +  currentPrestigeBonus + "\nmultiplier";

        }

        PrestigeCalculation();
    }

    public void Prestige()
    {
        for (int i = 0; i < generators.Count; i++)
        {
            prestigeBonus += (0.01f * generators[i].generatorQuantity);
            generators[i].generatorQuantity = generators[i].startGenQuantity;
            generators[i].ownershipBonus = 1;
            generators[i].ownershipStep = 25;
            generators[i].currentCost = generators[i].startingCost;
            currency = 0;
        }
    }

    private float PrestigeCalculation()
    {
        currentPrestigeBonus = (float)math.floor(currency / math.pow(10, 12)) * 0.02f; //replace currency by all time currency
        return currentPrestigeBonus;
    }

    public string SciFormat(double num)
    {
        return num.ToString("0.00E+0");
    }

}

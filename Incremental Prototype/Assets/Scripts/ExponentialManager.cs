using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ExponentialManager : MonoBehaviour
{
    //Variables
    public float currency = 0;

    public float prestigeBonus;
    private float currentPrestigeBonus;

    public List<Generator> generators;
    public List<GeneratorUIManager> uiManagers;
    public int currentGeneratorIndex;

    //Text
    public TextMeshProUGUI currencyText;
    public TextMeshProUGUI prestigeBoostText;


    //Buttons
    public Toggle maxBuyToggle;
    public Button prestigeButton;


    //Methods

    private void Start()
    {
        //initialyze generators currentcost
        for(int i = 0; i < generators.Count; i++)
        {
            generators[i].currentCost = generators[i].startingCost;
        }
    }

    private void FixedUpdate()
    {
        currencyText.text = "Currency : \n" + System.Math.Round(currency, 2) + " λ";
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

    private void PrestigeCalculation()
    {
        currentPrestigeBonus = 0;
        for (int i = 0; i < generators.Count; i++)
        {
            currentPrestigeBonus += (0.01f * generators[i].generatorQuantity);
        }
        currentPrestigeBonus += 1;
    }

    public void NextGenerator()
    {
        if(currentGeneratorIndex + 1 < uiManagers.Count)
        {
            //disable current
            uiManagers[currentGeneratorIndex].mask.enabled = true;

            //enable next
            uiManagers[currentGeneratorIndex + 1].mask.enabled = false;
            currentGeneratorIndex++;
        }
    }

    public void PreviousGenerator()
    {
        if (currentGeneratorIndex >= 1)
        {
            //disable current
            uiManagers[currentGeneratorIndex].mask.enabled = true;

            //enable previous
            uiManagers[currentGeneratorIndex - 1].mask.enabled = false;
            currentGeneratorIndex--;
        }
    }

    public void Generate()
    {
        uiManagers[currentGeneratorIndex].Generate();
    }

    public void Upgrade()
    {
        uiManagers[currentGeneratorIndex].MultiBuy();
    }
}

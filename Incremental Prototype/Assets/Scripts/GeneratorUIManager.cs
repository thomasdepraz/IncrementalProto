using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GeneratorUIManager : MonoBehaviour
{
    public ExponentialManager manager;
    public Generator associatedGenerator;
    public Button generateButton;
    public Button upgradeButton;
    public Button unlockAutoclick;
    public Image fillBar;
    public Image levelFillBar;
    public Image autoClickUnlockBar;
    public Image costBar;
    public TextMeshProUGUI incomeText;
    public TextMeshProUGUI generatorCountText;
    public TextMeshProUGUI upgradeCostText;
    public ParticleSystem multiplierParticles;
    public Mask mask;
    public int index;
    public ParticleSystem onHitUpgrade;
    public ParticleSystem autoclickUnlockParticles;

    private string generatorString = " generator ";

    [HideInInspector] public bool canStartCoroutine = true;

    //Private variables
    private float multiBuyCost;

    private float multiBuyIndex = 1;
    private float maxBuyIndex;
    private float previousOwnershipStep = 0;

    private void Start()
    {
        previousOwnershipStep = 0;
    }

    private void FixedUpdate()
    {
        #region FillBar
        if (!canStartCoroutine)
        {
            if(associatedGenerator.generatorRate / associatedGenerator.ownershipBonus > 0.1)
            {
                FillBar();
            }
            else
            {
                fillBar.fillAmount = 1;
            }
        }

        autoClickUnlockBar.fillAmount = (float)manager.currency / associatedGenerator.autoclickUnlockCost;
        costBar.fillAmount = (float)manager.currency / multiBuyCost;
        #endregion

        #region CostCalculation
        //MultiBuy Calculation
        multiBuyCost  = associatedGenerator.startingCost * ((Mathf.Pow(associatedGenerator.costMultiplier, associatedGenerator.generatorQuantity) - Mathf.Pow(associatedGenerator.costMultiplier, associatedGenerator.generatorQuantity + multiBuyIndex)) / (1 - associatedGenerator.costMultiplier)) / associatedGenerator.costMultiplier;
       
        //MaxBuyCost Calculation
        if(associatedGenerator.maxBuy)
        {
            MaxBuy();
        }
        #endregion

        #region ButtonManagement
        //generate button management       
        if(!canStartCoroutine || associatedGenerator.generatorQuantity == 0)
        {
            generateButton.interactable = false;
        }

        else if(canStartCoroutine && associatedGenerator.generatorQuantity != 0)
        {
            generateButton.interactable = true;
        }
        

        //upgrade button management
        if(manager.currency >= multiBuyCost)
        {
            upgradeButton.interactable = true;
        }
        else
        {
            upgradeButton.interactable = false;
        }

        //max buy toggle
        if(manager.maxBuyToggle.isOn)
        {
            associatedGenerator.maxBuy = true;
        }
        else
        {
            associatedGenerator.maxBuy = false;
            multiBuyIndex = 1;
        }
        
        //unlockautoclick button
        if(manager.currency >= associatedGenerator.autoclickUnlockCost)
        {
            unlockAutoclick.interactable = true;
        }
        else
        {
            unlockAutoclick.interactable = false;
        }

        #endregion

        #region TextManagement
        //genString
        if (multiBuyIndex > 1)
        {
            generatorString = " generators ";
        }
        else
        {
            generatorString = " generator ";
        }

        //genCount
        generatorCountText.text = associatedGenerator.generatorQuantity + " " + associatedGenerator.generatorName + " generators | " + System.Math.Round(multiBuyCost, 2) + " λ " + "for " + multiBuyIndex +" "  + associatedGenerator.generatorName + generatorString ;

        //upgradeCost
        //upgradeCostText.text = "Cost : " + System.Math.Round(multiBuyCost, 2) + " λ" + "\nfor " + multiBuyIndex + generatorString + associatedGenerator.generatorName;

        //incomeText.text
        incomeText.text = associatedGenerator.baseIncome * associatedGenerator.generatorQuantity * manager.prestigeBonus + " λ";
        #endregion

        Ownership();
    }
    private void FillBar()
    {
        if(fillBar.fillAmount < 1)
        {
            fillBar.fillAmount += Time.fixedDeltaTime / associatedGenerator.generatorRate * associatedGenerator.ownershipBonus;
        }
    }

    public void Generate()
    {
        if(canStartCoroutine)
        {
            canStartCoroutine = false;
            StartCoroutine(GenerateCurrency());
        }
    }

    public void MultiBuy()
    {
        if (multiBuyCost <= manager.currency)
        {
            manager.currency -= multiBuyCost;
            associatedGenerator.generatorQuantity += multiBuyIndex;
            onHitUpgrade.Play();
        }
    }

    public void MaxBuy()
    {
        //maxBuy Calculation
        maxBuyIndex = Mathf.Floor(Mathf.Log((float)(Mathf.Pow(associatedGenerator.costMultiplier, associatedGenerator.generatorQuantity) - ((manager.currency * (1 - associatedGenerator.costMultiplier)) / associatedGenerator.startingCost)), associatedGenerator.costMultiplier) - associatedGenerator.generatorQuantity);

        if (maxBuyIndex > 0)
        {
            multiBuyIndex = maxBuyIndex;
        }
        else
        {
            multiBuyIndex = 1;
        }
    }

    public void UnlockAutoClick()
    {
        //autoclick activate
        associatedGenerator.autoBuy = true;
        manager.currency -= associatedGenerator.autoclickUnlockCost;


        //play particle + disable

        StartCoroutine(UnlockAutoclickParticle());
        
        

        // si pas en train de générer : générer
        if(canStartCoroutine)
        {
            Generate();
        }
    }

    
    private void Ownership()
    {
        levelFillBar.fillAmount = (associatedGenerator.generatorQuantity - previousOwnershipStep) / (associatedGenerator.ownershipStep - previousOwnershipStep);
        if (associatedGenerator.generatorQuantity >= associatedGenerator.ownershipStep)
        {
            previousOwnershipStep = associatedGenerator.ownershipStep;
            multiplierParticles.Play();
            associatedGenerator.ownershipBonus *= 2;
            associatedGenerator.ownershipStep *= 2;
            levelFillBar.fillAmount = 0;
        }
    }

    private IEnumerator GenerateCurrency()
    {
        yield return new WaitForSeconds(associatedGenerator.generatorRate / associatedGenerator.ownershipBonus);

        // calculate currency 
        associatedGenerator.currentIncome = associatedGenerator.baseIncome * (associatedGenerator.generatorQuantity);
        manager.currency += associatedGenerator.currentIncome * manager.prestigeBonus;


        fillBar.fillAmount = 0;//reset progressBar
        canStartCoroutine = true;

        if(associatedGenerator.autoBuy)//autoClick onGenerateButton
        {
            Generate();
        }
    }

    private IEnumerator UnlockAutoclickParticle()
    {
        //play particle
        autoclickUnlockParticles.Play();
        yield return new WaitUntil(() => autoclickUnlockParticles.isPlaying == false);
        
        //disable button
        unlockAutoclick.transform.parent.gameObject.SetActive(false);
    }


}

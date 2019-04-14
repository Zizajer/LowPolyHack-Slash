using Devdog.General;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FuzzyLogicArmor : MonoBehaviour {

    public AnimationCurve lowArmor;
    public AnimationCurve mediumArmor;
    public AnimationCurve highArmor;

    private float enterValue;

    private float[] values = new float[]
        {
         0, 0, 0
        };

    private void Start()
    {

    }

    public void EvaluateStatements()
    {
        enterValue = PlayerManager.instance.currentPlayer.inventoryPlayer.stats.Get("Default", "Armor").currentValue;
        values[0] = lowArmor.Evaluate(enterValue);
        values[1] = mediumArmor.Evaluate(enterValue);
        values[2] = highArmor.Evaluate(enterValue);
    }

    public int CheckStatements()
    {
        EvaluateStatements();
        if (values.Max() == values[0])
            return 5;
        else if (values.Max() == values[1])
            return 15;
        else
            return 25;
    }
}

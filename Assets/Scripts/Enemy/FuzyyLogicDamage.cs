using Devdog.General;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FuzyyLogicDamage : MonoBehaviour {

    public AnimationCurve lowDamage;
    public AnimationCurve mediumDamage;
    public AnimationCurve highDamage;

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
        enterValue = PlayerManager.instance.currentPlayer.inventoryPlayer.stats.Get("Default", "Damage").currentValue;
        values[0] = lowDamage.Evaluate(enterValue);
        values[1] = mediumDamage.Evaluate(enterValue);
        values[2] = highDamage.Evaluate(enterValue);
    }

    public int CheckStatements()
    {
        EvaluateStatements();
        if (values.Max() == values[0])
            return 10;
        else if (values.Max() == values[1])
            return 20;
        else
            return 50;
    }
}

using Devdog.General;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FuzzyLogicHealth : MonoBehaviour {

    public AnimationCurve lowHealth;
    public AnimationCurve mediumHealth;
    public AnimationCurve highHealth;

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
        enterValue = PlayerManager.instance.currentPlayer.inventoryPlayer.stats.Get("Default", "Health").currentValue;
        values[0] = highHealth.Evaluate(enterValue);
        values[1] = mediumHealth.Evaluate(enterValue);
        values[2] = lowHealth.Evaluate(enterValue);
    }

    public int CheckStatements()
    {
        EvaluateStatements();
        if (values.Max() == values[0])
            return 15;
        else if (values.Max() == values[1])
            return 10;
        else
            return 5;
    }
}

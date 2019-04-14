using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WayPointManager : MonoBehaviour {

    public AnimationCurve attackOperation;
    public AnimationCurve findItemOperation;
    public AnimationCurve callForHelpOperation;
    public AnimationCurve retreatOperation;

    private FuzzyLogicArmor fuzzylogicArmor;
    private FuzzyLogicHealth fuzzyLogicHealth;
    private FuzyyLogicDamage FuzyyLogicDamage;
    private Dictionary<GameObject, float> nearestAllies;

    private float[] posibilityChooseOperationValues = new float[]
        {
         0, 0, 0, 0
        };


    // Use this for initialization
    void Start () {
        fuzzyLogicHealth = GetComponent<FuzzyLogicHealth>();
        FuzyyLogicDamage = GetComponent<FuzyyLogicDamage>();
        fuzzylogicArmor = GetComponent<FuzzyLogicArmor>();
        nearestAllies = new Dictionary<GameObject, float>();
    }

    public string getOperation()
    {
        int healthStatus = fuzzyLogicHealth.CheckStatements();
        int armorStatus = fuzzylogicArmor.CheckStatements();
        int damageStatus = FuzyyLogicDamage.CheckStatements();

        int dangerValue = healthStatus + armorStatus + damageStatus;

        nearestAllies = checkForNearbyAlies(transform.position, this.tag, 50);

        if (dangerValue > 75)
            dangerValue -= nearestAllies.Count * 10;

        Debug.Log(dangerValue);

        posibilityChooseOperationValues[0] = attackOperation.Evaluate(dangerValue);
        posibilityChooseOperationValues[1] = findItemOperation.Evaluate(dangerValue);
        posibilityChooseOperationValues[2] = callForHelpOperation.Evaluate(dangerValue);
        posibilityChooseOperationValues[3] = retreatOperation.Evaluate(dangerValue);

        string operation = null;
        if (posibilityChooseOperationValues.Max() == posibilityChooseOperationValues[0])
            operation = "Attack";
        else if (posibilityChooseOperationValues.Max() == posibilityChooseOperationValues[1])
            operation = "FindItem";
        else if (posibilityChooseOperationValues.Max() == posibilityChooseOperationValues[2])
            operation = "CallForHelp";
        else
            operation = "Retreat";

        operation = "Retreat";

        Debug.Log(operation);

        return operation;
    }
	
    public Vector3 getRetreatPosition(Vector3 point, Vector3 pivot)
    {
        //TO-DO check if point is available to move there
        Vector3 angles = new Vector3(Random.Range(90, 270), 0, Random.Range(90, 270));
        Vector3 dir = point - pivot;
        dir = Quaternion.Euler(angles) * dir;
        point = dir + pivot;
        return point;
    }

    public Vector3 getMeetingPoint(Vector3 startPosition, Vector3 enemyPosition, string tagOfAllies, float maxDistanceToSearch)
    {
        Vector3 center = Vector3.zero;
        float count = 0;
        List<GameObject> aliesToNotify = new List<GameObject>();
        Dictionary<GameObject, float> nearestAllies = checkForNearbyAlies(startPosition, tagOfAllies, maxDistanceToSearch);
        
        foreach (GameObject ally in nearestAllies.Keys)
        {
            if (count < 3) {
                aliesToNotify.Add(ally);
                center += ally.transform.position;
                count++;
            }
        }
        count = 0;
        float originalDistance = Vector3.Distance(startPosition, enemyPosition);
        float averageDistance = 0;
        foreach (GameObject ally in aliesToNotify)
        {
            if (count < nearestAllies.Count)
            {
                averageDistance += Vector3.Distance(ally.transform.position, enemyPosition);
                count++;
            }

        }
        averageDistance = averageDistance / aliesToNotify.Count;
        Debug.Log(averageDistance - originalDistance);
        if (averageDistance - originalDistance > maxDistanceToSearch/4)
        {
            Vector3 positionToMeet = center / count;
            foreach (GameObject ally in nearestAllies.Keys)
            {
                ally.SendMessage("goToPosition", positionToMeet);
            }
            return positionToMeet;
        }
        else
        {
            foreach (GameObject ally in nearestAllies.Keys)
            {
                ally.SendMessage("attack");
            }
            return enemyPosition;
        }
    }

    private Dictionary<GameObject,float> checkForNearbyAlies(Vector3 position,string tag, float maxDistanceToSearch)
    {
        GameObject[] alies;
        alies = GameObject.FindGameObjectsWithTag(tag);
        GameObject closest = null;
        Dictionary<GameObject, float> nearestAllies = new Dictionary<GameObject, float>();
        foreach (GameObject ally in alies)
        {
            float curDistance = Vector3.Distance(ally.transform.position, position);
            if (curDistance < maxDistanceToSearch)
            {
                nearestAllies.Add(ally,curDistance);
            }
        }
        return nearestAllies;

    }

    public Vector3 findNearestTypeOfItem(Vector3 startPosition, string searchedTag, float maxDistanceToSearch)
    {
        GameObject[] items;
        items = GameObject.FindGameObjectsWithTag(searchedTag);
        float distance = maxDistanceToSearch;
        GameObject closestWeapon = null;
        Vector3 position = Vector3.zero;
        foreach (GameObject item in items)
        {
            Debug.Log(item);
            float curDistance = Vector3.Distance(item.transform.position, startPosition);
            Debug.Log(curDistance);
            if (curDistance < distance)
            {
                position = item.transform.position;
                distance = curDistance;
            }
        }
        Debug.Log("Pozycja" + position);
        return position;
    }
	
}

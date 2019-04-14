using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockUnit : MonoBehaviour
{
    public float minSpeed = 100.0f;         //movement speed of the flock
    public float turnSpeed = 20.0f;         //rotation speed of the flock
    public float randomFreq = 20.0f;

    public float randomForce = 20.0f;       //Force strength in the unit sphere
    public float toOriginForce = 20.0f;
    public float toOriginRange = 100.0f;

    public float gravity = 2.0f;            //Gravity of the flock

    public float avoidanceRadius = 400.0f;  //Minimum distance between flocks
    public float avoidanceForce = 20.0f;

    public float followVelocity = 4.0f;
    public float followRadius = 40.0f;      //Minimum Follow distance to the leader

    private Transform originPoint;               //Parent transform
    private Vector3 velocity;               //Velocity of the flock
    private Vector3 normalizedVelocity;
    private Vector3 randomPush;             //Random push value
    private Vector3 originPush;
    private Transform[] flockPositions;            //Flock objects in the group
    private FlockUnit[] otherFlocks;       //Unity Flocks in the group
    private Transform currentFlock;   //My transform

    void Start()
    {
        randomFreq = 1.0f / randomFreq;
        
        originPoint = transform.parent;
         
        currentFlock = transform;
        
        Component[] tenpFlocksArray = null;
        
        if (originPoint)
        {
            tenpFlocksArray = originPoint.GetComponentsInChildren<FlockUnit>();
        }
        
        flockPositions = new Transform[tenpFlocksArray.Length];
        otherFlocks = new FlockUnit[tenpFlocksArray.Length];

        for (int i = 0; i < tenpFlocksArray.Length; i++)
        {
            flockPositions[i] = tenpFlocksArray[i].transform;
            otherFlocks[i] = (FlockUnit)tenpFlocksArray[i];
        }
        
        transform.parent = null;
        
        StartCoroutine(UpdateRandom());
    }

    IEnumerator UpdateRandom()
    {
        while (true)
        {
            randomPush = Random.insideUnitSphere * randomForce;
            yield return new WaitForSeconds(randomFreq + Random.Range(-randomFreq / 3.0f, randomFreq / 3.0f));
        }
    }

    void Update()
    {
        //Internal variables
        float speed = velocity.magnitude;
        Vector3 averageVelocity = Vector3.zero;
        Vector3 averagePosition = Vector3.zero;
        float count = 0;
        float f = 0.0f;
        float lengthVelocityForce = 0.0f;
        Vector3 currentFlockPosition = currentFlock.position;
        Vector3 velocityForce;
        Vector3 average;
        Vector3 newVelocity;

        for (int i = 0; i < flockPositions.Length; i++)
        {
            Transform transform = flockPositions[i];
            if (transform != currentFlock)
            {
                Vector3 currentPosition = transform.position;
                averagePosition += currentPosition;
                count++;
                
                velocityForce = currentFlockPosition - currentPosition;
                lengthVelocityForce = velocityForce.magnitude;
                
                if (lengthVelocityForce < followRadius)
                {
                    if (lengthVelocityForce < avoidanceRadius)
                    {
                        f = 1.0f - (lengthVelocityForce / avoidanceRadius);

                        if (lengthVelocityForce > 0)
                            averageVelocity += (velocityForce / lengthVelocityForce) * f * avoidanceForce;
                    }
                    f = lengthVelocityForce / followRadius;
                    FlockUnit tempOtherFlock = otherFlocks[i];
                    averageVelocity += tempOtherFlock.normalizedVelocity * f * followVelocity;
                }
            }
        }

        if (count > 0)
        {
            averageVelocity /= count;
            average = (averagePosition / count) - currentFlockPosition;
        }
        else
        {
            average = Vector3.zero;
        }
        
        velocityForce = originPoint.position - currentFlockPosition;
        lengthVelocityForce = velocityForce.magnitude;
        f = lengthVelocityForce / toOriginRange;
        
        if (lengthVelocityForce > 0)
            originPush = (velocityForce / lengthVelocityForce) * f * toOriginForce;

        if (speed < minSpeed && speed > 0)
        {
            velocity = (velocity / speed) * minSpeed;
        }

        newVelocity = velocity;
        
        newVelocity -= newVelocity * Time.deltaTime;
        newVelocity += randomPush * Time.deltaTime;
        newVelocity += originPush * Time.deltaTime;
        newVelocity += averageVelocity * Time.deltaTime;
        newVelocity += average.normalized * gravity * Time.deltaTime;
        
        velocity = Vector3.RotateTowards(velocity, newVelocity, turnSpeed * Time.deltaTime, 100.00f);
        currentFlock.rotation = Quaternion.LookRotation(velocity);
        
        currentFlock.Translate(velocity * Time.deltaTime, Space.World);
        
        normalizedVelocity = velocity.normalized;
    }
}

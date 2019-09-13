using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Agent : MonoBehaviour {

    private static List<GameObject> agents = new List<GameObject>();
    private AnimationController animationController = null;
    private Queue<Vector3> path = new Queue<Vector3>();
    private Vector3 destination;
    private Vector3 agentToDestination;
    private Vector3 desiredForce;
    private Vector3 agentToNeighbor;
    private Vector3 neighborToAgent;
    private Vector3 repulsiveForceSum = new Vector3();
    private float maxSpeed = 1;
    private float sqrMaxSpeed = 1;
    private float maxSpeedMinSpeedFraction = 1 / minSpeed;
    private const float minSpeed = 1.7f;
    private const float sqrMinSpeed = minSpeed * minSpeed;
    private const int k = 1;
    private const int kObstacle = 2;
    private const float Q = 0.7f;
    private const float QObstacle = 4f;
    private const float agentToAgentForceFactor = k * Q * Q;
    private const float obstacleToAgentForceFactor = kObstacle * Q * QObstacle;
    private const float neighborhoodRange = 4.0f;
    private const float sqrNeighborhoodRange = neighborhoodRange * neighborhoodRange;
    private const float obstacleNeighborhoodRange = 12.0f;
    private const float sqrObstacleNeighborhoodRange = obstacleNeighborhoodRange * obstacleNeighborhoodRange;
    private const int destinationReachRange = 4;
    private const int sqrDestinationReachRange = destinationReachRange * destinationReachRange;
    
    void Start() {
        agents.Add(gameObject);
        animationController = GetComponent<AnimationController>();
        animationController.Walk();
    }

    void Update() {

        if(ReachedDestination()) {
            path.Enqueue(path.Dequeue());
            destination = path.Peek();
        }

        Vector3 desiredForce = ComputeDesiredForce();
        Vector3 velocity = desiredForce;
        Vector3 repulsiveForce = ComputeRepulsiveForce(velocity);
        velocity.x += repulsiveForce.x;
        velocity.z += repulsiveForce.z;
        Vector3 obstacleRepulsiveForce = ComputeObstacleRepulsiveForce(velocity);
        velocity.x += obstacleRepulsiveForce.x;
        velocity.z += obstacleRepulsiveForce.z;

        //DrawForce(desiredForce, Color.blue);
        //DrawForce(repulsiveForce, Color.red);
        //DrawForce(obstacleRepulsiveForce, Color.yellow);
        //DrawForce(velocity, Color.green);

        float sqrVelocityMagnitude = velocity.sqrMagnitude;
        if (sqrVelocityMagnitude > sqrMaxSpeed) {
            velocity.Normalize();
            velocity.x *= maxSpeed;
            velocity.z *= maxSpeed;
            animationController.SetSpeed(maxSpeedMinSpeedFraction);
        } else if (sqrVelocityMagnitude < sqrMinSpeed) {
            velocity.Normalize();
            velocity.x *= minSpeed;
            velocity.z *= minSpeed;
            animationController.SetSpeed(1);
        } else {
            animationController.SetSpeed(Mathf.Sqrt(sqrVelocityMagnitude) / minSpeed);
        }
        
        float deltaTime = Time.deltaTime;
        velocity.x *= deltaTime;
        velocity.z *= deltaTime;

        transform.rotation = Quaternion.LookRotation(velocity);
        transform.position += velocity;

    }

    public bool ReachedDestination() {
        agentToDestination.x = transform.position.x - destination.x;
        agentToDestination.z = transform.position.z - destination.z;
        return (agentToDestination).sqrMagnitude <= sqrDestinationReachRange; 
    }

    public void EnqueueDestination(GameObject destination) {
        path.Enqueue(destination.transform.position);
        this.destination = path.Peek();
    }

    public void SetMaxSpeed(float speed) {
        maxSpeed = speed;
        sqrMaxSpeed = speed * speed;
        maxSpeedMinSpeedFraction = maxSpeed / minSpeed;
    }

    public float GetMaxSpeed() {
        return maxSpeed;
    }

    private void DrawForce(Vector3 force, Color color) {
        Debug.DrawLine(transform.position, transform.position + force, color);
    }

    private Vector3 ComputeDesiredForce() {
  
        Vector3 agentPosition = transform.position;

        desiredForce.x = destination.x - agentPosition.x;
        desiredForce.z = destination.z - agentPosition.z;
        desiredForce.Normalize();
        desiredForce.x *= maxSpeed;
        desiredForce.z *= maxSpeed;

        return desiredForce;

    }

    private Vector3 ComputeRepulsiveForce(Vector3 velocity) {

        repulsiveForceSum.x = 0;
        repulsiveForceSum.z = 0;
        Vector3 repulsiveForce;
        Vector3 neighborPosition;
        Vector3 agentPosition;

        for (int i = 0, totalAgents = agents.Count; i < totalAgents; ++i) {

            GameObject neighbor = agents[i];
            if (neighbor == gameObject) { continue; }

            neighborPosition = neighbor.transform.position;
            agentPosition = transform.position;
           
            neighborToAgent.x = agentPosition.x - neighborPosition.x;
            neighborToAgent.z = agentPosition.z - neighborPosition.z;
            float sqrNeighborToAgentDistance = neighborToAgent.sqrMagnitude;

            if (sqrNeighborToAgentDistance < sqrNeighborhoodRange) {
                if (neighborToAgent.x == 0 && neighborToAgent.z == 0) {
                    neighborToAgent.x = neighborToAgent.z = 0.1f;
                    sqrNeighborToAgentDistance = neighborToAgent.sqrMagnitude;
                }
                repulsiveForce = (agentToAgentForceFactor / sqrNeighborToAgentDistance) * neighborToAgent;
                if (Vector3.Dot(velocity, repulsiveForce) <= -0.95) {
                    repulsiveForce = Quaternion.Euler(0, 10, 0) * repulsiveForce;
                }
                repulsiveForceSum.x += repulsiveForce.x;
                repulsiveForceSum.z += repulsiveForce.z;
            }

        }

        return repulsiveForceSum;

    }

    Vector3 ComputeObstacleRepulsiveForce(Vector3 velocity) {

        repulsiveForceSum.x = 0;
        repulsiveForceSum.z = 0;
        List<Obstacle> obstacles = Obstacle.GetObstacles();
        Vector3 closestPoint;
        Vector3 repulsiveForce;

        for (int i = 0, totalObstacles = obstacles.Count; i < totalObstacles; ++i) {

            closestPoint = obstacles[i].ComputeClosestPoint(transform.position);

            neighborToAgent.x = transform.position.x - closestPoint.x;
            neighborToAgent.z = transform.position.z - closestPoint.z;
            float sqrNeighborToAgentDistance = neighborToAgent.sqrMagnitude;
            if (sqrNeighborToAgentDistance <= sqrObstacleNeighborhoodRange) {

                repulsiveForce = (obstacleToAgentForceFactor / sqrNeighborToAgentDistance) * neighborToAgent;
                float angle = Vector3.Angle(velocity, repulsiveForce) / 2.0f;
                repulsiveForce = Quaternion.Euler(0, angle, 0) * repulsiveForce;
                repulsiveForceSum.x += repulsiveForce.x;
                repulsiveForceSum.z += repulsiveForce.z;

            }

        }

        return repulsiveForceSum;

    }

}

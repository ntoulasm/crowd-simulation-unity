using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour {

    public GameObject[] agentPrefabs;
    public float minSpeed;
    public float maxSpeed;

    private GameObject[] destinations;
    [SerializeField]
    private int minAgentsPerDestination;
    [SerializeField]
    private int maxAgentsPerDestination;

    void Start() {
        destinations = GameObject.FindGameObjectsWithTag("Destination");
        CreateAgents();
    }

    private void CreateAgents() {

        int[] invalidPoints = new int[4] { -1, -1, -1, -1 };

        for (int i = 0; i < destinations.Length; ++i) {

            int totalAgents = Random.Range(minAgentsPerDestination, maxAgentsPerDestination);

            while (totalAgents-- != 0) {

                GameObject agentPrefab = agentPrefabs[Random.Range(0, agentPrefabs.Length)];
                GameObject agent = Instantiate(agentPrefab, new Vector3(destinations[i].transform.position.x, 0.1f, destinations[i].transform.position.z), Quaternion.identity);
                Agent agentScript = agent.GetComponent<Agent>();
                Animator animator = agent.GetComponent<Animator>();

                invalidPoints[0] = i;
                int destinationIndex = ComputeDestinationIndex(invalidPoints);
                agentScript.EnqueueDestination(destinations[destinationIndex]);
                invalidPoints[1] = destinationIndex;
                destinationIndex = ComputeDestinationIndex(invalidPoints);
                agentScript.EnqueueDestination(destinations[destinationIndex]);
                invalidPoints[2] = destinationIndex;
                destinationIndex = ComputeDestinationIndex(invalidPoints);
                agentScript.EnqueueDestination(destinations[destinationIndex]);

                AnimationController animationController = agent.GetComponent<AnimationController>();

                float speed = Random.Range(minSpeed, maxSpeed);
                agentScript.SetMaxSpeed(speed);
                animationController.SetSpeed(speed / minSpeed);

                float offset = Random.Range(0f, 1f);
                animationController.SetOffset(offset);

            }

        }

    }

    private bool ArrayContains(int[] array, int value) {
        for (int i = 0, l = array.Length; i < l; ++i) {
            if (array[i] == value) {
                return true;
            }
        }
        return false;
    }


    private int ComputeDestinationIndex(int[] invalidPoints) {
        int destinationPointIndex;
        while (ArrayContains(invalidPoints, (destinationPointIndex = Random.Range(0, destinations.Length)))) ;
        return destinationPointIndex;
    }


}

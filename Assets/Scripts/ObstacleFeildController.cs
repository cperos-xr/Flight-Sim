using System;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleFeildController : MonoBehaviour
{
    public GameObject fieldShape; // GameObject that contains the Collider and Renderer
    private Collider fieldCollider;

    public List<ObstacleObject> obstacleObjects = new List<ObstacleObject>();

    void Start()
    {
        // Disable Renderer to make the fieldShape invisible at runtime
        Renderer fieldRenderer = fieldShape.GetComponent<Renderer>();
        if (fieldRenderer != null)
        {
            fieldRenderer.enabled = false;
        }

        // Get Collider for defining field area
        fieldCollider = fieldShape.GetComponent<Collider>();
        if (fieldCollider != null)
        {
            InitializeField();
        }
        else
        {
            Debug.LogWarning("Field Collider is not set. Obstacle field will not be initialized.");
        }
    }

    void InitializeField()
    {
        // Create a new GameObject to hold all instantiated obstacles
        GameObject obstaclesParent = new GameObject("ObstaclesParent");

        foreach (ObstacleObject obstacle in obstacleObjects)
        {
            for (int i = 0; i < obstacle.numberToSpawn; ++i)
            {
                // Generate random position within the collider bounds
                Vector3 randomPosition = RandomPositionWithinCollider(fieldCollider);

                // Instantiate the obstacle at the random position
                GameObject instantiatedObstacle = Instantiate(obstacle.obstaclePrefab, randomPosition, Quaternion.identity);

                // Set the instantiated obstacle's parent to the new GameObject
                instantiatedObstacle.transform.SetParent(obstaclesParent.transform, false);

                // If the obstacle prefab contains an ObstacleController, initialize it
                ObstacleController controller = instantiatedObstacle.GetComponent<ObstacleController>();
                if (controller != null)
                {
                    controller.InitializeObstacle(controller.obstacle);
                }
            }
        }
    }


    Vector3 RandomPositionWithinCollider(Collider collider)
    {
        Bounds bounds = collider.bounds;
        Vector3 min = bounds.min;
        Vector3 max = bounds.max;

        Vector3 randomPosition = new Vector3(
            UnityEngine.Random.Range(min.x, max.x),
            UnityEngine.Random.Range(min.y, max.y),
            UnityEngine.Random.Range(min.z, max.z)
        );

        return randomPosition;
    }
}

[Serializable]
public struct ObstacleObject
{
    public GameObject obstaclePrefab;
    public int numberToSpawn;
}

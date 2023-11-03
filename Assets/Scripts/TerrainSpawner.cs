using UnityEngine;

public class TerrainSpawner : MonoBehaviour
{
    public GameObject prefabToSpawn; // Assign your prefab in the inspector
    public int numberOfObjects; // Number of prefabs to spawn
    public Vector2 planeSize; // Size of the plane on which to spawn objects

    private void Start()
    {
        SpawnObjects();
    }

    private void SpawnObjects()
    {
        for (int i = 0; i < numberOfObjects; i++)
        {
            // Generate a random position within the plane
            Vector3 randomPosition = new Vector3(
                Random.Range(-planeSize.x / 2, planeSize.x / 2),
                0, // Assuming the plane is at y = 0
                Random.Range(-planeSize.y / 2, planeSize.y / 2)
            );

            // Convert local position to world position
            randomPosition = transform.TransformPoint(randomPosition);

            // Instantiate the prefab at the random position
            Instantiate(prefabToSpawn, randomPosition, Quaternion.identity, transform);
        }
    }
}

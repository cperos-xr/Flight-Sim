using System.Collections;
using System.Collections.Generic;
using UnityEditor.Timeline.Actions;
using UnityEngine;

public class ObstacleController : MonoBehaviour
{
    public SO_Obstacle obstacle;
    public Rigidbody rb;
    public enum Shape { Cuboid, Sphere }
    public Shape obstacleShape;

    public bool randomizeRotation;

    public Vector2 rotationSpeedMinMax;


    public void InitializeObstacle(SO_Obstacle obstacle)
    {
        Vector3 obstacleScale = ObstacleScaleSelector(obstacle.minObstacleScale, obstacle.maxObstacleScale);
        transform.localScale = obstacleScale;

        if (obstacleShape == Shape.Cuboid)
        {
            rb.mass = ObstacleMassCuboid(obstacle.density, obstacleScale);
        }
        else if (obstacleShape == Shape.Sphere)
        {
            rb.mass = ObstacleMassSphere(obstacle.density, obstacleScale);
        }

        rb.angularDrag = obstacle.angularDrag;
        rb.drag = obstacle.drag;

        if (randomizeRotation)
        {
            ObstacleRotation();
        }

        ApplyRotationSpeed();
    }

    private void ApplyRotationSpeed()
    {
        float randomSpeed = UnityEngine.Random.Range(rotationSpeedMinMax.x, rotationSpeedMinMax.y);
        float speedInRadians = randomSpeed * Mathf.Deg2Rad;

        Vector3 randomAxis = UnityEngine.Random.onUnitSphere;
        rb.angularVelocity = randomAxis * speedInRadians;
    }

    private void ObstacleRotation()
    {
        Vector2 minMaxRotation = new Vector2(0f, 360f);  // Define your min-max rotation angles here

        float randomX = UnityEngine.Random.Range(minMaxRotation.x, minMaxRotation.y);
        float randomY = UnityEngine.Random.Range(minMaxRotation.x, minMaxRotation.y);
        float randomZ = UnityEngine.Random.Range(minMaxRotation.x, minMaxRotation.y);

        Quaternion randomRotation = Quaternion.Euler(randomX, randomY, randomZ);
        rb.MoveRotation(randomRotation);
    }


    public Vector3 ObstacleScaleSelector(Vector3 minSize, Vector3 maxSize)
    {
        float x = Random.Range(minSize.x, maxSize.x);
        float y = Random.Range(minSize.y, maxSize.y);
        float z = Random.Range(minSize.z, maxSize.z);

        return new Vector3(x, y, z);
    }

    public float ObstacleMassCuboid(float density, Vector3 scale)
    {
        float volume = scale.x * scale.y * scale.z;
        float mass = density * volume;

        return mass;
    }

    public float ObstacleMassSphere(float density, Vector3 scale)
    {
        // Compute the volume assuming the object is a sphere
        // Volume = 4/3 * pi * r^3, where r = average radius based on the scale
        float avgRadius = (scale.x + scale.y + scale.z) / 3.0f;
        float volume = (4.0f / 3.0f) * Mathf.PI * Mathf.Pow(avgRadius, 3);

        // Compute mass
        float mass = density * volume;

        return mass;
    }
}

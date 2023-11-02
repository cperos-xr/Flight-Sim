using UnityEngine;

[CreateAssetMenu(fileName = "ObstacleConfig", menuName = "Config/ObstacleConfig")]

public class SO_Obstacle : ScriptableObject
{
    public Vector3 minObstacleScale;
    public Vector3 maxObstacleScale;

    public float density; // mass based on size
    public float drag;
    public float angularDrag;





}

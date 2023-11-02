using UnityEngine;
[CreateAssetMenu(fileName = "ShipConfig", menuName = "Config/ShipConfig")]
public class SO_ShipConfiguration : ScriptableObject
{
    public float maxThrust;
    public float throttleIncrement;
    public float responsiveness;
    public float mass;
    public float drag;
    public float angularDrag;
    public float lift;

}
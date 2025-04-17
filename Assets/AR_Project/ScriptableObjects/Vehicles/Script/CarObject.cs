using UnityEngine;

[CreateAssetMenu(fileName = "CarObject", menuName = "Scriptable Objects/CarObject")]
public class CarObject : ScriptableObject
{
    public bool isAvailable;
    public int Price;
    public float Health;
    public float FuelCapacity;
    public float Speed;
    public float Handling;
}

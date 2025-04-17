using UnityEngine;

public class CashScript : MonoBehaviour
{
    [SerializeField] private int amount;
    public int GetAmount()
    {
        return amount;
    }
}

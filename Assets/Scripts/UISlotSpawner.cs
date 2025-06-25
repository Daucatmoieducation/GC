using UnityEngine;

public class SlotSpawner : MonoBehaviour
{
    public GameObject slotPrefab;
    public int count = 10;

    void Start()
    {
        for (int i = 0; i < count; i++)
        {
            Instantiate(slotPrefab, transform);
        }
    }
}
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance;

    public int turnCount = 0;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void AddTurn()
    {
        turnCount++;
        Debug.Log("Turn: " + turnCount);
    }

    public int GetTurn()
    {
        return turnCount;
    }
}
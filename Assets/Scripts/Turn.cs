using TMPro;
using UnityEngine;

public class TurnUIUpdater : MonoBehaviour
{
    public TextMeshProUGUI turnText;

    void Update()
    {
        int turn = TurnManager.Instance != null ? TurnManager.Instance.GetTurn() : 0;
        turnText.text = "" + turn;
    }
}

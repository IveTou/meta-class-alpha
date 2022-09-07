using UnityEngine;
using TMPro;

public class UILogger : MonoBehaviour
{
    public TextMeshProUGUI LogUI;
    private string incomingMessage = "Hello";

    void Update()
    {
        LogUI.text = incomingMessage.ToString();
    }

    public void SetMessage(string message)
    {
        incomingMessage = message;
    }
}

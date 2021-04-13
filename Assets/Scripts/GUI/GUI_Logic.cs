using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// GUI management
/// </summary>
public class GUI_Logic : MonoBehaviour
{
    [Header("Components")]
    public Text Health;

    public Text CurrentMag;

    public Text MagSize;

    public Text CurrentAmmo;

    public Text Reloading;

    public Transform MessageRoot;

    public GameObject Message;

    public Text Timer;

    public Text EndMessage;



    public void sendMessage(string text)
    {
        GameObject message = Instantiate(Message, MessageRoot);

        message.GetComponent<Message>().Display(text);
    }
}

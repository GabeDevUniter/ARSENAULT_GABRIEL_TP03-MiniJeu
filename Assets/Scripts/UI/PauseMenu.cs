using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [SerializeField]
    private Button btnQuit;

    private Menu menu;

    static private bool isPaused = false;

    static public bool IsPaused { get { return isPaused; } }

    void Awake()
    {
        isPaused = false;

        menu = GetComponent<Menu>();

        btnQuit.onClick.AddListener(onQuit);
    }

    void Update()
    {
        //if (Input.GetKey(KeyCode.P)) TogglePause();
    }

    public void Pause()
    {
        if (GameManager.singleton != null) GameManager.singleton.SetCursorMode(false);

        menu.Open();

        Time.timeScale = 0f;

        isPaused = true;
    }

    public void Unpause()
    {
        if(GameManager.singleton != null) GameManager.singleton.SetCursorMode(true);

        menu.Close();

        Time.timeScale = 1f;

        isPaused = false;
    }

    public void TogglePause()
    {
        if (isPaused) Unpause();
        else Pause();
    }

    void onQuit()
    {
        Unpause();

        GameManager.singleton.ChangeLevel("Main");
    }
}

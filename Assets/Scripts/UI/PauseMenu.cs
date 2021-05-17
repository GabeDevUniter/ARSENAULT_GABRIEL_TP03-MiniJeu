using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Special menu script for the pause menu
/// </summary>
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
        Unpause(); // Prevent the game from freezing when changing level

        GameManager.singleton.ChangeLevel("Main");
    }
}

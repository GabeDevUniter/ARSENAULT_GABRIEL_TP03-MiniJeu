using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Game Manager
/// </summary>
public class GameManager : MonoBehaviour
{
    #region Fields and properties

    //Singleton//

    public static GameManager singleton;

    //

    public bool inGame = true;

    public bool IsGameEnded { get; private set; } = false;

    public bool IsGameOver { get; private set; } = false;

    //Player//

    private GameObject Player;

    private Player _playerLogic;

    private PlayerMovement _playerMovement;

    public Vector3 PlayerHead { get { return _playerMovement.HeadPosition; } }

    public Player PlayerLogic { get { return _playerLogic; } }

    //

    //GUI//

    public GUI_Logic GUI;

    private PauseMenu pauseMenu;

    //

    //Timer//

    private Coroutine Timer;

    private int timer_elapsed = 1;

    //

    #endregion

    #region Unity Methods

    void Awake()
    {
        if (singleton != null) return;
        
        Player = GameObject.FindGameObjectWithTag("Player");

        if (Player != null)
        {
            _playerLogic = Player.GetComponent<Player>();

            _playerMovement = Player.GetComponent<PlayerMovement>();
        }

        SetCursorMode(inGame);
        

        if (inGame) Timer = StartCoroutine(StartTimer());

#if !UNITY_EDITOR && UNITY_WEBGL
        UnityEngine.WebGLInput.captureAllKeyboardInput = false;
#endif

        singleton = this;

        pauseMenu = FindObjectOfType<PauseMenu>();
        if(pauseMenu != null) pauseMenu.Unpause();
    }

    private void Update()
    {
        if (IsGameOver && Input.GetKeyDown(KeyCode.R))
        {
            ResetLevel();
        }
        else if (pauseMenu != null && Input.GetKeyDown(KeyCode.P)) pauseMenu.TogglePause();
    }

    #endregion

    #region Game Methods

    public void ChangeLevel(string name)
    {
        ResetStaticVariables();

        SceneManager.LoadScene(name);
    }

    public void ResetLevel()
    {
        ChangeLevel(SceneManager.GetActiveScene().name);
    }

    void ResetStaticVariables()
    {
        singleton = null;

        Grunt.GruntCount = 0;
    }

    public void GameOver(bool win)
    {
        StopTimer();

        Trigger.setTriggerGlobal(win ? TriggerCondition.LevelEnd : TriggerCondition.GameOver);

        GUI.Timer.color = win ? Color.green : Color.red;

        GUI.EndMessage.text = win ? "Tous les ennemis sont mort." : "Vous êtes mort.";

        IsGameEnded = win;
        IsGameOver = !win;
    }

    public void SetCursorMode(bool firstPerson)
    {
        Cursor.visible = !firstPerson;
        Cursor.lockState = firstPerson ? CursorLockMode.Locked : CursorLockMode.None;
    }

    #endregion

    #region Timer

    public IEnumerator StartTimer()
    {
        while (true)
        {
            GUI.Timer.text = TimerFormat();

            timer_elapsed++;

            yield return new WaitForSeconds(1f);
        }
    }

    public void StopTimer()
    {
        StopCoroutine(Timer);
    }

    /// <summary>
    /// The proper timer text format
    /// </summary>
    /// <returns>The timer format 0:00</returns>
    private string TimerFormat()
    {
        int minutes = 0;

        int seconds;

        for (seconds = timer_elapsed; seconds >= 60; seconds -= 60)
            minutes++;

        string extraZero = seconds >= 10 ? "" : "0";

        return $"{minutes}:{extraZero}{seconds}";
    }

    #endregion
}

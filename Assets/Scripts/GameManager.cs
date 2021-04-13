using System.Collections;
using UnityEngine;

/// <summary>
/// Game Manager
/// </summary>
public class GameManager : MonoBehaviour
{
    #region Fields and properties

    //Singleton//
    
    public static GameManager singleton;

    //

    //Player//

    private GameObject Player;

    private Player _playerLogic;

    private PlayerMovement _playerMovement;

    public Vector3 PlayerHead { get { return _playerMovement.HeadPosition; } }

    public Player PlayerLogic { get { return _playerLogic; } }

    //

    //GUI//

    public GUI_Logic GUI;

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

        Timer = StartCoroutine(StartTimer());

        singleton = this;
    }

    #endregion

    #region Game Methods

    public void GameOver(bool win)
    {
        StopTimer();

        GUI.Timer.color = win ? Color.green : Color.red;

        GUI.EndMessage.text = win ? "Tous les ennemis sont mort." : "Vous êtes mort.";
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

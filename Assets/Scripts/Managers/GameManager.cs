using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public int BestScore { get; set; }
    public int TimeMatch { get; private set; }
    public int TimeRespawn { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);

        SettingLoad();
    }

    /// <summary>
    /// Muda o stado do jogo
    /// </summary>
    /// <param name="state">Estado do jogo</param>
    public void ChangeGameState(GameController.GameStates state)
    {
        //Pega a estancia da cena e altera o estado do jogo
        GameController.Instance.ChangeGameState(state);
    }

    /// <summary>
    /// Muda a cena do jogo
    /// </summary>
    /// <param name="indexScene">numero que representa a cena</param>
    public void ChangeScene(int indexScene)
    {
        SceneManager.LoadScene(indexScene);
    }

    /// <summary>
    /// Recebe a pontuação atual e salva caso seja um record
    /// </summary>
    /// <param name="newScore">Nova pontuação</param>
    public void ChangeScore(int newScore)
    {
        if (newScore > BestScore)
        {
            BestScore = newScore;

            SettingSave();
        }
    }

    /// <summary>
    /// Altera o tempo da partida
    /// </summary>
    public void ChangeTimeMatch()
    {
        switch (TimeMatch)
        {
            case 60:
                TimeMatch = 120; //2 Min
                break;
            case 120:
                TimeMatch = 180; //3 Min
                break;
            case 180:
                TimeMatch = 60; //1 Min
                break;
        }

        SettingSave();
    }

    /// <summary>
    /// Altera o tempo do Spawn
    /// </summary>
    public void ChangeTimeRespawn()
    {
        switch (TimeRespawn)
        {
            case 2:
                TimeRespawn = 5; //2 seg
                break;
            case 5:
                TimeRespawn = 10; //5 seg
                break;
            case 10:
                TimeRespawn = 2; //10 seg
                break;
        }

        SettingSave();
    }

    /// <summary>
    /// Salva as configurações
    /// </summary>
    public void SettingSave()
    {
        PlayerPrefs.SetInt(Constants.Keys.BEST_SCORE, BestScore);
        PlayerPrefs.SetInt(Constants.Keys.SETTINGS_TIME_MATCH, TimeMatch);
        PlayerPrefs.SetInt(Constants.Keys.SETTINGS_TIME_RESPAWN, TimeRespawn);

        PlayerPrefs.Save();
    }

    /// <summary>
    /// Carrega as configurações
    /// </summary>
    public void SettingLoad()
    {
        BestScore = PlayerPrefs.GetInt(Constants.Keys.BEST_SCORE, 0);
        TimeMatch = PlayerPrefs.GetInt(Constants.Keys.SETTINGS_TIME_MATCH, 120);
        TimeRespawn = PlayerPrefs.GetInt(Constants.Keys.SETTINGS_TIME_RESPAWN, 5);
    }
}

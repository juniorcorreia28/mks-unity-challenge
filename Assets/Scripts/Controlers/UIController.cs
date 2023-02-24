using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [Header("UI ELEMENTS")]
    [SerializeField]
    private Text timer;
    [SerializeField]
    private Text timeMatch;
    [SerializeField]
    private Text timeRespawn;
    [SerializeField]
    private Text scoreOld;
    [SerializeField]
    private Text scoreNew;
    [SerializeField]
    private GameObject screenGameOver;
    [SerializeField]
    private GameObject gameOverFistSelected;
    [SerializeField]
    private CanvasGroup fadeScreen;

    private GameManager gameManager;

    private void Start()
    {
        gameManager = GameManager.Instance;

        if (timeMatch) timeMatch.text = (gameManager.TimeMatch / 60) + "min";
        if (timeRespawn) timeRespawn.text = gameManager.TimeRespawn + "seg";

        if(fadeScreen) StartCoroutine(FadeOutCoroutine());
    }

    private void Update()
    {
        if (timer)
        {
            float timeRemaining = GameController.Instance.Timer;
            float minutes = Mathf.FloorToInt(timeRemaining / 60);
            float seconds = Mathf.FloorToInt(timeRemaining % 60);

            timer.text = minutes + ":" + seconds;
        }

        //Abre a tela de gameOver
        if (screenGameOver && !screenGameOver.activeInHierarchy)
        {
            if (GameController.Instance.GameState == GameController.GameStates.GameOver)
            {
                scoreOld.text = gameManager.BestScore + " pts";
                scoreNew.text = GameController.Instance.Score + " pts";

                screenGameOver.SetActive(true);

                EventSystem.current.SetSelectedGameObject(gameOverFistSelected);
            }
        }
    }

    /// <summary>
    /// Pausa o jogo
    /// </summary>
    public void GamePaused()
    {
        gameManager.ChangeGameState(GameController.GameStates.Paused);
    }

    /// <summary>
    /// Despausa o jogo
    /// </summary>
    public void GameUnpaused()
    {
        gameManager.ChangeGameState(GameController.GameStates.Active);
    }

    /// <summary>
    /// Inicia o jogo
    /// </summary>
    public void GameStart()
    {
        gameManager.ChangeGameState(GameController.GameStates.GameStart);
    }

    /// <summary>
    /// Muda a cena do jogo
    /// </summary>
    /// <param name="indexScene">Valor que representa a nova cena</param>
    public void ChangeScene(int indexScene)
    {
        gameManager.ChangeScene(indexScene);
    }

    /// <summary>
    /// Altera o tempo da partida
    /// </summary>
    public void SettingsTimeMatch()
    {
        gameManager.ChangeTimeMatch();

        timeMatch.text = (gameManager.TimeMatch / 60) + "min";
    }

    /// <summary>
    /// Altera o tempo do Spawn
    /// </summary>
    public void SettingsTimeRespawn()
    {
        gameManager.ChangeTimeRespawn();

        timeRespawn.text = gameManager.TimeRespawn + "seg";
    }

    #region Coroutines
    /// <summary>
    /// Executa o efeito fade out na tela
    /// </summary>
    private IEnumerator FadeOutCoroutine()
    {
        fadeScreen.alpha = 1;

        while (fadeScreen.alpha > 0)
        {
            fadeScreen.alpha -= Time.deltaTime / 1;

            yield return null;
        }

        fadeScreen.alpha = 0;

        fadeScreen.gameObject.SetActive(false);

        GameController.Instance.ChangeGameState(GameController.GameStates.GameStart);

        yield return null;
    }
    #endregion
}

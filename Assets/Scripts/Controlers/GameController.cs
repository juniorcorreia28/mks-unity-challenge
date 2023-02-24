using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    [SerializeField]
    private GameObject respawnPositions;

    public static GameController Instance { get; private set; }
    public GameObject Player { get; private set; }
    public bool PlayersCanMove { get; private set; }
    public bool EnemiesCanMove { get; private set; }
    public bool SpawnNewEnemies { get; private set; }
    public int Score { get; set; }
    public float Timer { get; private set; }
    public GameStates GameState { get; private set; }

    private int lastTransformUsed;

    private float timerRespawn;

    private GameObject enemyGameObject;

    private Transform[] transforms;

    public enum GameStates
    {
        Active,
        Paused,
        GameStart,
        GameOver
    }

    private void Awake()
    {
        Instance = this;

        Player = GameObject.FindGameObjectWithTag(Constants.SystemTags.PLAYER);
        transforms = respawnPositions.GetComponentsInChildren<Transform>();
    }

    private void Update()
    {
        switch (GameState)
        {
            case GameStates.Active: //Modo padrão
                break;
            case GameStates.Paused:
                break;
            case GameStates.GameStart:
                if (Timer > 0)
                {
                    Timer -= Time.deltaTime;
                }
                else
                {
                    //Termina a partida quando o tempo acabar.
                    ChangeGameState(GameStates.GameOver);
                }

                if (timerRespawn > 0)
                {
                    timerRespawn -= Time.deltaTime;
                }
                else
                {
                    //Estancia um inimigo
                    enemyGameObject = ObjectPoolManager.Instance.GetPooledObject(Random.Range(5, 7));

                    if (enemyGameObject != null)
                    {
                        int random = Random.Range(0, transforms.Length);

                        if (lastTransformUsed != random)
                        {
                            Vector3 randomPosition = transforms[random].position;

                            enemyGameObject.transform.position = randomPosition;
                            enemyGameObject.transform.up = Player.transform.position;
                            enemyGameObject.SetActive(true);

                            lastTransformUsed = random;

                            timerRespawn = GameManager.Instance.TimeRespawn;
                        }
                    }
                }
                break;
            case GameStates.GameOver:
                break;
        }
    }

    public void ChangeGameState(GameStates state)
    {
        switch (state)
        {
            case GameStates.Active: //Modo padrão
                Time.timeScale = 1;

                break;
            case GameStates.Paused:
                Time.timeScale = 0;

                break;
            case GameStates.GameStart:
                PlayersCanMove = true;
                EnemiesCanMove = true;
                SpawnNewEnemies = true;

                Timer = GameManager.Instance.TimeMatch;

                //Faz o primeiro objeto ser estanciado de primeira
                timerRespawn = -1; 
                break;
            case GameStates.GameOver:
                PlayersCanMove = false;
                EnemiesCanMove = false;
                SpawnNewEnemies = false;

                GameManager.Instance.ChangeScore(Score);
                break;
        }

        GameState = state;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Boat))]
public class PlayerController : MonoBehaviour
{
    [Header("SETTINGS")]
    [SerializeReference][Min(0.1f)]
    private float delayFire;
    [Header("PREFABS")]
    [Header("OBJECTS")]

    private bool fire1;
    private bool fire2;

    private float delay;

    private Vector2 moveDirection;

    private Boat boat;

    private readonly WaitForSeconds DELAY_DEATH = new WaitForSeconds(5);

    private void Start()
    {
        boat = GetComponent<Boat>();
    }

    public void Fire1(InputAction.CallbackContext context)
    {
        fire1 = context.performed;
    }

    public void Fire2(InputAction.CallbackContext context)
    {
        fire2 = context.performed;
    }

    public void Move(InputAction.CallbackContext context)
    {
        moveDirection = context.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        if (!boat.IsDeath)
        {
            if (GameController.Instance.PlayersCanMove)
            {
                boat.Move(moveDirection);

                delay += Time.fixedDeltaTime;

                //Só pode atirar depois do delay
                if ((fire1 || fire2) && delay > delayFire)
                {
                    boat.FrontalSingleShot(fire1);
                    boat.SideTripleShot(fire2);

                    delay = 0f;
                }
            }
        }
        else
        {
            //Caso o player morra, o jogo acaba
            StartCoroutine(GameOverCoroutine());
        }
    }

    #region Coroutines
    /// <summary>
    /// Executa o game over depois de um delay
    /// </summary>
    private IEnumerator GameOverCoroutine()
    {
        yield return DELAY_DEATH;

        GameController.Instance.ChangeGameState(GameController.GameStates.GameOver);

        yield return null;
    }
    #endregion
}

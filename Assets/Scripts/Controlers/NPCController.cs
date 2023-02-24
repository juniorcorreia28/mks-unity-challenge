using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Boat))]
public class NPCController : MonoBehaviour
{
    [Header("SETTINGS")]
    [SerializeReference][Min(0.1f)]
    private float delayFire;
    [SerializeField]
    private Types enemyType;
    [SerializeField]
    private float distanceShoot;

    private float delay;

    Vector3 targetDirection;

    private Boat boat;
    private Transform target;

    private readonly WaitForSeconds DELAY_DEATH = new WaitForSeconds(5);

    private enum Types
    {
        Shooter,
        Chaser,
    }

    private void Start()
    {
        boat = GetComponent<Boat>();

        //O alvo é o jogador
        target = GameController.Instance.Player.transform;
    }

    private void FixedUpdate()
    {
        if (GameController.Instance.EnemiesCanMove)
        {
            if (!boat.IsDeath)
            {
                Vector3 direction = target.position - transform.position;

                switch (enemyType)
                {
                    case Types.Shooter:
                        delay += Time.fixedDeltaTime;

                        //Só pode atirar depois do delay e quando o player chegar perto
                        if (Vector3.Distance(transform.position, target.position) < distanceShoot && delay > delayFire)
                        {
                            boat.FreeShot(true, direction.normalized);

                            delay = 0f;
                        }
                        break;
                    case Types.Chaser:
                        Vector3 forward = transform.right;

                        targetDirection = new Vector3(Vector3.Dot(forward, direction), 0f, 0f);
                        break;
                }

                boat.Move(targetDirection);
            }
            else
            {
                //Caso o player morra, o jogo acaba
                StartCoroutine(DisableCoroutine());
            }
        }
    }

    #region Coroutines
    /// <summary>
    /// Desativa o objeto depois de um delay
    /// </summary>
    private IEnumerator DisableCoroutine()
    {
        yield return DELAY_DEATH;

        GameController.Instance.Score++;

        boat.ResetValues();

        //Reinicia os valores do objeto
        delay = 0f;

        gameObject.SetActive(false);

        yield return null;
    }
    #endregion
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boat : MonoBehaviour
{
    [Header("SETTINGS")]
    [SerializeField]
    [Min(1)]
    private int healthMax;
    [SerializeField]
    private float velocity;
    [SerializeField]
    private Ignore ignoreCollision;
    [SerializeField]
    private Color color;
    [SerializeField]
    private LayerMask layerMaskEnviroment;
    [SerializeField]
    private List<Sprite> sprites;
    [Header("GAMEOBJECTS")]
    [SerializeField]
    private Transform hud;
    [SerializeField]
    private Transform cannonFront;
    [SerializeField]
    private Transform cannonLeft;
    [SerializeField]
    private Transform cannonRight;
    [Header("OBJECTS")]

    private int health;
    private int ignoreLayer;

    private float lifeBarTime;

    private enum Ignore
    {
        Player,
        Enemy
    }

    private GameObject gameObjectCannonBall;
    private GameObject gameObjectExplosion;
    private GameObject gameObjectLifeBar;
    private GameObject gameObjectShot;

    private LifeBar lifeBar;
    private new Rigidbody2D rigidbody;
    private CapsuleCollider2D capsuleCollider;
    private SpriteRenderer spriteRenderer;

    RaycastHit2D[] raycastHits = new RaycastHit2D[2];

    public bool IsDeath { get; set; }

    private void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        health = healthMax;

        //Estancia a lifebar
        gameObjectLifeBar = ObjectPoolManager.Instance.GetPooledObject(3);

        if (gameObjectLifeBar != null)
        {
            //Joga o objeto na HUD
            gameObjectLifeBar.transform.parent = hud;
            gameObjectLifeBar.SetActive(true);

            lifeBar = gameObjectLifeBar.GetComponent<LifeBar>();

            lifeBar.Color = color;
        }

        switch (ignoreCollision)
        {
            case Ignore.Player:
                ignoreLayer = LayerMask.NameToLayer(Constants.UserLayers.IGNORE_PLAYER);
                break;
            case Ignore.Enemy:
                ignoreLayer = LayerMask.NameToLayer(Constants.UserLayers.IGNORE_ENEMY);
                break;
        }

        Physics2D.IgnoreLayerCollision(gameObject.layer, ignoreLayer);

        //Define o visual do barco
        spriteRenderer.sprite = sprites[health];
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!IsDeath)
        {
            //Recebe dano da cannonBall
            if (collision.CompareTag(Constants.UserTags.CANNON_BALL))
            {
                //'Destroi' o objeto cannonBall
                collision.gameObject.SetActive(false);

                Damage();
            }
            else if (collision.CompareTag(Constants.UserTags.ENEMY) && ignoreCollision == Ignore.Player)
            {
                Damage();
            }
            else if (collision.CompareTag(Constants.SystemTags.PLAYER) && ignoreCollision == Ignore.Enemy)
            {
                Death();
            }
        }
    }

    /// <summary>
    /// Move o barco
    /// </summary>
    /// <param name="moveDirection">Direção em que vai se mover</param>
    public void Move(Vector2 moveDirection)
    {
#if UNITY_EDITOR
        Debug.DrawLine(transform.position, transform.position + (transform.up * (capsuleCollider.size.y + 0.1f)), Color.red);
#endif

        int hits = capsuleCollider.Raycast(transform.up, raycastHits, capsuleCollider.size.y + 0.1f, layerMaskEnviroment);

        Vector2 forward = new Vector2(transform.up.x, transform.up.y).normalized;

        for (int i = 0; i < hits; i++)
        {
            if (raycastHits[i].collider.CompareTag(Constants.UserTags.GROUND))
            {
                forward = Vector2.zero;
            }
        }

        rigidbody.MovePosition(rigidbody.position + Time.fixedDeltaTime * velocity * forward);
        rigidbody.MoveRotation(rigidbody.rotation + Time.fixedDeltaTime * velocity + (-moveDirection.x));

        //Atualiza a posição da barra de vida
        lifeBar.Position(transform.position);

        //Após uns segundos, a barra de vida some
        lifeBarTime += Time.fixedDeltaTime;

        if (lifeBarTime > lifeBar.Duration && lifeBar.IsShowing)
        {
            lifeBar.Show(false);
        }
    }

    /// <summary>
    /// Atira na direção
    /// </summary>
    /// <param name="performed">botão de atirar</param>
    /// <param name="direction">direção do tiro</param>
    public void FreeShot(bool performed, Vector3 direction)
    {
        if (performed)
        {
            //Estancia a bola de canhão
            gameObjectCannonBall = ObjectPoolManager.Instance.GetPooledObject(0);

            if (gameObjectCannonBall != null)
            {
                gameObjectCannonBall.transform.position = cannonFront.position;
                gameObjectCannonBall.transform.up = direction;
                gameObjectCannonBall.layer = ignoreLayer;
                gameObjectCannonBall.SetActive(true);

                //Estancia fumação do tiro
                gameObjectShot = ObjectPoolManager.Instance.GetPooledObject(4);

                if (gameObjectShot != null)
                {
                    gameObjectShot.transform.parent = cannonFront.transform;

                    gameObjectShot.transform.position = cannonFront.position;
                    gameObjectShot.transform.up = direction;
                    gameObjectShot.SetActive(true);
                }
            }
        }
    }

    /// <summary>
    /// Tiro frontal de canhão
    /// </summary>
    /// <param name="performed">botão de atirar</param>
    public void FrontalSingleShot(bool performed)
    {
        if (performed)
        {
            //Estancia a bola de canhão
            gameObjectCannonBall = ObjectPoolManager.Instance.GetPooledObject(0);

            if (gameObjectCannonBall != null)
            {
                gameObjectCannonBall.transform.SetPositionAndRotation(cannonFront.position, cannonFront.rotation);
                gameObjectCannonBall.layer = ignoreLayer;
                gameObjectCannonBall.SetActive(true);

                //Estancia fumação do tiro
                gameObjectShot = ObjectPoolManager.Instance.GetPooledObject(4);

                if (gameObjectShot != null)
                {
                    gameObjectShot.transform.parent = cannonFront.transform;

                    gameObjectShot.transform.SetPositionAndRotation(cannonFront.position, cannonFront.rotation);
                    gameObjectShot.SetActive(true);
                }
            }
        }
    }

    /// <summary>
    /// Tiro lateral de canhão
    /// </summary>
    /// <param name="performed">botão de atirar</param>
    public void SideTripleShot(bool performed)
    {
        if (performed)
        {
            Transform cannon;
            Vector3 cannonPosition = Vector3.zero;

            for (int i = 0; i < 6; i++)
            {
                if (i < 3)
                {
                    cannon = cannonLeft;

                    switch (i)
                    {
                        case 0:
                            cannonPosition = cannon.position + transform.up * 0.5f;
                            break;
                        case 1:
                            cannonPosition = cannon.position;
                            break;
                        case 2:
                            cannonPosition = cannon.position + transform.up * -0.5f;
                            break;
                    }
                }
                else
                {
                    cannon = cannonRight;

                    switch (i)
                    {
                        case 3:
                            cannonPosition = cannon.position + transform.up * 0.5f;
                            break;
                        case 4:
                            cannonPosition = cannon.position;
                            break;
                        case 5:
                            cannonPosition = cannon.position + transform.up * -0.5f;
                            break;
                    }
                }

                //Estancia a bola de canhão
                gameObjectCannonBall = ObjectPoolManager.Instance.GetPooledObject(0);

                if (gameObjectCannonBall != null)
                {
                    gameObjectCannonBall.transform.SetPositionAndRotation(cannonPosition, cannon.rotation);
                    gameObjectCannonBall.layer = ignoreLayer;
                    gameObjectCannonBall.SetActive(true);

                    //Estancia fumação do tiro
                    gameObjectShot = ObjectPoolManager.Instance.GetPooledObject(4);

                    if (gameObjectShot != null)
                    {
                        gameObjectShot.transform.parent = cannon;

                        gameObjectShot.transform.SetPositionAndRotation(cannonPosition, cannon.rotation);
                        gameObjectShot.SetActive(true);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Reinicia os valores do objeto
    /// </summary>
    public void ResetValues()
    {
        lifeBarTime = 0f;
        health = healthMax;

        IsDeath = false;

        capsuleCollider.enabled = true;

        //Define o visual do barco
        spriteRenderer.sprite = sprites[health];
    }

    /// <summary>
    /// Recebe um dano
    /// </summary>
    private void Damage()
    {
        //Mostra a barra de vida
        if (!lifeBar.IsShowing)
        {
            lifeBar.Show(true);

            lifeBarTime = 0;
        }

        //Efeito de explosão unica
        gameObjectExplosion = ObjectPoolManager.Instance.GetPooledObject(1);

        if (gameObjectExplosion != null)
        {
            gameObjectExplosion.transform.position = transform.position;
            gameObjectExplosion.SetActive(true);
        }

        health--;

        spriteRenderer.sprite = sprites[health];

        lifeBar.UpdateLifeBar(health, healthMax);

        //Verifica se o barco foi destruido
        if (health < 1)
        {
            Death();
        }
    }

    /// <summary>
    /// Barco destruido
    /// </summary>
    private void Death()
    {
        //Efeito de explosão multipla
        gameObjectExplosion = ObjectPoolManager.Instance.GetPooledObject(2);

        if (gameObjectExplosion != null)
        {
            gameObjectExplosion.transform.position = transform.position;
            gameObjectExplosion.SetActive(true);
        }

        health = 0;

        spriteRenderer.sprite = sprites[health];

        capsuleCollider.enabled = false;

        IsDeath = true;
    }
}

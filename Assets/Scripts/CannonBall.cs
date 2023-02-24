using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonBall : MonoBehaviour
{
    [Header("SETTINGS")]
    [SerializeField][Min(0.1f)]
    private float velocity;

    private bool isDisabling;

    private TrailRenderer trailRenderer;

    private void Start()
    {
        trailRenderer = GetComponent<TrailRenderer>();
    }

    private void Update()
    {
        //O canhão se move pra direção que está olhando
        transform.Translate(Time.deltaTime * velocity * transform.up, Space.World);

        //Desabilita quando o objeto sair da visão da camera.
        if (isDisabling && !trailRenderer.isVisible)
        {
            gameObject.SetActive(false);
        }
    }

    private void OnBecameInvisible()
    {
        isDisabling = true;
    }

    private void OnEnable()
    {
        isDisabling = false;

        if (trailRenderer) trailRenderer.Clear();
    }
}

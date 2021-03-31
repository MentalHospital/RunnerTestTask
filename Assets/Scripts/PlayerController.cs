using UnityEngine;
using System;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    public event Action onTakingDamage;
    public event Action onGameOver;
    public int Lives { get; private set; } = 3;
    private const float playerModelRadius = 0.25f;
    private const float collisionCheckDistance = 0.05f;
    private const float playerStrafeSpeed = 2f;
    private const float lifeSubtractionTime = 3f;

    private float horizontalInput = 0f;
    private float rayLength;

    private float contactTime = 0f;
    private bool blinking = false;

    [SerializeField] private MapTreadmill mapTreadmill;
    [SerializeField] LayerMask raycastMask;
    [SerializeField] GameObject playerModel;

    private void Start()
    {
        rayLength = playerModelRadius + collisionCheckDistance;
    }

    private void Update()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
    }

    private void FixedUpdate()
    {
        if (CheckDirection(Vector3.left))
        {
            CountContactTime();
            if (horizontalInput > 0)
            {
                transform.Translate(Vector3.right * horizontalInput * playerStrafeSpeed * Time.fixedDeltaTime);
            }
        }
        else if (CheckDirection(Vector3.right))
        {
            CountContactTime();
            if (horizontalInput < 0)
            {
                transform.Translate(Vector3.right * horizontalInput * playerStrafeSpeed * Time.fixedDeltaTime);
            }
        }
        else
        {
            ResetContactTime();
            transform.Translate(Vector3.right * horizontalInput * playerStrafeSpeed * Time.fixedDeltaTime);
        }

        if (CheckForObjectsFront() && !blinking)
        {
            TakeDamage();
        }
    }

    private bool CheckDirection(Vector3 direction)
    {
        return Physics.Raycast(transform.position, direction, rayLength, raycastMask);
    }

    private bool CheckForObjectsFront()
    {
        Vector3 origin = transform.position - Vector3.forward * mapTreadmill.Speed * Time.fixedDeltaTime;
        float frontRayLength = rayLength + mapTreadmill.Speed * Time.fixedDeltaTime;
        bool frontLeft = Physics.Raycast(origin - Vector3.right * playerModelRadius, Vector3.forward, frontRayLength, raycastMask);
        bool frontRight = Physics.Raycast(origin + Vector3.right * playerModelRadius, Vector3.forward, frontRayLength, raycastMask);
        return frontLeft || frontRight;
    }

    private void CountContactTime()
    {
        contactTime += Time.fixedDeltaTime;
        if (contactTime >= lifeSubtractionTime)
        {
            TakeDamage();
            contactTime = 0f;
        }
    }
    private void ResetContactTime()
    {
        if (contactTime > 0)
        {
            contactTime = 0;
        }
    }

    private void TakeDamage()
    {
        Lives -= 1;
        if (Lives > 0)
        {
            onTakingDamage?.Invoke();
            StartCoroutine(BlinkAndStopRaycasting());
        }
        else
        {
            onGameOver?.Invoke();
        }
    }

    private IEnumerator BlinkAndStopRaycasting()
    {
        MeshRenderer meshRenderer = playerModel.GetComponent<MeshRenderer>();
        Color normalColor = meshRenderer.material.color;
        Color blinkColor = Color.red;
        blinking = true;
        for (int i = 0; i < 3; i++)
        {
            meshRenderer.material.SetColor("_Color", blinkColor);
            yield return new WaitForSeconds(.3f);
            meshRenderer.material.SetColor("_Color", normalColor);
            yield return new WaitForSeconds(.3f);
        }
        blinking = false;
    }
}
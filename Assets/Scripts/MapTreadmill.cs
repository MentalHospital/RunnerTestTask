using UnityEngine;
using System.Collections.Generic;

public class MapTreadmill : MonoBehaviour
{
    [SerializeField] private PlayerController player;
    private List<Transform> roadElements;
    private List<Transform> obstacles;
    [SerializeField] private Transform obstaclesHolder;
    [SerializeField] private Transform roadsHolder;
    [SerializeField] private GameObject roadPrefab;
    [SerializeField] private GameObject obstaclePrefab;
    public float Speed { get; private set; } = 1f;
    public float SpeedMinValue { get; private set; } = 5f;
    public float SpeedMaxValue { get; private set; } = 20f;

    private Camera cameraMain;
    private int roadsToShow = 5;
    private float roadHalfLength = 5f;
    private float roadHalfWidth = 1.5f;
    private float obstacleHalfLength = 0.375f;
    private float obstacleHalfWidth = 0.375f;
    private bool isMoving = false;
    private void Start()
    {
        isMoving = true;
        roadElements = new List<Transform>();
        obstacles = new List<Transform>();
        cameraMain = Camera.main;
        //requiring road elements to be chilren of map
        foreach(Transform t in roadsHolder)
        {
            roadElements.Add(t);
        }
        foreach (Transform t in obstaclesHolder)
        {
            obstacles.Add(t);
        }
        roadsToShow = roadElements.Count;

        player.onGameOver += GameOverHandler;
    }

    private void FixedUpdate()
    {
        if (!isMoving)
            return;

        foreach (var road in roadElements)
        {
            road.Translate(Vector3.back * Speed * Time.fixedDeltaTime);
            if (road.position.z < cameraMain.transform.position.z - roadHalfLength)  
            {
                ResetRoadPosition(road);
            }
        }
        foreach(var obstacle in obstacles)
        {
            obstacle.Translate(Vector3.back * Speed * Time.fixedDeltaTime);
            if (obstacle.position.z < cameraMain.transform.position.z - obstacleHalfWidth)
            {
                ResetObstaclePosition(obstacle);
            }
        }
    }
    public void SetSpeed(float speed)
    {
        Speed = speed;
    }

    private void ResetRoadPosition(Transform road)
    {
        road.position = new Vector3(road.position.x, road.position.y, road.position.z + roadHalfLength * 2 * roadsToShow);
    }

    private void ResetObstaclePosition(Transform obstacle)
    {
        obstacle.position = new Vector3(
            Random.Range(-roadHalfWidth + obstacleHalfWidth, roadHalfWidth - obstacleHalfWidth),
            obstacleHalfLength,
            Random.Range(-roadHalfLength, roadHalfLength) + roadHalfLength * 2 * roadsToShow
        );
    }

    private void GameOverHandler()
    {
        player.onGameOver -= GameOverHandler;
        isMoving = false;
    }
}

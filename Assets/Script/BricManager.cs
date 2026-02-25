
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BricManager : MonoBehaviour
{
    [SerializeField]
    private GameObject player;
    private float playerBoundaryX;

    public float t;

    public BricPool bricPool;
    public int rows = 5;
    public int cols = 11;
    public float spacing = 1.5f;
    public float stepDistanceVertical = 1f;

    public Vector2 startPosition = new Vector2(0, -4);

    private GameObject[,] brics;
    private int reverseGrave;

    private bool isPaused = false;

    private float time = 0;

    private enum MoveState { MoveRight, MoveLeft }
    private MoveState currentState = MoveState.MoveRight;

    void Start()
    {
        playerBoundaryX = player.GetComponent<PlayerScript>().boundary;
        brics = new GameObject[rows, cols];

        SpawnBrics();
    }

    private void Update()
    {
        time += Time.deltaTime;

        if (time >= t)
        {
            time = 0;
            StartCoroutine(MoveAllBricsDown());
        }
    }

    private void SpawnBrics()
    {
        var bricTypes = bricPool.GetBricsTypes();

        for (int row = 0; row < rows; row++)
        {
            var bricType = GetBricsTypeForRow(row, bricTypes);
            for (int col = 0; col < cols; col++)
            {
                GameObject bric = bricPool.GetBrics(bricType.prefab);

                if (bric != null)
                {
                    float xPos = startPosition.x + (col * spacing);
                    float yPos = startPosition.y - (row * spacing);

                    Debug.Log($"[EnemyManager] {bric.name} est à la position X: {xPos}; Y: {yPos}");

                    bric.transform.position = new Vector3(xPos, yPos, 0);

                    brics[row, col] = bric;

                    reverseGrave++;
                }
            }
        }
    }

    IEnumerator MoveAllBricsDown()
    {
        for (int row = rows - 1; row >= 0; row--)
        {
            for (int col = 0; col < cols; col++)
            {
                if (brics[row, col] != null && brics[row, col].activeSelf)
                {
                    Vector3 direction = Vector3.down;

                    MoveBrics(brics[row, col], direction, stepDistanceVertical);
                }
            }
        }
        yield break;
    }

    private int GetTopActiveRow()
    {
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                if (brics[row, col] != null && brics[row, col].activeSelf)
                {
                    return row;
                }
            }
        }

        return rows - 1;
    }

    private void MoveBrics(GameObject bric, Vector3 direction, float stepDistance)
    {
        if (bric == null) return;

        Vector3 newPosition = bric.transform.position + direction * stepDistance;

        newPosition.x = Mathf.Round(newPosition.x * 100f) / 100f;
        newPosition.y = Mathf.Round(newPosition.y * 100f) / 100f;
        newPosition.z = Mathf.Round(newPosition.z * 100f) / 100f;

        bric.transform.position = newPosition;

        BricBottom(bric, direction);
    }

    private void BricBottom(GameObject enemy, Vector3 direction)
    {
        if (enemy == null) return;

        Vector3 newPosition = enemy.transform.position;
    }

    public void ReturnBric(GameObject enemy, GameObject prefab)
    {
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                if (brics[row, col] == enemy)
                {
                    brics[row, col] = null;

                    return;
                }
            }
        }
    }

    private BricData.BricType GetBricsTypeForRow(int row, List<BricData.BricType> bricTypes)
    {
        if (row <= 2)
        {
            return bricTypes[2];
        }
        else if (row <= 5)
        {
            return bricTypes[1];
        }
        else
        {
            return bricTypes[0];
        }
    }
}

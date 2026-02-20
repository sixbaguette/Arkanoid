
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField]
    private GameObject player;
    private float playerBoundaryX;

    public EnemyPool enemyPool;
    public int rows = 5;
    public int cols = 11;
    public float spacing = 1.5f;
    public float stepDistanceVertical = 1f;

    public Vector2 startPosition = new Vector2(0, -4);

    private GameObject[,] enemies;
    private int reverseGrave;

    private bool isPaused = false;

    private float time = 0;

    private enum MoveState { MoveRight, MoveLeft }
    private MoveState currentState = MoveState.MoveRight;

    void Start()
    {
        playerBoundaryX = player.GetComponent<PlayerScript>().boundary;
        enemies = new GameObject[rows, cols];

        SpawnEnemies();
    }

    private void Update()
    {
        time += Time.deltaTime;

        if (time == 5)
        {
            time = 0;
            StartCoroutine(MoveAllEnemiesDown());
        }
    }

    private void SpawnEnemies()
    {
        var enemyTypes = enemyPool.GetEnemyTypes();

        for (int row = 0; row < rows; row++)
        {
            var enemyType = GetEnemyTypeForRow(row, enemyTypes);
            for (int col = 0; col < cols; col++)
            {
                GameObject enemy = enemyPool.GetEnemy(enemyType.prefab);

                if (enemy != null)
                {
                    float xPos = startPosition.x + (col * spacing);
                    float yPos = startPosition.y - (row * spacing);

                    Debug.Log($"[EnemyManager] {enemy.name} est à la position X: {xPos}; Y: {yPos}");

                    enemy.transform.position = new Vector3(xPos, yPos, 0);

                    enemies[row, col] = enemy;

                    reverseGrave++;
                }
            }
        }
    }

    IEnumerator MoveAllEnemiesDown()
    {
        for (int row = rows - 1; row >= 0; row--)
        {
            for (int col = 0; col < cols; col++)
            {
                if (enemies[row, col] != null && enemies[row, col].activeSelf)
                {
                    Vector3 direction = Vector3.down;

                    MoveEnemy(enemies[row, col], direction, stepDistanceVertical);

                    yield return null;
                }
            }
        }
    }

    private List<GameObject> GetBottomEnemies()
    {
        List<GameObject> bottomeEnemies = new List<GameObject>();

        for (int col = 0; col < cols; col++)
        {
            for (int row = rows - 1; row >= 0; row--)
            {
                if (enemies[row, col] != null && enemies[row, col].activeSelf)
                {
                    bottomeEnemies.Add(enemies[row, col]);
                    break;
                }
            }
        }

        return bottomeEnemies;
    }

    private void MoveEnemy(GameObject enemy, Vector3 direction, float stepDistance)
    {
        if (enemy == null) return;

        Vector3 newPosition = enemy.transform.position + direction * stepDistance;

        newPosition.x = Mathf.Round(newPosition.x * 100f) / 100f;
        newPosition.y = Mathf.Round(newPosition.y * 100f) / 100f;
        newPosition.z = Mathf.Round(newPosition.z * 100f) / 100f;

        enemy.transform.position = newPosition;

        EnemyBottom(enemy, direction);
    }

    private void EnemyBottom(GameObject enemy, Vector3 direction)
    {
        if (enemy == null) return;

        Vector3 newPosition = enemy.transform.position;
    }

    public void ReturnEnemy(GameObject enemy, GameObject prefab)
    {
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                if (enemies[row, col] == enemy)
                {
                    enemies[row, col] = null;
                }
            }
        }
    }

    private EnemyData.EnemyType GetEnemyTypeForRow(int row, List<EnemyData.EnemyType> enemyTypes)
    {
        if (row <= 2)
        {
            return enemyTypes[2];
        }
        else if (row <= 5)
        {
            return enemyTypes[1];
        }
        else
        {
            return enemyTypes[0];
        }
    }
}

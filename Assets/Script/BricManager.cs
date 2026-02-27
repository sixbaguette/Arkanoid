
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

    private List<List<GameObject>> brics = new List<List<GameObject>>();
    private int reverseGrave;

    private bool isPaused = false;

    private float time = 0;

    private enum MoveState { MoveRight, MoveLeft }
    private MoveState currentState = MoveState.MoveRight;

    void Start()
    {
        playerBoundaryX = player.GetComponent<PlayerScript>().boundary;

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
            CreateNewTopRow(bricTypes);
        }
    }

    IEnumerator MoveAllBricsDown()
    {
        foreach (var row in brics)
        {
            foreach (var bric in row)
            {
                if (bric != null && bric.activeSelf)
                {
                    bric.transform.position += Vector3.down * stepDistanceVertical;
                }
            }
        }

        RemoveBottomRowIfNeeded();

        yield return null;
    }

    private int GetTopActiveRow()
    {
        for (int row = 0; row < brics.Count; row++)
        {
            for (int col = 0; col < brics[row].Count; col++)
            {
                if (brics[row][col] != null && brics[row][col].activeSelf)
                {
                    return row;
                }
            }
        }

        return brics.Count - 1;
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

    public void ReturnBric(GameObject enemy)
    {
        if (enemy == null) return;

        Bric bricData = enemy.GetComponent<Bric>();
        if (bricData == null) return;

        bricPool.ReturnToPool(enemy, bricData.originalPrefab);

        // Retirer la brique de la grille
        for (int i = 0; i < brics.Count; i++)
        {
            for (int j = 0; j < brics[i].Count; j++)
            {
                if (brics[i][j] == enemy)
                {
                    brics[i][j] = null;
                }
            }
        }

        // Nettoyer les lignes vides
        for (int i = brics.Count - 1; i >= 0; i--)
        {
            if (brics[i].TrueForAll(b => b == null))
            {
                brics.RemoveAt(i);
            }
        }

        // Créer une nouvelle ligne si le haut est vide
        if (brics.Count == 0 || brics[0].TrueForAll(b => b == null))
        {
            CreateNewTopRow(bricPool.GetBricsTypes());
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

    private void CreateNewTopRow(List<BricData.BricType> bricTypes)
    {
        List<GameObject> newRow = new List<GameObject>();

        // Calculer la Y de la nouvelle ligne
        float yPos;

        if (brics.Count == 0)
        {
            // Pas de ligne existante → position de départ
            yPos = startPosition.y;
        }
        else
        {
            // Prendre la Y de la ligne la plus haute + spacing
            float maxY = float.MinValue;
            foreach (var bric in brics[0]) // ligne du haut
            {
                if (bric != null)
                    maxY = Mathf.Max(maxY, bric.transform.position.y);
            }
            yPos = maxY + spacing;
        }

        for (int col = 0; col < cols; col++)
        {
            var randomType = bricTypes[Random.Range(0, bricTypes.Count)];
            GameObject bric = bricPool.GetBrics(randomType.prefab);

            if (bric != null)
            {
                float xPos = startPosition.x + (col * spacing);
                bric.transform.position = new Vector3(xPos, yPos, 0);

                Bric id = bric.GetComponent<Bric>();
                if (id == null)
                    id = bric.AddComponent<Bric>();

                id.originalPrefab = randomType.prefab;

                bric.SetActive(true);

                newRow.Add(bric);
            }
        }

        brics.Insert(0, newRow);
    }

    private void RemoveBottomRowIfNeeded()
    {
        if (brics.Count == 0) return;

        var lastRow = brics[brics.Count - 1];

        bool allBelowScreen = true;

        foreach (var bric in lastRow)
        {
            if (bric != null && bric.transform.position.y > -4.5f)
            {
                allBelowScreen = false;
                break;
            }
        }

        if (allBelowScreen)
        {
            foreach (var bric in lastRow)
            {
                if (bric != null)
                {
                    Bric id = bric.GetComponent<Bric>();
                    bricPool.ReturnToPool(bric, id.originalPrefab);
                }
            }

            brics.RemoveAt(brics.Count - 1);
        }
    }
}

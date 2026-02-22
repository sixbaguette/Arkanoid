using System;
using UnityEngine;

public class Balle : MonoBehaviour
{
    public Transform paddle;
    [SerializeField] private float paddleWidth = 1.5f;
    [SerializeField] private float paddleHeight = 0.2f;
    [SerializeField] private float ballRadius = 0.15f;

    private PlayerScript PlayerScript;

    private float boundary;
    private float boundaryY;

    public float speed = 5f;

    [SerializeField]
    private Vector2 origin;
    [SerializeField]
    private Vector2 reflected;
    [SerializeField]
    private Vector2 direction;

    [SerializeField]
    private RaycastHit2D hit;

    private bool hasReflected = false;

    private Vector2 previousPosition;

    private void Awake()
    {
        PlayerScript = GameObject.Find("Paddle").GetComponent<PlayerScript>();
        direction = Vector2.zero;
    }

    private void Update()
    {
        if (paddle == null)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            transform.position = new Vector3(paddle.position.x, paddle.position.y + 0.5f, 0f);
            direction = new Vector2(1f, 1f).normalized;
        }

        CheckBounds();
        CheckPaddleCollision();

        previousPosition = transform.position;
        transform.Translate(direction * speed * Time.deltaTime);

        Vector3 screenBounds = Camera.main.ViewportToWorldPoint(Vector3.right);
        Vector3 screenBoundsY = Camera.main.ViewportToWorldPoint(Vector3.up);

        float newXPosition = transform.position.x;
        float newYPosition = transform.position.y;

        boundary = screenBounds.x * PlayerScript.boundaryPercentage;
        boundaryY = screenBoundsY.y * PlayerScript.boundaryPercentage;

        if ((newXPosition <= -boundary || newXPosition >= boundary
        || newYPosition <= -boundaryY || newYPosition >= boundaryY)
        && !hasReflected)
        {
            hasReflected = true;
        }

        if ((newXPosition > -boundary && newXPosition < boundary)
        && (newYPosition > -boundaryY && newYPosition < boundaryY))
        {
            hasReflected = false;
        }
    }

    private void CheckPaddleCollision()
    {
        Vector2 ballPos = transform.position;
        Vector2 paddlePos = paddle.position;

        bool isWithinX =
            ballPos.x + ballRadius >= paddlePos.x - paddleWidth / 2 &&
            ballPos.x - ballRadius <= paddlePos.x + paddleWidth / 2;

        bool crossedPaddle =
            previousPosition.y - ballRadius > paddlePos.y + paddleHeight / 2 &&
            ballPos.y - ballRadius <= paddlePos.y + paddleHeight / 2;

        if (isWithinX && crossedPaddle && direction.y < 0)
        {
            BounceFromPaddle(ballPos, paddlePos);
        }
    }

    private void BounceFromPaddle(Vector2 ballPos, Vector2 paddlePos)
    {
        float hitPoint = (ballPos.x - paddlePos.x) / (paddleWidth / 2);

        float maxBounceAngle = 75f;

        float bounceAngle = hitPoint * maxBounceAngle;

        float rad = bounceAngle * Mathf.Deg2Rad;

        direction = new Vector2(Mathf.Sin(rad), Mathf.Cos(rad)).normalized;
    }

    private void CheckBounds()
    {
        Vector3 screenBounds = Camera.main.ViewportToWorldPoint(Vector3.one);

        float boundaryX = screenBounds.x;
        float boundaryY = screenBounds.y;

        Vector2 pos = transform.position;

        if (pos.x <= -boundaryX + ballRadius)
        {
            direction.x = Mathf.Abs(direction.x);
            transform.position = new Vector2(-boundaryX + ballRadius, pos.y);
        }
        else if (pos.x >= boundaryX - ballRadius)
        {
            direction.x = -Mathf.Abs(direction.x);
            transform.position = new Vector2(boundaryX - ballRadius, pos.y);
        }

        if (pos.y >= boundaryY - ballRadius)
        {
            direction.y = -Mathf.Abs(direction.y);
            transform.position = new Vector2(pos.x, boundaryY - ballRadius);
        }
        else if (pos.y <= -boundaryY + ballRadius)
        {
            direction = Vector2.zero;
            transform.position = new Vector3(paddle.position.x, paddle.position.y + 0.5f, 0);
        }
    }
}

using UnityEngine;

public class Balle : MonoBehaviour
{
    private Rigidbody2D rb;

    private PlayerScript PlayerScript;

    private float boundary;
    private float boundaryY;

    public float speed = 25f;

    [SerializeField]
    private Vector2 origin;
    [SerializeField]
    private Vector2 reflected;
    [SerializeField]
    private Vector2 direction;

    [SerializeField]
    private RaycastHit2D hit;

    private bool hasReflected = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        PlayerScript = GameObject.Find("Paddle").GetComponent<PlayerScript>();
        direction = Vector2.up;
    }

    private void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime);

        Vector3 screenBounds = Camera.main.ViewportToWorldPoint(Vector3.right);
        Vector3 screenBoundsY = Camera.main.ViewportToWorldPoint(Vector3.up);

        float newXPosition = rb.position.x + (PlayerScript.currentSpeed * Time.fixedDeltaTime);

        boundary = screenBounds.x * PlayerScript.boundaryPercentage;
        boundaryY = screenBounds.x * PlayerScript.boundaryPercentage;

        if ((newXPosition <= -boundary || newXPosition >= boundary || newXPosition >= boundaryY || newXPosition <= boundaryY) && !hasReflected)
        {
            Reflect();

            newXPosition = Mathf.Clamp(newXPosition, -boundary, boundary);
            hasReflected = true;
            Debug.Log("true");
        }

        if (newXPosition > -boundary && newXPosition < boundary && newXPosition < boundaryY && newXPosition > -boundaryY)
        {
            hasReflected = false;
            Debug.Log("false");
        }
    }

    private void Reflect()
    {
        origin = transform.position;

        hit = Physics2D.Raycast(transform.position, direction);

        if (hit)
        {
            reflected = Reflect(direction, hit.normal);

            direction = reflected;
        }
    }

    Vector2 Reflect(Vector2 d, Vector2 n)
    {
        return d - 2 * Vector2.Dot(n, d) * n;
    }
}

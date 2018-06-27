using UnityEngine;
using System.Collections;
using Pathfinding;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Seeker))]

public class EnemyMovement : MonoBehaviour {

    // What to chase
    public Transform target;

    // How many times each second we will update our path
    public float updateRate = 2f;

    // Caching
    private Seeker seeker;
    private Rigidbody2D rb;

    // The calculated path
    public Path path;

    // The AI's speed per second
    public float speed = 300f;
    public ForceMode2D fMode;

    [HideInInspector]
    public bool pathIsEnded = false;

    // The max distance from the AI to a waypoint for it to continue to the next waypoint
    public float nextWaypointDistance = 1f;

    // The waypoint we are currently moving towards
    private int currentWaypoint = 0;

    private bool searchingForPlayer = false;

    public bool stillSearching = false;

    public bool facingRight = true;

    float horizontalVelocity;

    float timer = 0f;

    public float continueSearchingTime = 15f;

    Vector3 enemyLocation;

    Player player;
    EnemySenses enemySenses;

    public static EnemyMovement instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    void Start()
    {
        player = Player.instance;
        enemySenses = EnemySenses.instance;

        enemyLocation = transform.localScale;

        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();

        if (target == null)
        {
            if (!searchingForPlayer)
            {
                searchingForPlayer = true;
                StartCoroutine(SearchForPlayer());
            }
            return;
        }

        // Start a new path to the target position, return the result to the OnPathComplete method
        seeker.StartPath(transform.position, target.position, OnPathComplete);

        StartCoroutine(UpdatePath());
    }

    void FixedUpdate()
    {
        if (target != null && player.isDead == true || target != null && enemySenses.CanPlayerBeSeen() == false && stillSearching == false)
        {
            searchingForPlayer = false;
            return;
        }
        else if ((target != null && stillSearching == true && enemySenses.CanPlayerBeSeen() == false) || (target != null && stillSearching == false && enemySenses.CanPlayerBeSeen() == true))
        {
            if (!searchingForPlayer)
            {
                searchingForPlayer = true;
                StartCoroutine(UpdatePath());
            }
        }
        else if (target == null)
        {
            if (!searchingForPlayer)
            {
                searchingForPlayer = true;
                StartCoroutine(SearchForPlayer());
            }
            return;
        }

        if (path == null)
            return;

        if (currentWaypoint >= path.vectorPath.Count)
        {
            if (pathIsEnded)
                return;

            // Debug.Log("End of path reached.");
            pathIsEnded = true;
            return;
        }
        pathIsEnded = false;

        // Direction to the next waypoint
        Vector2 dir = (path.vectorPath[currentWaypoint] - transform.position).normalized;
        dir *= speed * Time.fixedDeltaTime;

        // Move the AI
        rb.AddForce(dir, fMode);

        // Debug.Log("Dir: " + dir);

        float dist = Vector3.Distance(transform.position, path.vectorPath[currentWaypoint]);
        if (dist < nextWaypointDistance)
        {
            currentWaypoint++;
            return;
        }
    }

    void LateUpdate()
    {
        Flip();
    }

    void Flip()
    {
        horizontalVelocity = rb.velocity.x;

        if (facingRight == true && horizontalVelocity < -.10) // Facing right and moving left
        {
            enemyLocation.x *= -1;
            transform.localScale = enemyLocation;
            facingRight = false;
        }
        else if (facingRight == false && horizontalVelocity > .10) // Facing left and moving right
        {
            enemyLocation.x *= -1;
            transform.localScale = enemyLocation;
            facingRight = true;
        }
    }

    IEnumerator SearchForPlayer()
    {
        GameObject searchResult = GameObject.FindGameObjectWithTag("Player");
        if (searchResult == null)
        {
            yield return new WaitForSeconds(0.5f);
            StartCoroutine(SearchForPlayer());
        }
        else if (player.isDead == false)
        {
            target = searchResult.transform;
            searchingForPlayer = false;
            StartCoroutine(UpdatePath());
            yield return false;
        }
    }

    IEnumerator UpdatePath()
    {
        if (target == null)
        {
            if (!searchingForPlayer)
            {
                searchingForPlayer = true;
                StartCoroutine(SearchForPlayer());
            }
            yield return false;
        }

        // Start a new path to the target position, return the result to the OnPathComplete method
        seeker.StartPath(transform.position, target.position, OnPathComplete);

        yield return new WaitForSeconds(1f / updateRate);
        StartCoroutine(UpdatePath());
    }

    public void OnPathComplete (Path p)
    {
        // Debug.Log("We got a path. Did it have an error? " + p.error);
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }

    IEnumerator OnTriggerExit2D()
    {
        stillSearching = true;

        while (timer < continueSearchingTime)
        {
            yield return new WaitForSeconds(1f);
            timer++;
            // Debug.Log("Timer: " + timer);
        }

        if (timer >= continueSearchingTime)
        {
            stillSearching = false;
        }
    }

    void OnTriggerEnter2D()
    {
        timer = 0.0f;
    }
}
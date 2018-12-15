using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guard : MonoBehaviour {

    public static event System.Action OnSpotted;
    /*I NEED A WAY TO RESET THIS EVENT WHILE ITS STATIC*/
    /*NOTE TO SELF: Remove comments after this*/

    public float speed = 5;
    public float waitTime = .3f;
    public float turnSpeed = 90f;
    public float TimeToSpotPlayer = 0.5f;
    private float playerVisibleTimer;

    public Light spotlight;
    public float viewDistance;
    public LayerMask viewMask;
    float viewAngle;

    public Transform pathHolder;
    Transform player;
    Color originalSpotLightColor;

    public GameObject hud;
    public GameObject Overhead_Camera;
    public GameObject Follow_Camera;

    public GameObject playerObject;
    public player playerScript;


    void Start()
    {
        //hud = GameObject.FindGameObjectWithTag("hud");
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerObject = GameObject.FindGameObjectWithTag("Player");
        playerScript = playerObject.GetComponent<player>();
        viewAngle = spotlight.spotAngle;
        originalSpotLightColor = spotlight.color;
        Vector3[] waypoints = new Vector3[pathHolder.childCount];
        for(int i = 0; i < waypoints.Length; i++)
        {
            waypoints[i] = pathHolder.GetChild(i).position;
            waypoints[i] = new Vector3(waypoints[i].x, transform.position.y, waypoints[i].z);
        }

        StartCoroutine(FollowPath(waypoints));

        Overhead_Camera.SetActive(true);
       // Follow_Camera.SetActive(false);
    }

    void Update()
    {
        if(CanSeePlayer())
        {
            //If the player is in the line of sight if the guard
            playerVisibleTimer += Time.deltaTime;
            transform.LookAt(GameObject.FindGameObjectWithTag("Player").transform);
            Time.timeScale = 1f;
            Follow_Camera.SetActive(true);
        }
        else
        {
            playerVisibleTimer -= Time.deltaTime;
            Time.timeScale = 1f;
            Follow_Camera.SetActive(false);
        }
        playerVisibleTimer = Mathf.Clamp(playerVisibleTimer, 0, TimeToSpotPlayer);
        spotlight.color = Color.Lerp(originalSpotLightColor, Color.red, playerVisibleTimer / TimeToSpotPlayer);

        if (playerVisibleTimer >= TimeToSpotPlayer)
        {
            // when the player gets caught
            if(OnSpotted != null)
            {
                OnSpotted();
                playerScript.disabled = true;
            }
        }
    }

    bool CanSeePlayer()
    {
        if (Vector3.Distance(transform.position,player.position) < viewDistance)
        {
            Vector3 dirToPlayer = (player.position - transform.position).normalized;
            float angleBetweenGuardAndPlayer = Vector3.Angle(transform.forward, dirToPlayer);
            if (angleBetweenGuardAndPlayer < viewAngle / 2f)
            {
                if (!Physics.Linecast(transform.position, player.position, viewMask))
                {
                    return true;
                }
            }
        }
        return false;
    }

    IEnumerator FollowPath(Vector3[] waypoints)
    {
        transform.position = waypoints[0];
        int targetWaypointIndex = 1;
        Vector3 targetWaypoint = waypoints[targetWaypointIndex];
        transform.LookAt(targetWaypoint);

        while (true)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetWaypoint, speed * Time.deltaTime);
            if (transform.position == targetWaypoint)
            {
                targetWaypointIndex = (targetWaypointIndex + 1) % waypoints.Length;
                targetWaypoint = waypoints[targetWaypointIndex];
                yield return new WaitForSeconds(waitTime);
                yield return StartCoroutine(TurnToFace(targetWaypoint));
            }
            yield return null;
        }
    }

    IEnumerator TurnToFace(Vector3 lookTarget)
    {
        Vector3 dirToLookTarget = (lookTarget - transform.position).normalized;
        float targetAngle = 90 - Mathf.Atan2(dirToLookTarget.z, dirToLookTarget.x) * Mathf.Rad2Deg;

        while (Mathf.Abs(Mathf.DeltaAngle(transform.eulerAngles.y, targetAngle)) > 0.05f)
        {
            float angle = Mathf.MoveTowardsAngle(transform.eulerAngles.y, targetAngle, turnSpeed * Time.deltaTime);
            transform.eulerAngles = Vector3.up * angle;
            yield return null;
        }
    }

    void OnDrawGizmos()
    {
        Vector3 startPos = pathHolder.GetChild(0).position;
        Vector3 previousPos = startPos;
        foreach (Transform waypoint in pathHolder)
        {
            Gizmos.DrawSphere(waypoint.position, .3f); //creates the shperes in the scene view
            Gizmos.DrawLine(previousPos, waypoint.position);
            previousPos = waypoint.position;
        }
        Gizmos.DrawLine(previousPos, startPos);
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.forward * viewDistance);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guard : MonoBehaviour
{
    public static event System.Action onGuardHasSpottedPlayer;

    public Transform pathHolder;
    public float delay = .5f;
    float speed = 8f;
    public float turnSpeed = 90f;
    public float timeToSpotPlayer = .5f;
    float playerVisibleTimer;

    public Light spotlight;
    public float viewDistance;
    float viewAngle;
    Transform player;
    public LayerMask mask;
    public Color originalSpotlightColor;
    
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        viewAngle = spotlight.spotAngle;
        Vector3[] waypoints = new Vector3[pathHolder.childCount];

        for (int i = 0; i < waypoints.Length; i++) {
            waypoints[i] = pathHolder.GetChild(i).position;
            waypoints[i] = new Vector3(waypoints[i].x, transform.position.y, waypoints[i].z);
        }

        StartCoroutine(followPath(waypoints));
    }

   
    void Update()
    {
        if (canSeePlayer())
            playerVisibleTimer += Time.deltaTime;
        else
            playerVisibleTimer -= Time.deltaTime;

        playerVisibleTimer = Mathf.Clamp(playerVisibleTimer, 0, timeToSpotPlayer);
        spotlight.color = Color.Lerp(originalSpotlightColor, Color.red, playerVisibleTimer / timeToSpotPlayer);

        if (playerVisibleTimer >= timeToSpotPlayer) {
            if (onGuardHasSpottedPlayer != null)
                onGuardHasSpottedPlayer();
        }
    }

    bool canSeePlayer() {
        if (Vector3.Distance(transform.position, player.position) < viewDistance) {
            Vector3 directionToPlayer = (player.position - transform.position).normalized;
            float angleBetweenGuardAndPlayer = Vector3.Angle(transform.forward, directionToPlayer);
            if (angleBetweenGuardAndPlayer < viewAngle / 2f) {
                Ray ray = new Ray(transform.position, directionToPlayer);
                RaycastHit hitInfo;

                /*if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity, mask))
                    Debug.DrawLine(ray.origin, hitInfo.point, Color.green);
                else
                    Debug.DrawLine(ray.origin, ray.origin + ray.direction * 100, Color.blue); */

                if (Physics.Raycast(ray, out hitInfo)) {
                    if (hitInfo.collider.tag == "Player")
                        return true;
                }
            }
        }

        return false;
    }

    void OnDrawGizmos() {
        Vector3 startPosition = pathHolder.GetChild(0).position;
        Vector3 previousPosition = startPosition;

        foreach (Transform waypoint in pathHolder) {
            Gizmos.DrawSphere(waypoint.position, .3f);
            Gizmos.DrawLine(previousPosition, waypoint.position);
            previousPosition = waypoint.position;
        }

        Gizmos.DrawLine(previousPosition, startPosition);

        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.forward * viewDistance);
    }

    IEnumerator followPath(Vector3[] waypoints) {
        while (true) {
            foreach (Vector3 waypoint in waypoints) {
                yield return turnToFace(waypoint);
                yield return StartCoroutine(move(waypoint, speed));
                yield return new WaitForSeconds(delay);
            }
        }
    }

    IEnumerator move(Vector3 desitnation, float speed) {
        while (transform.position != desitnation) {
            transform.position = Vector3.MoveTowards(transform.position, desitnation, speed * Time.deltaTime);
            yield return null;
        }
    }

    IEnumerator turnToFace(Vector3 target) {
        Vector3 direction = (target - transform.position).normalized;
        float targetAngle = 90 - Mathf.Atan2(direction.z, direction.x) * Mathf.Rad2Deg;

        while (Mathf.Abs(Mathf.DeltaAngle(transform.eulerAngles.y, targetAngle)) > 0.05f) {
            float angle = Mathf.MoveTowardsAngle(transform.eulerAngles.y, targetAngle, turnSpeed * Time.deltaTime);
            transform.eulerAngles = Vector3.up * angle;
            yield return null;
        }
    }
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StudentVision : MonoBehaviour
{
    public float viewRadius;
    [Range(0, 360)] public float viewAngle;

    public LayerMask targetMask;
    public LayerMask obstacleMask;

    public List<Transform> visibleTargets = new List<Transform>();

    private WaitForSeconds waitForCheckDelay;

    private void Start()
    {
        waitForCheckDelay = new WaitForSeconds(0.2f);
        StartCoroutine(FOVRoutine());
    }

    private IEnumerator FOVRoutine()
    {
        while (true)
        {
            yield return waitForCheckDelay;
            FindVisibleTargets();
        }
    }

    private void FindVisibleTargets()
    {
        visibleTargets.Clear();
        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, targetMask);

        foreach (Collider col in targetsInViewRadius)
        {
            Transform target = col.transform;
            Vector3 dirToTarget = (target.position - transform.position).normalized;
            if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
            {
                float distanceToTarget = Vector3.Distance(transform.position, target.position);
                if (!Physics.Raycast(transform.position, dirToTarget, distanceToTarget, obstacleMask))
                {
                    if (target.gameObject.layer == 7 || target.gameObject.layer == 6)
                    {
                        Transform mainParent = GameGlobals.instance.FindMainParent(target.gameObject)?.transform;
                        if (mainParent != null && !visibleTargets.Contains(mainParent))
                        {
                            visibleTargets.Add(mainParent);
                        }
                    }
                    else if (!visibleTargets.Contains(target))
                    {
                        visibleTargets.Add(target);
                    }
                }
            }
        }
    }

    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, viewRadius);

        Vector3 viewAngleA = DirFromAngle(-viewAngle / 2, false);
        Vector3 viewAngleB = DirFromAngle(viewAngle / 2, false);

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + viewAngleA * viewRadius);
        Gizmos.DrawLine(transform.position, transform.position + viewAngleB * viewRadius);

        Gizmos.color = Color.red;
        foreach (Transform visibleTarget in visibleTargets)
        {
            Gizmos.DrawLine(transform.position, visibleTarget.position);
        }
    }

    public bool CanSeeCorpse()
    {
        return visibleTargets.Exists(target => target.CompareTag("Corpse"));
    }

    public bool CanSeePlayerCarryingCorpse()
    {
        var playerController = GameGlobals.instance.Player;
        return visibleTargets.Contains(playerController.transform) && playerController.currentCorpse != null;
    }

    public bool CanSeeBlood()
    {
        return visibleTargets.Exists(target => target.CompareTag("Blood"));
    }
}

using UnityEngine;

namespace RPG.Control
{
    public class PatrolPath : MonoBehaviour
    {
        [SerializeField] private Color startingWaypointColourValue = new Vector4(0f, 0.7f, 1f, 1f);
        [SerializeField] private Color waypointColourValue = new Vector4(0f, 0f, 1f, 1f);

        public Color startingWaypointColour
        {
            get { return startingWaypointColourValue; }
            set { startingWaypointColourValue = value; }
        }

        public Color waypointColour
        {
            get { return waypointColourValue; }
            set { waypointColourValue = value; }
        }

        private void OnDrawGizmos()
        {
            const float waypointGizmoRadius = 0.3f;
            const float waypointGizmoHeight = 0.2f;
            const float waypointGizmoOffset = waypointGizmoHeight / 2f;
            for (int i = 0; i < transform.childCount; i++)
            {
                Vector3 waypoint = GetWaypoint(i);
                Vector3 nextWaypoint = GetWaypoint(GetNextIndex(i));

                if (i == 0)
                    Gizmos.color = startingWaypointColour;
                else
                    Gizmos.color = waypointColour;

                Gizmos.DrawSphere(waypoint + Vector3.up * waypointGizmoOffset, waypointGizmoRadius);
                Gizmos.DrawLine(waypoint, nextWaypoint);
            }
        }

        public int GetNextIndex(int i)
        {
            return (i + 1) % transform.childCount;
        }

        public Vector3 GetWaypoint(int i)
        {
            return transform.GetChild(i).position;
        }

        public float GetWaypointDwellTime(int i)
        {
            WaypointDwellTime waypointDwellTime = transform.GetChild(i).GetComponent<WaypointDwellTime>();
            if (waypointDwellTime != null)
            {
                return waypointDwellTime.DwellTime;
            }
            return 0f;
        }
        public int GetClosestWaypoint(Vector3 position)
        {
            int closestWaypointIndex = 0;
            float closestDistance = Mathf.Infinity;
            for (int i = 0; i < transform.childCount; i++)
            {
                float distance = Vector3.Distance(position, GetWaypoint(i));
                if (distance < closestDistance)
                {
                    closestWaypointIndex = i;
                    closestDistance = distance;
                }
            }
            return closestWaypointIndex;
        }
    }
}

using System;
using UnityEngine;

namespace ProceduralCreature
{
    [System.Serializable]
    public class Point : MonoBehaviour
    {
        public float size;
        [SerializeField] public Point parent_point;
        [SerializeField] public Point children_point;

        private int index;
        private float desired_distance = 4f;

        public void SetIndex(int i)
        {
            index = i;
        }

        public Vector3 leftPosition;
        public Vector3 rightPosition;

        private void CalculateSides()
        {
            Vector3 direction;

            if (parent_point != null)// Direction to parent
                direction = (parent_point.transform.position - transform.position).normalized;
            else if (children_point != null)// If no parent, use the direction to the child
                direction = (children_point.transform.position - transform.position).normalized;
            else// If the point has no parent and no children, exit (isolated point)
            {
                leftPosition = transform.position;
                rightPosition = transform.position;
                return;
            }

            Vector3 normal = Vector3.Cross(direction, Vector3.up).normalized; // Find a perpendicular vector in the XZ plane
            // Offset positions based on size, placing them on the border of the "Point"
            leftPosition = transform.position + normal * (size );
            rightPosition = transform.position - normal * (size );
        }

        private void Constrain()
        {
            if (children_point != null)
            {
                var direction = (children_point.transform.position - transform.position).normalized;
                var newPosition = transform.position + direction * desired_distance;
                children_point.transform.position = newPosition;
            }
        }

        public void Ping()
        {
            Constrain();
            CalculateSides();
        }

        public void ScaleBy(float value)
        {
            size = value;
            transform.localScale = Vector3.one * size;
        }

        public Point()
        {
            size = 1f;
            parent_point = null;
            children_point = null;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(leftPosition, 0.2f);
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(rightPosition, 0.2f);
        }
    }
}
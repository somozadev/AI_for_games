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
        public Vector3 leftPosition;
        public Vector3 rightPosition;
        private int index;
        [SerializeField] private float desired_distance = 1f;
        public float repositionSpeed = 2f;
        public float angleTheshold = 75f;


        [SerializeField] private bool hasLegs = false;
        [SerializeField] GameObject limbPrefab;
        [SerializeField] public Limb left_leg_parent;
        [SerializeField] public Limb right_leg_parent;


        public void Init(bool legs)
        {
            hasLegs = legs;
            size = 1f;
            parent_point = null;
            children_point = null;

            if (!hasLegs) return;
            CalculateSides();

            var aGameObjectL = Instantiate(limbPrefab, transform);
            aGameObjectL.transform.position = leftPosition;
            aGameObjectL.name = "limb_left";
            left_leg_parent = aGameObjectL.GetComponent<Limb>();
            left_leg_parent.Init(this, true);

            var aGameObjectR = Instantiate(limbPrefab, transform);
            aGameObjectR.transform.position = rightPosition;
            aGameObjectR.name = "limb_right";
            right_leg_parent = aGameObjectR.GetComponent<Limb>();
            right_leg_parent.Init(this, false);
        }



        public void SetIndex(int i)
        {
            index = i;
        }

        public void SetDistance(float distance)
        {
            desired_distance = distance;
        }

        public float GetPointLength()
        {
            return desired_distance;
        }

        public void Ping()
        {
            Constrain();
            CalculateSides();
            ApplyRotationThreshold();
        }

        private void ApplyRotationThreshold()
        {
            if (children_point != null &&
                children_point.children_point != null) // Ensure there is a parent, child, and grandchild
            {
                // Get the positions
                Vector3 parentPos = transform.position;
                Vector3 childPos = children_point.transform.position;
                Vector3 grandChildPos = children_point.children_point.transform.position;

                // Calculate the vector from parent to child and from child to grandchild
                Vector3 toChild = childPos - parentPos;
                Vector3 toGrandChild = grandChildPos - childPos;

                // Calculate the angle between the two vectors
                float angle = Vector3.Angle(toChild, toGrandChild);

                // If the angle exceeds the limit (e.g., 75 degrees)
                if (angle > 75f) // Adjust this value to your threshold
                {
                    // Calculate the normalized direction towards the child
                    Vector3 directionToChild = toChild.normalized;

                    // Calculate the target position for the grandchild
                    Vector3 targetPosition = childPos + directionToChild * desired_distance;

                    // Smoothly reposition the grandchild towards the target position
                    children_point.children_point.transform.position = Vector3.MoveTowards(
                        children_point.children_point.transform.position,
                        targetPosition,
                        repositionSpeed * Time.deltaTime);
                }
            }
        }

        private void CalculateSides()
        {
            Vector3 direction;

            if (parent_point != null) // Direction to parent
                direction = (parent_point.transform.position - transform.position).normalized;
            else if (children_point != null) // If no parent, use the direction to the child
                direction = (transform.position - children_point.transform.position).normalized;
            else // If the point has no parent and no children, exit (isolated point)
            {
                leftPosition = transform.position + transform.forward * size;
                rightPosition = transform.position - transform.forward * size;
                return;
            }

            Vector3 normal =
                Vector3.Cross(direction, Vector3.up).normalized; // Find a perpendicular vector in the XZ plane
            // Offset positions based on size, placing them on the border of the "Point"
            leftPosition = transform.position + normal * (size);
            rightPosition = transform.position - normal * (size);
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

        public void ScaleBy(float value)
        {
            size = value;
            transform.localScale = Vector3.one * size;
            transform.GetChild(0).localScale = Vector3.one * size * 2;
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
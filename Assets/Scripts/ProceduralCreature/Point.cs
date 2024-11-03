using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace ProceduralCreature
{
    [System.Serializable]
    public class Point : MonoBehaviour
    {
        public float size;
        [SerializeField] public Point parent_point;
        [SerializeField] public Point children_point;

        private int index;
        [SerializeField] private float desired_distance = 1f;
        public float repositionSpeed = 2f;
        public float angleTheshold = 75f;


        
        public virtual void Init()
        {
            size = 1f;
            Constrain();

        }

        public void SetIndex(int i)
        {
            index = i;
        }

        public void SetDistance(float distance)
        {
            desired_distance = distance;
            Constrain();
        }

        public float GetPointLength()
        {
            return desired_distance;
        }

        public virtual void Ping()
        {
            Constrain();
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

        


        private void Constrain()
        {
            if (children_point != null)
            {
                var direction = (children_point.transform.position - transform.position).normalized;
                var newPosition = transform.position + direction * desired_distance;
                children_point.transform.position = newPosition;
                children_point.transform.rotation = Quaternion.LookRotation(direction);
                

            }
        }

        public void ScaleBy(float value)
        {
            size = value;
            transform.localScale = Vector3.one * size;
           // transform.GetChild(0).localScale = Vector3.one * size * 2;
        }

        protected virtual void OnDrawGizmos()
        {

        }
    }
}
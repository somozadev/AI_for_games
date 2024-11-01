using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ProceduralCreature
{
    public class Limb : MonoBehaviour
    {
        public List<Point> points;
        [SerializeField] GameObject pointPrefab;
        public Transform target;
        private Vector3 baseInitialPosition;

        public Point attatchedPoint;
        public bool leftLimb;
        public bool righLimb;

        public void Init(Point pt, bool left)
        {
            attatchedPoint = pt;
            leftLimb = left;
            righLimb = !left;
            points = new List<Point>();

            for (int i = 0; i < 3; i++)
            {
                var p = Instantiate(pointPrefab, transform);
                p.name = "point_" + i;
                p.transform.position = new Vector3(p.transform.position.x, p.transform.position.y - i,
                    p.transform.position.z);
                p.GetComponent<Point>().ScaleBy(Random.Range(0.4f, 0.4f));
                points.Add(p.GetComponent<Point>());
                p.GetComponent<Point>().SetIndex(i + 1);
                p.GetComponent<Point>().SetDistance(1);
            }

            for (int i = 0; i < points.Count; i++)
            {
                if (i > 0)
                    points[i].parent_point = points[i - 1];
                if (i < points.Count - 1)
                    points[i].children_point = points[i + 1];
            }

            target = new GameObject("target").transform;
            foreach (Point point in points)
                point.Ping();


            target.SetParent(transform);
            target.position = points[points.Count - 1].transform.position;
            baseInitialPosition = points[0].transform.localPosition;
        }

        private void Update()
        {
            if (target && points.Count >= 2)
            {
                if (attatchedPoint != null)
                {
                    if (leftLimb)
                        transform.position = attatchedPoint.rightPosition;
                    if (righLimb)
                        transform.position = attatchedPoint.leftPosition;
                }
                MoveChainToTarget();
            }
        }

        private void MoveChainToTarget()
        {
            const int maxIterations = 150;
            const float threshold = 0.1f; // Threshold distance for the base to be close enough to its original position
            int iterations = 0;


            // FABRIK: Forward and Backward Iterations
            while (iterations < maxIterations)
            {
                // Step 1: Forward pass - start from hand and move towards the base
                points[points.Count - 1].transform.localPosition = target.localPosition; // Move the hand to the target

                for (int i = points.Count - 2; i >= 0; i--)
                {
                    Point current = points[i];
                    Point child = points[i + 1];

                    Vector3 direction = (child.transform.localPosition - current.transform.localPosition).normalized;
                    current.transform.localPosition =
                        child.transform.localPosition - direction * current.GetPointLength();
                }

                // Step 2: Check the base position against its initial position
                float baseDistanceToInitial = Vector3.Distance(points[0].transform.position, baseInitialPosition);
                if (baseDistanceToInitial < threshold)
                    break;

                // Step 3: Backward pass - start from base and move towards the hand
                points[0].transform.localPosition = baseInitialPosition; // Anchor the base at its initial position

                for (int i = 1; i < points.Count; i++)
                {
                    Point current = points[i];
                    Point parent = points[i - 1];

                    Vector3 direction = (current.transform.localPosition - parent.transform.localPosition).normalized;
                    current.transform.localPosition =
                        parent.transform.localPosition + direction * parent.GetPointLength();
                }

                // Step 4: Check the hand’s distance to the target
                float handDistanceToTarget =
                    Vector3.Distance(points[points.Count - 1].transform.localPosition, target.localPosition);
                if (handDistanceToTarget < threshold)
                    break;

                iterations++;
            }

            // Call Ping to constrain any rotation and other properties on each point after IK adjustment
            foreach (Point point in points)
            {
                point.Ping();
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            if (target)
                Gizmos.DrawWireCube(target.position, Vector3.one * 0.5f);
        }
    }
}


/*
 Steps for the FABRIK Method
Forward Pass (Hand to Base):

Start by positioning the end (hand) point at the target.
Move each parent point towards its child (the next point in the chain) to maintain their original distances.
Backward Pass (Base to Hand):

Reposition the root (base) point to its initial, anchored position.
Adjust each child point toward its parent to maintain the original distances.
Termination Conditions:

Stop when the base point's distance to its original position is within a small threshold.
Cap iterations to prevent an infinite loop (e.g., 150 iterations).
 *
 */
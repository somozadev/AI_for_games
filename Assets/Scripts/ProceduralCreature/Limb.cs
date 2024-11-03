using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace ProceduralCreature
{
    public class Limb : MonoBehaviour
    {
        public List<Point> points;
        [SerializeField] GameObject pointPrefab;
        public Transform target;
        private Vector3 baseInitialPosition;

        public BodyPoint bodyPoint;

        [SerializeField] private float maxDistance = 1.5f;

        private bool isLeft;
        private Limb contraryLimb;
        public bool isMoving;

        private bool isLockedY = true;
        private float lasStableHandYPosition = 0f; 
        private Coroutine animCorr;

        public MeshFilter _meshFilter;
        public MeshRenderer _meshRenderer;

        public void SetContraryLimb(Limb limb)
        {
            contraryLimb = limb;
        }

        public void Init(BodyPoint pt, bool left, Transform targetObj)
        {
            _meshFilter = GetComponent<MeshFilter>();
            _meshRenderer = GetComponent<MeshRenderer>();
            target = targetObj;
            isLeft = left;
            bodyPoint = pt;
            points = new List<Point>();
            for (int i = 0; i < 3; i++)
            {
                var p = Instantiate(pointPrefab, transform);
                p.name = "point_" + i;
                p.GetComponent<Point>().ScaleBy(Random.Range(0.4f, 0.4f));
                points.Add(p.GetComponent<Point>());
                p.GetComponent<Point>().SetIndex(i + 1);
                p.GetComponent<Point>().SetDistance(1);
                p.GetComponent<Point>().Init();
            }

            //this config may vary based on type of leg
            points[0].SetDistance(.5f);
            points[1].SetDistance(1.5f);
            points[2].SetDistance(2);

            for (int i = 0; i < points.Count; i++)
            {
                if (i > 0)
                    points[i].parent_point = points[i - 1];
                if (i < points.Count - 1)
                    points[i].children_point = points[i + 1];
            }
            
            
            lasStableHandYPosition = -0.5f; //set with body raycast to ground
            isLockedY = true;   

        }


        private void Update()
        {
            if (target && points.Count >= 2)
            {
                MoveChainToTarget();
                CheckDistanceToTarget();
                _meshFilter.mesh = MeshGenerator.GenerateTriangularMesh(points);

            }
        }

        private void MoveChainToTarget()
        {
            const int maxIterations = 150;
            const float threshold = 0.1f; // Threshold distance for the base to be close enough to its original position
            int iterations = 0;
            var handPosition = points[points.Count - 1];


            // FABRIK: Forward and Backward Iterations
            while (iterations < maxIterations)
            {
                // Step 1: Forward pass - start from hand and move towards the base
                handPosition.transform.position = target.position; // Move the hand to the target

                for (int i = points.Count - 2; i >= 0; i--)
                {
                    Point current = points[i];
                    Point child = points[i + 1];

                    Vector3 direction = (child.transform.position - current.transform.position).normalized;
                    current.transform.position = child.transform.position - direction * current.GetPointLength();
                }

                // Step 2: Check the base position against its initial position
                float baseDistanceToInitial = Vector3.Distance(points[0].transform.localPosition, Vector3.zero);
                if (baseDistanceToInitial < threshold)
                    break;

                // Step 3: Backward pass - start from base and move towards the hand
                points[0].transform.localPosition = Vector3.zero; // Anchor the base at its initial position

                for (int i = 1; i < points.Count; i++)
                {
                    Point current = points[i];
                    Point parent = points[i - 1];

                    Vector3 direction = (current.transform.position - parent.transform.position).normalized;
                    current.transform.position = parent.transform.position + direction * parent.GetPointLength();
                }

                // Step 4: Check the hand’s distance to the target
                float handDistanceToTarget = Vector3.Distance(handPosition.transform.position, target.position);
                if (handDistanceToTarget < threshold)
                    break;

                iterations++;
            }

            if (isLockedY)
                handPosition.transform.position = new Vector3(handPosition.transform.position.x, lasStableHandYPosition, handPosition.transform.position.z);
            // Call Ping to constrain any rotation and other properties on each point after IK adjustment
            foreach (Point point in points)
            {
                point.Ping();
            }
        }


        private void CheckDistanceToTarget()
        {
            Vector3 start = isLeft ? bodyPoint.attatchedTargetL : bodyPoint.attatchedTargetR;
            Debug.DrawLine(start, target.position, Color.red);


            if (Vector3.Distance(start, target.position) > maxDistance
                && animCorr == null && contraryLimb && !contraryLimb.isMoving)
            {
                // target.position = start;
                animCorr = StartCoroutine(LerpTargetTo(start));
            }
        }

        private IEnumerator LerpTargetTo(Vector3 pos)
        {
            var elapsedTime = 0f;
            var maxAnimTime = .05f;
            var targetInitialPos = target.position;
            while (elapsedTime <= maxAnimTime)
            {
                isMoving = true;
                elapsedTime += Time.deltaTime;
                target.position = Vector3.Lerp(targetInitialPos, pos, elapsedTime / maxAnimTime);
                yield return null;
            }

            target.position = pos;
            animCorr = null;
            isMoving = false;
            yield return null;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            if (target)
            {
                Gizmos.DrawWireCube(target.position, Vector3.one * 0.5f);
            }
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
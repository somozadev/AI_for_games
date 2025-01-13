 using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

namespace ProceduralCreature
{
    public class BodyPoint : Point
    {
        [SerializeField] private bool isDebugMode = true;

        [SerializeField] private bool hasLegs = false;
        [SerializeField] GameObject limbPrefab;
        [SerializeField] public Limb left_leg_parent;
        [SerializeField] public Limb right_leg_parent;
        public Vector3 leftPosition;
        public Vector3 rightPosition;

        [SerializeField] public Vector3 attatchedTargetL; //change this to be a transform so i can + - from it's forward to offset left and right legs 
        [SerializeField] public Vector3 attatchedTargetR;

        [SerializeField] private Transform targetL;
        [SerializeField] private Transform targetR;

        public void Init(bool legs)
        {
            base.Init();
            CalculateSides();
            Ping();

            hasLegs = legs;
            if (!hasLegs) return;

            StartCoroutine(WaitToRepositionBodyToInitLegs());
        }

        private IEnumerator WaitToRepositionBodyToInitLegs()
        {
            yield return new WaitForEndOfFrame();

            targetL = new GameObject(gameObject.name + "_targetL").transform;
            targetR = new GameObject(gameObject.name + "_targetR").transform;
            InitLegs();
        }

        private void InitLegs()
        {
            Vector3 tempPosL, tempPosR;
            RayCastFloor(out tempPosL, out tempPosR);
            targetL.position = tempPosL;
            targetR.position = tempPosR;
            CreateLeg(true);
            CreateLeg(false);
            left_leg_parent.SetContraryLimb(right_leg_parent);
            right_leg_parent.SetContraryLimb(left_leg_parent);
        }


        public override void Ping()
        {
            base.Ping();
            CalculateSides();

            if (hasLegs && isDebugMode)
                RayCastFloor(out attatchedTargetL, out attatchedTargetR);
        }

        
        private void RayCastFloor(out Vector3 left, out Vector3 right)
        {
            RaycastHit ray;

            if (Physics.Raycast(transform.position, -transform.up, out ray, 5, 1 << 6))
            {
                float y = ray.point.y;
                left = new Vector3(leftPosition.x, y, leftPosition.z);
                right = new Vector3(rightPosition.x, y, rightPosition.z);
                Debug.DrawRay(transform.position, -transform.up, Color.magenta, .2f);
            }
            else
            {
                left = default;
                right = default;
            }
        }

        private void CreateLeg(bool left)
        {
            var go = Instantiate(limbPrefab, transform);

            if (left)
            {
                go.transform.position = leftPosition;
                go.name = "limb_left";
                left_leg_parent = go.GetComponent<Limb>();
                left_leg_parent.Init(this, true, targetL);
            }
            else
            {
                go.transform.position = rightPosition;
                go.name = "limb_right";
                right_leg_parent = go.GetComponent<Limb>();
                right_leg_parent.Init(this, false, targetR);
            }
        }

        protected void CalculateSides()
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

        protected override void OnDrawGizmos()
        {
            if (!isDebugMode) return;

            base.OnDrawGizmos();
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(leftPosition, 0.2f);
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(rightPosition, 0.2f);

            if (!hasLegs) return;

            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(attatchedTargetL, 0.25f);
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(attatchedTargetR, 0.25f);
        }
    }
}
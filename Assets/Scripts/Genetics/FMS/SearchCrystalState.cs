using ProceduralCreature;
using UnityEngine;

namespace Genetics
{
    [System.Serializable]
    public class SearchCrystalState : BaseState
    {
        [SerializeField] private Vector3 _targetPosition;
        [SerializeField] private bool _targetSet = false;
        CreaturePlayerController controller;
        private float distanceThreshold = 25f;
        private float accumulatedDistance = 0f;

        public SearchCrystalState()
        {
            stateName = "Looking for crystals";
        }

        public override void EnterState(AgentStateManager agent)
        {
            return;
        }

        public override void EnterState(AgentStateManager agent, Collider collidedObject)
        {
            controller = agent.GetCreatureContainer().GetCreatureController();
            _targetPosition = collidedObject.transform.position;
            _targetSet = true;
        }

        public override void UpdateState(AgentStateManager agent)
        {
            if (!controller) return;

            if (Vector3.Distance(controller.transform.position, _targetPosition) <= 1f)
            {
                _targetSet = false;
                agent.SwitchState(agent.eatState);
            }

            MoveTowardsTarget(agent);
        }

        public override void OnTriggerEnter(Collider other, AgentStateManager agent, Creature creature)
        {
            return;
        }

        private void MoveTowardsTarget(AgentStateManager agent)
        {
            if (!_targetSet || !controller) return;
            var direction = (_targetPosition - controller.transform.position).normalized;
            var distanceToMove = agent.GetCreature().Chromosome.BasicStats.speed * Time.deltaTime;

            if (!controller.isPossessed)
            {
                Vector3 localDirection = controller.transform.InverseTransformDirection(direction);
                localDirection.x = 0;
                Vector3 moveDirection = controller.transform.TransformDirection(localDirection);

                accumulatedDistance += distanceToMove;
                if (accumulatedDistance >= distanceThreshold)
                {
                    agent.GetCreature().OnMovePerformed();
                    accumulatedDistance = 0f;
                }


                controller.transform.position +=
                    moveDirection * (agent.GetCreature().Chromosome.BasicStats.speed / 2 * Time.deltaTime);

                Quaternion toRotation = Quaternion.LookRotation(direction, Vector3.up);
                controller.transform.rotation = Quaternion.RotateTowards(controller.transform.rotation, toRotation,
                    controller.rotSpeed * Time.deltaTime);
            }
        }
    }
}
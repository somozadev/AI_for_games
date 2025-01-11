using ProceduralCreature;
using UnityEngine;

namespace Genetics
{
    [System.Serializable]
    public class SearchFruitState : BaseState
    {
        [SerializeField] private Vector3 _targetPosition;
        [SerializeField] private bool _targetSet = false;
        CreaturePlayerController controller;
        private float distanceThreshold = 25f;
        private float accumulatedDistance = 0f;
        private Collider _targetRef;

        public SearchFruitState()
        {
            stateName = "Looking for fruits";
        }

        public override void EnterState(AgentStateManager agent)
        {
            return;
        }

        public override void EnterState(AgentStateManager agent, Collider collidedObject)
        {
            controller = agent.GetCreatureContainer().GetCreatureController();
            _targetPosition = collidedObject.transform.position;
            _targetRef = collidedObject;
            _targetSet = true;
        }

        public override void UpdateState(AgentStateManager agent)
        {
            if (!controller) return;
            if(agent.GetCreature().Chromosome.BasicStats.energy <= 0)
                agent.SwitchState(agent.restState);
            if (Vector3.Distance(controller.transform.position, _targetPosition) <= 1f)
            {
                _targetSet = false;
                agent.SwitchState(agent.eatState, _targetRef);
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
        public override void OnTriggerStay(Collider other, AgentStateManager agent, Creature creature)
        {
            
        }
    }
}
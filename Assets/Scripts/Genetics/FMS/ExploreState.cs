using ProceduralCreature;
using UnityEngine;

namespace Genetics
{
    [System.Serializable]
    public class ExploreState : BaseState
    {
        [SerializeField] private Vector3 _targetPosition;
        [SerializeField] private bool _targetSet = false;
        CreaturePlayerController controller;

        private float exploringRadius = 1000; //perception radius used to detect food / danger instead 

        // agent.GetCreature().Chromosome.BasicStats.perception * TRIGGER USAGE? SIZE BASED ?
        public override void EnterState(AgentStateManager agent)
        {
            controller = agent.GetCreatureContainer().GetCreatureController();
            SetRandomTarget(agent);
        }

        public override void UpdateState(AgentStateManager agent)
        {
            if (!controller) return;

            if (Vector3.Distance(controller.transform.position, _targetPosition) <= 1f)
            {
                _targetSet = false;
                agent.SwitchState(agent.restState);
                // SetRandomTarget(agent);
            }

            MoveTowardsTarget(agent);
        }

        public override void OnTriggerEnter(Collider other, AgentStateManager agent, Creature creature)
        {
            if (other.CompareTag("Fruit") && creature.Chromosome.Diet == enums.DietType.Frugivorous || creature.Chromosome.Diet == enums.DietType.Omnivorous)
                agent.SwitchState(agent.searchFruitState);
            else if (other.CompareTag("Creature") && creature.Chromosome.Diet == enums.DietType.Carnivorous|| creature.Chromosome.Diet == enums.DietType.Omnivorous)
                agent.SwitchState(agent.searchCreatureState);
            else if (other.CompareTag("Plant") && creature.Chromosome.Diet == enums.DietType.Herbivorous || creature.Chromosome.Diet == enums.DietType.Omnivorous)
                agent.SwitchState(agent.searchPlantState);
            else if (other.CompareTag("Crystal") && creature.Chromosome.Diet == enums.DietType.Crystavorous)
                agent.SwitchState(agent.searchCrystalState);
        }

        private void MoveTowardsTarget(AgentStateManager agent)
        {
            if (!_targetSet || !controller) return;
            var direction = (_targetPosition - controller.transform.position).normalized;
            if (!controller.isPossessed)
            {
                Vector3 localDirection = controller.transform.InverseTransformDirection(direction);
                localDirection.x = 0;
                Vector3 moveDirection = controller.transform.TransformDirection(localDirection);
                controller.transform.position +=
                    moveDirection * (agent.GetCreature().Chromosome.BasicStats.speed * Time.deltaTime);

                Quaternion toRotation = Quaternion.LookRotation(direction, Vector3.up);
                controller.transform.rotation = Quaternion.RotateTowards(controller.transform.rotation, toRotation,
                    controller.rotSpeed * Time.deltaTime);
            }
        }


        private void SetRandomTarget(AgentStateManager agent)
        {
            float range = exploringRadius * Random.Range(1.0f, 2.0f);

            float randomX = Random.Range(-range, range);
            float randomZ = Random.Range(-range, range);
            Vector3 randomOffset = new Vector3(randomX, 0, randomZ);
            _targetPosition = agent.GetCreatureContainer().GetCreatureController().transform.position + randomOffset;
            _targetSet = true;
        }
    }
}
using System.Collections;
using UnityEngine;

namespace Genetics
{
    [System.Serializable]
    public class RestState : BaseState
    {
        private float elapsedTime = 0f;
        private float maxIdleTime = 5f;

        public RestState()
        {
            stateName = "Resting";
        }

        public override void EnterState(AgentStateManager agent)
        {
            return;
        }

        public override void EnterState(AgentStateManager agent, Collider collidedObject)
        {
            return;
        }

        public override void UpdateState(AgentStateManager agent)
        {
            elapsedTime += Time.deltaTime;
            if (elapsedTime >= maxIdleTime)
            {
                elapsedTime = 0f;
                agent.GetCreature().OnRestPerformed();
            }

            if (agent.GetCreature().Chromosome.BasicStats.HpLevels is <= 30 and > 20 ||
                agent.GetCreature().Chromosome.BasicStats.EnergyLevels <= 10) return;
            if (agent.GetCreature().Chromosome.BasicStats.EnergyLevels > 70)
                agent.SwitchState(agent.exploreState);
            else
            {
                var creature = agent.GetCreature();
                if (creature.Chromosome.Diet == enums.DietType.Frugivorous ||
                    creature.Chromosome.Diet == enums.DietType.Omnivorous)
                    agent.SwitchState(agent.searchFruitState);
                else if (creature.Chromosome.Diet == enums.DietType.Carnivorous ||
                         creature.Chromosome.Diet == enums.DietType.Omnivorous)
                    agent.SwitchState(agent.searchCreatureState);
                else if (creature.Chromosome.Diet == enums.DietType.Herbivorous ||
                         creature.Chromosome.Diet == enums.DietType.Omnivorous)
                    agent.SwitchState(agent.searchPlantState);
                else if (creature.Chromosome.Diet == enums.DietType.Crystavorous)
                    agent.SwitchState(agent.searchCrystalState);
            }
        }


        public override void OnTriggerEnter(Collider other, AgentStateManager agent, Creature creature)
        {
            return;
        }
    }
}
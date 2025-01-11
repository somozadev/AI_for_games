using UnityEngine;

namespace Genetics
{
    [System.Serializable]
    public class AttackState : BaseState
    {
        private float elapsedTime = 1.5f;
        private float maxAttackTime = 1.5f;
        private GameObject otherCreature;
        private CreatureContainer otherCreatureComponent;

        public AttackState()
        {
            stateName = "Attacking";
        }


        public override void EnterState(AgentStateManager agent)
        {
            return;
        }

        public override void EnterState(AgentStateManager agent, Collider collidedObject)
        {
            otherCreature = collidedObject.gameObject;
            otherCreatureComponent = otherCreature.GetComponentInParent<CreatureContainer>();
        }

        public override void UpdateState(AgentStateManager agent)
        {
            elapsedTime += Time.deltaTime;
            if (elapsedTime >= maxAttackTime)
            {
                elapsedTime = 0f;
                otherCreatureComponent.Creature.OnDamageReceived(agent.GetCreature());
                agent.GetCreature().OnMovePerformed(); //attack consumes twice the energy than move
                agent.GetCreature().OnMovePerformed();

                if (otherCreatureComponent.Creature.Chromosome.BasicStats.hp <= 0)
                {
                    agent.GetCreature().OnEatPray();
                    agent.SwitchState(agent.restState);
                    
                }
                if (agent.GetCreature().Chromosome.BasicStats.energy <= 0)
                    agent.SwitchState(agent.restState);
                if (agent.GetCreature().Chromosome.BasicStats.HpLevels <= 15)
                    agent.SwitchState(agent.exploreState);
            }
        }


        public override void OnTriggerEnter(Collider other, AgentStateManager agent, Creature creature)
        {
            return;
        }

        public override void OnTriggerStay(Collider other, AgentStateManager agent, Creature creature)
        {
            
        }
    }
}
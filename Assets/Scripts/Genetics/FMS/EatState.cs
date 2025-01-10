using UnityEngine;

namespace Genetics
{
    [System.Serializable]
    public class EatState : BaseState
    {
        private float elapsedTime = 0f;
        private float maxEatingTime = 6f;
        private GameObject foodGoRef;

        public EatState()
        {
            stateName = "Eating";
        }


        public override void EnterState(AgentStateManager agent)
        {
            return;
        }

        public override void EnterState(AgentStateManager agent, Collider collidedObject)
        {
            foodGoRef = collidedObject.gameObject;
        }

        public override void UpdateState(AgentStateManager agent)
        {
            elapsedTime += Time.deltaTime;
            if (elapsedTime >= maxEatingTime)
            {
                elapsedTime = 0f;
                agent.GetCreature().OnEatFood();
                if (foodGoRef)
                    GameManager.Instance.FoodSpawner.Remove(foodGoRef);
                agent.SwitchState(agent.exploreState);
            }
        }


        public override void OnTriggerEnter(Collider other, AgentStateManager agent, Creature creature)
        {
        }
        public override void OnTriggerStay(Collider other, AgentStateManager agent, Creature creature)
        {
            
        }
    }
}
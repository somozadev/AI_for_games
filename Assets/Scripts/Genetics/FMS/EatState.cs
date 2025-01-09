using UnityEngine;

namespace Genetics
{
    [System.Serializable]
    public class EatState : BaseState
    {
        public EatState()
        {
            stateName = "Eating";
        }
        public override void EnterState(AgentStateManager agent)
        {
            throw new System.NotImplementedException();
        }

        public override void EnterState(AgentStateManager agent, Collider collidedObject)
        {
            throw new System.NotImplementedException();
        }

        public override void UpdateState(AgentStateManager agent)
        {
            throw new System.NotImplementedException();
        }

        public override void OnTriggerEnter(Collider other, AgentStateManager agent, Creature creature)
        {
            throw new System.NotImplementedException();
        }
    }
}
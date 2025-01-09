using UnityEngine;

namespace Genetics
{
        [System.Serializable]
    public class FleeState : BaseState
    {
        public FleeState()
        {
            stateName = "Escaping";
        }
        public override void EnterState(AgentStateManager agent)
        {
        }

        public override void EnterState(AgentStateManager agent, Collider collidedObject)
        {
            throw new System.NotImplementedException();
        }

        public override void UpdateState(AgentStateManager agent)
        {
        }

        public override void OnTriggerEnter(Collider other, AgentStateManager agent, Creature creature)
        {
            throw new System.NotImplementedException();
        }
    }
}
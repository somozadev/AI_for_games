using UnityEngine;

namespace Genetics
{
        [System.Serializable]
    public class FleeState : BaseState
    {
        public override void EnterState(AgentStateManager agent)
        {
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
using UnityEngine;

namespace Genetics
{
    [System.Serializable]
    public class SearchCreatureState : BaseState
    {
        public SearchCreatureState()
        {
            stateName = "Looking for creatures";
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
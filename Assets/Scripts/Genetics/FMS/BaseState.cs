using UnityEngine;

namespace Genetics
{
    [System.Serializable]
    public abstract class BaseState
    {
        public abstract void EnterState(AgentStateManager agent); 
        public  abstract void UpdateState(AgentStateManager agent);
        public abstract void OnTriggerEnter(Collider other,AgentStateManager agent, Creature creature);

    }
}
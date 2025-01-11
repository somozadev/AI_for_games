using UnityEngine;

namespace Genetics
{
    [System.Serializable]
    public abstract class BaseState
    {
        public string stateName;
        public abstract void EnterState(AgentStateManager agent); 
        public abstract void EnterState(AgentStateManager agent, Collider collidedObject); 
        public  abstract void UpdateState(AgentStateManager agent);
        public abstract void OnTriggerEnter(Collider other,AgentStateManager agent, Creature creature);
        public abstract void OnTriggerStay(Collider other,AgentStateManager agent, Creature creature);

    }
}
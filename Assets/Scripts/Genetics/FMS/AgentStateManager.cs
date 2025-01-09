using System;
using UnityEngine;

namespace Genetics
{
    public class AgentStateManager : MonoBehaviour
    {
        [SerializeField] private CreatureContainer creatureContainer;
        public Creature GetCreature() => creatureContainer.Creature;
        public CreatureContainer GetCreatureContainer() => creatureContainer;

        public BaseState GetCurrentState() => currentState;
        [SerializeField] private BaseState currentState;
        public ExploreState exploreState = new ExploreState();
        public FleeState fleeState = new FleeState();
        public RestState restState = new RestState();
        public EatState eatState = new EatState();
        public SearchFruitState searchFruitState = new SearchFruitState();
        public SearchPlantState searchPlantState = new SearchPlantState();
        public SearchCreatureState searchCreatureState = new SearchCreatureState();
        public SearchCrystalState searchCrystalState = new SearchCrystalState();
        public SearchLuminState searchLuminState = new SearchLuminState();

        private void Awake()
        {
            creatureContainer = GetComponent<CreatureContainer>();
            creatureContainer.onTriggerEnterEvent.AddListener(OnTriggerEnterAgent);
        }

        private void Start()
        {
            currentState = restState;
            currentState.EnterState(this);
        }

        private void Update()
        {
            currentState.UpdateState(this);
        }

        public void SwitchState(BaseState state)
        {
            currentState = state;
            currentState.EnterState(this);
        }

        public void SwitchState(BaseState state, Collider collidedObject)
        {
            currentState = state;
            currentState.EnterState(this, collidedObject);
        }

        private void OnTriggerEnterAgent(Collider other)
        {
            currentState.OnTriggerEnter(other, this, creatureContainer.Creature);
        }
    }
}
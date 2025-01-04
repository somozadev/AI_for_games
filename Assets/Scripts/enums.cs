namespace Genetics
{
    [System.Serializable]
    public static class enums
    {
        [System.Serializable]
        public struct BasicStats
        {
            public int initial_hp;
            public int initial_dmg;
            public int initial_speed;
            public int initial_energy;
            public float initial_perception;

            public int hp;
            public int dmg;
            public int speed;
            public int energy;
            public float perception;

            public int HpLevels => hp / initial_hp * 100;
            public int DmgLevels => dmg / initial_dmg * 100;
            public int SpeedLevels => speed / initial_speed * 100;
            public int EnergyLevels => energy / initial_energy * 100;
            public float perceptionLevels => perception / initial_perception * 100;
            
            public BasicStats(int hp, int dmg, int speed, int energy, float perception)
            {
                initial_hp = this.hp = hp;
                initial_dmg = this.dmg = dmg;
                initial_speed = this.speed = speed;
                initial_energy = this.energy = energy;
                initial_perception = this.perception = perception;
            }
        }

        [System.Serializable]
        public enum TerrainType
        {
            Ground,
            Water,
            Air
        }

        [System.Serializable]
        public enum DietType
        {
            Herbivorous,
            Carnivorous,
            Omnivorous,
            Frugivorous, //only fruits
            Lumivorous, //feeds on light or bioluminiscence
            Crystavorous //feeds on minerals or precious stones 
        }

        [System.Serializable]
        public enum ClimateType
        {
            Neutral,
            Hot,
            Cold,
            Humid,
            Interior
        }

        [System.Serializable]
        public enum SocialType
        {
            Lonely,
            Group,
            Swarm,
        }

        [System.Serializable]
        public enum SpecialSkillsType
        {
            Camuflage,
            Hibernation,
        }
    }
}
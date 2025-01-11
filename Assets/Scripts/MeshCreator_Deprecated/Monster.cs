using System;
using System.Collections.Generic;
using System.Linq;
using MonsterCreator;
using UnityEngine;
using UnityEngine.Serialization;

namespace MeshCreator
{
    public class Monster : MonoBehaviour
    {
       
    }
}

/*
 *  [SerializeField] private BodyInitConfig bodyInitConfig;

        private void Awake()
        {
            bodyInitConfig.monsterBody = GetComponentInChildren<MonsterBody>();
        }

        public void Start()
        {
            for (int i = 0; i < bodyInitConfig.numberOfBones; i++)
            {
                bodyInitConfig.monsterBody.AddBoneToBack();
            }

            bodyInitConfig.monsterBody.AnimationCurve = bodyInitConfig.bodyShapeCurve;
        }

        private void OnValidate()
        {
            if (bodyInitConfig.monsterBody != null)
            {
                bodyInitConfig.bodyShapeCurve = bodyInitConfig._bodyShapeCurve;
            }
        }

        [Serializable]
        public struct BodyInitConfig
        {
            public MonsterBody monsterBody;
            public int numberOfBones;
            [SerializeField] public AnimationCurve _bodyShapeCurve;

            public AnimationCurve bodyShapeCurve
            {
                get { return _bodyShapeCurve; }
                set
                {
                    _bodyShapeCurve = value;
                    if (monsterBody != null)
                    {
                        monsterBody.AnimationCurve = _bodyShapeCurve;
                    }
                }
            }
        }
 */
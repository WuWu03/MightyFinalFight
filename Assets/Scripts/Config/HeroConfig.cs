using System.Collections.Generic;
using UnityEngine;
using System;
namespace Runtime.Config
{
    public class HeroConfig: BaseScriptableObject<HeroData>
    {
    }

    [Serializable]
    public class HeroData: BaseConfigData
    {
        public string Name;
        public string AssetName;
        public string Desc;
        public string HeadIcon;
        public float AttackSpeed;
        public float MoveSpeed;
        public Vector2 JumpForce;
        public int[] Skills;//技能序列
        public float[] AttackWait;//连击时间
    }
}

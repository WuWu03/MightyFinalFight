using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrameWork.Pool
{
    public class GameObjectPool : ResPool<GameObject, GameObjectPool>
    {
        protected override bool m_NeedInstantiate { get { return true; } }

        public override void Put(string resPath, GameObject go)
        {
            go.SetActive(false);
            go.transform.SetParent(m_PoolRoot, false);
            go.transform.localPosition = Vector3.zero;
            base.Put(resPath, go);
        }
    }
}
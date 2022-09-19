using UnityEngine;

namespace GameFrameWork.Resources
{
    public class GameObjectPool : ResourcesPool<GameObject, GameObjectPool>
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
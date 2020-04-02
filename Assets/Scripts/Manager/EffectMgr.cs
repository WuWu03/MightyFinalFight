using FrameWork;
using FrameWork.Pool;
using UnityEngine;

namespace Runtime
{
    public class EffectMgr : BaseMgr<EffectMgr>
    {
        public BaseEffect PlayEffect(string effectName,Transform parent,Vector3 pos,Vector3 angles,bool isAutoPlay,bool isAutoRelease = true,float playTime = -1, VoidNotPar playEndCallback = null)
        {
            BaseEffect effect = ObjectPool.Ins.Get<BaseEffect>(effectName, parent);
            effect.transform.localPosition = pos;
            effect.transform.localRotation = Quaternion.Euler(angles);
            effect.PlayTime = playTime;
            effect.PlayEndCallback = playEndCallback;
            effect.IsAutoRelease = isAutoRelease;
            effect.SetRes(string.Format("{0}/{1}", ResDefine.EFFECT_PATH, effectName));

            if(isAutoPlay)
            {
                effect.Play();
            }
            return effect;
        }
        
        public void PutEffect(BaseEffect effect)
        {
            effect.Release();
            ObjectPool.Ins.Put(effect);
        }

        public override void ShutDown()
        {

        }
    }
}

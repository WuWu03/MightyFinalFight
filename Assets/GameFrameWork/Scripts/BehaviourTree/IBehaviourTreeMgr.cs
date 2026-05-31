using GameFrameWork.Resources;

namespace GameFrameWork.BehaviourTree
{
    public interface IBehaviourTreeMgr
    {
        public void SetResourceMgr(IResourcesMgr  resourceMgr);
        public void AddBehaviourTree(object owner, string dataName);
        public void RemoveBehaviourTree(object owner);
        public void StartAllTrees();
        public void StartTree(object owner);
        public void StopAllTrees();
        public void StopTree(object owner);
        public void PauseAllTrees();
        public void PauseTree(object owner);
        public void ResumeAllTrees();
        public void ResumeTree(object owner);
    }
}
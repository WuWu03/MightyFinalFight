using GameFrameWork.Assets;

namespace GameFrameWork.BehaviourTree
{
    public interface IBehaviourTreeMgr
    {
        public void SetResourceMgr(IResourceMgr  resourceMgr);
        public void InitBehaviourTreeData();
        public void AddBehaviourTree(object owner, int id);
        public void RemoveBehaviourTree(object owner, int id);
        public void StartAllTrees();
        public void StartTree(object owner, int id);
        public void StopAllTrees();
        public void StopTree(object owner, int id);
        public void PauseAllTrees();
        public void PauseTree(object owner, int id);
        public void ResumeAllTrees();
        public void ResumeTree(object owner, int id);
    }
}
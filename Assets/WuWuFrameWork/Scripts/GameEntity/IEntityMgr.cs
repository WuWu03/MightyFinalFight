using System.Collections.Generic;
using WuWuFramework.Pool;
using UnityEngine;

namespace WuWuFramework.GameEntity
{
    public interface IEntityMgr
    {
        public int acquireCount {get; }
        public int createCount {get; }
        public int releaseCount {get; }
        public int destroyCount{get; }
        public int usingEntityCount{get; }
        public int unUsedEntityCount{get; }
        public void SetGameObjectPoolMgr(IGameObjectPoolMgr gameObjectPoolMgr, Transform poolRoot);
        public T GetEntity<T>(string entityName = null, Transform parent = null) where T : BaseEntity, new();
        public void PutEntities(BaseEntity[] entities);
        public void PutEntity(BaseEntity entity);
        public void DestroyEntities(BaseEntity[] entities);
        public void DestroyEntity(BaseEntity entity);
        public void DestroyAllUnUsedEntities();
        public void DestroyAll();
        public List<T> FindEntities<T>(string entityName) where T : BaseEntity;
        public void FindEntities<T>(string entityName, List<T> entities) where T : BaseEntity;
        public T FindEntity<T>(string entityName) where T : BaseEntity;
        public bool HasEntity<T>(string entityName) where T : BaseEntity;
    }
}
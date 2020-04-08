using FrameWork;
using System.Collections.Generic;
using UnityEngine;

namespace Runtime
{
    public delegate void MsgHandle(BaseData data);
    public class ObjectMsgCenter:BaseMgr<ObjectMsgCenter>
    {
        private void Awake()
        {
            m_DicObjectMsg = new Dictionary<string, List<ObjectMsg>>();
        }

        public void AddMsg(string msgName, string ownerName, MsgHandle handle)
        {
            List<ObjectMsg> list = null;
            if (!m_DicObjectMsg.TryGetValue(msgName, out list))
            {
                list = new List<ObjectMsg>();
                m_DicObjectMsg.Add(msgName, list);
            }

            bool exist = false;

            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].OwnerName.Equals(ownerName) && list[i].Handle.Equals(handle))
                {
                    exist = true;
                    break;
                }
            }

            if (!exist)
                list.Add(new ObjectMsg(ownerName, handle));
        }

        public bool RemoveMsg(string msgName, string ownerName,MsgHandle handle)
        {
            List<ObjectMsg> list = null;

            if (!m_DicObjectMsg.TryGetValue(msgName, out list))
            {
                return false;
            }

            bool exist = false;

            for (int i = list.Count - 1; i > -1; i--)
            {
                if (list[i].OwnerName.Equals(ownerName) && list[i].Handle.Equals(handle))
                {
                    list[i].Destroy();
                    list.RemoveAt(i);
                    exist = true;
                    break;
                }
            }

            if(list.Count < 1)
            {
                m_DicObjectMsg.Remove(msgName);
            }

            return exist;
        }

        public void SendMsg(BaseData msgData)
        {
            if(msgData == null || !msgData.CanSend)
            {
                Debug.LogError(msgData != null ? msgData.ToString() : "Msg is invalid");
                return;
            }

            List<ObjectMsg> list = null;
            if (!m_DicObjectMsg.TryGetValue(msgData.DataName, out list))
            {
                Debug.LogError("Don't have such msg:" + msgData.ToString());
                return;
            }

            for (int i = 0; i < list.Count; i++)
            {
                if(list[i].OwnerName.Equals(msgData.Receiver))
                {
                    list[i].Handle?.Invoke(msgData);
                }
            }
        }

        public override void ShutDown()
        {
            throw new System.NotImplementedException();
        }

        private Dictionary<string, List<ObjectMsg>> m_DicObjectMsg = null;
    }

    class ObjectMsg
    {
        public ObjectMsg(string ownerName,MsgHandle handle)
        {
            m_OwnerName = ownerName;
            m_Handle = handle;
        }

        public string OwnerName
        {
            get
            {
                return m_OwnerName;
            }
        }

        public MsgHandle Handle
        {
            get
            {
                return m_Handle;
            }
        }

        public void Destroy()
        {
            m_OwnerName = string.Empty;
            m_Handle = null;
        }

        private string m_OwnerName = string.Empty;
        private MsgHandle m_Handle = null;
    }
}

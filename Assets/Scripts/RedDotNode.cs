using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using UnityEngine;
namespace Kultie.Notification
{
    public class RedDotNode
    {
        public string rdName { get; set; }

        public int rdCount { get; private set; }
        public RedDotNode parent;
        public RedDotSystem.OnRdCountChange countChangeFunc;
        public Dictionary<string, RedDotNode> rdChildrenDic = new Dictionary<string, RedDotNode>();


        #region Internal interface
        private void CheckRedDotCount()
        {
            int num = 0;
            foreach (RedDotNode node in rdChildrenDic.Values)
                num += node.rdCount;
            if (num != rdCount)
            {
                rdCount = num;
                NotifyRedDotCountChange();
            }

            parent?.CheckRedDotCount();
        }

        private void NotifyRedDotCountChange()
        {
            countChangeFunc?.Invoke(this);
        }

        #endregion

        #region External interface

        public void SetRedDotCount(int rdCount)
        {
            if (rdChildrenDic.Count > 0)
            {
                return;
            }
            this.rdCount = rdCount;

            NotifyRedDotCountChange();

            parent?.CheckRedDotCount();
        }

        public JObject GetData()
        {
            JObject result = new JObject();

            result["value"] = rdCount;
            foreach (var kv in rdChildrenDic)
            {
                result[kv.Key] = kv.Value.GetData();
            }
            return result;
        }

        #endregion
    }
}
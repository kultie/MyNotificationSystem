using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Kultie.Notification
{
    public class RedDotSystem
    {
        private static RedDotSystem _instance;
        public static RedDotSystem Instance => _instance;
        private static List<string> lstRedDotTreeList;
        private ISave _saveService;
        string Root => lstRedDotTreeList[0];

        public RedDotSystem(ISave saveService)
        {
            _saveService = saveService;
            _instance = this;
            InitRedDotTreeNode();
        }

        public RedDotSystem(string[] keys, ISave saveService)
        {
            _saveService = saveService;
            _instance = this;
            for (int i = 0; i < keys.Length; i++)
            {
                keys[i] = keys[i].Insert(0, "Root/");
            }

            var _keys = new List<string>(keys);
            _keys.Insert(0, "Root");


            lstRedDotTreeList = new List<string>(_keys);


            InitRedDotTreeNode();
            // SaveLoadManager.onClearData += ClearData;
        }

        public void ClearData()
        {
            InitRedDotTreeNode();
        }

        public delegate void OnRdCountChange(RedDotNode node);

        private RedDotNode mRootNode;

        #region Internal interface

        private void InitRedDotTreeNode()
        {
            mRootNode = new RedDotNode { rdName = Root };

            foreach (string path in lstRedDotTreeList)
            {
                string[] treeNodeAy = path.Split('/');
                int nodeCount = treeNodeAy.Length;
                RedDotNode curNode = mRootNode;

                if (treeNodeAy[0] != mRootNode.rdName)
                {
                    continue;
                }

                if (nodeCount > 1)
                {
                    for (int i = 1; i < nodeCount; i++)
                    {
                        if (!curNode.rdChildrenDic.ContainsKey(treeNodeAy[i]))
                        {
                            curNode.rdChildrenDic.Add(treeNodeAy[i], new RedDotNode());
                        }

                        curNode.rdChildrenDic[treeNodeAy[i]].rdName = treeNodeAy[i];
                        curNode.rdChildrenDic[treeNodeAy[i]].parent = curNode;

                        curNode = curNode.rdChildrenDic[treeNodeAy[i]];
                    }
                }
            }

            LoadData();
        }

        public void SaveData()
        {
            JObject dataToSave = new JObject();
            dataToSave["Root"] = mRootNode.GetData();
            _saveService.SaveData(dataToSave, true);
        }

        public void LoadData()
        {
            var loadData = _saveService.LoadData();
            if (loadData != null)
            {
                SetNodeData(string.Empty, loadData);
            }
        }

        private void SetNodeData(string path, JToken saveData)
        {
            var data = saveData as JObject;
            foreach (var kv in data)
            {
                if (kv.Value.Type == JTokenType.Object)
                {
                    string childPath = string.IsNullOrEmpty(path) ? kv.Key : (path + "/" + kv.Key);
                    SetNodeData(childPath, kv.Value.Value<JObject>());
                }
                else if (kv.Value.Type == JTokenType.Integer)
                {
                    Set(path, kv.Value.Value<int>());
                }
            }
        }

        #endregion

        #region External interface

        public void AddRedDotCallback(string strNode, OnRdCountChange callBack)
        {
            var nodeList = strNode.Split('/');

            if (nodeList.Length == 1)
            {
                if (nodeList[0] != Root)
                {
                    Debug.LogWarning("Get Wrong Root Node! current is " + nodeList[0]);
                    return;
                }
            }

            var node = mRootNode;
            for (int i = 1; i < nodeList.Length; i++)
            {
                if (!node.rdChildrenDic.ContainsKey(nodeList[i]))
                {
                    Debug.LogWarning("Does Not Contain child Node: " + nodeList[i]);
                    return;
                }

                node = node.rdChildrenDic[nodeList[i]];

                if (i == nodeList.Length - 1)
                {
                    node.countChangeFunc += callBack;
                    return;
                }
            }
        }

        public bool IsPathValid(string strNode)
        {
            var nodeList = strNode.Split('/');

            if (nodeList.Length == 1)
            {
                if (nodeList[0] != Root)
                {
                    return false;
                }
            }

            var node = mRootNode;
            for (int i = 1; i < nodeList.Length; i++)
            {
                if (!node.rdChildrenDic.ContainsKey(nodeList[i]))
                {
                    return false;
                }

                node = node.rdChildrenDic[nodeList[i]];

                if (i == nodeList.Length - 1)
                {
                    return true;
                }
            }

            return false;
        }

        public void RemoveRedDotCallback(string strNode, OnRdCountChange callBack)
        {
            var nodeList = strNode.Split('/');

            if (nodeList.Length == 1)
            {
                if (nodeList[0] != Root)
                {
                    Debug.LogWarning("Get Wrong Root Node! current is " + nodeList[0]);
                    return;
                }
            }

            var node = mRootNode;
            for (int i = 1; i < nodeList.Length; i++)
            {
                if (!node.rdChildrenDic.ContainsKey(nodeList[i]))
                {
                    Debug.LogWarning("Does Not Contain child Node: " + nodeList[i]);
                    return;
                }

                node = node.rdChildrenDic[nodeList[i]];

                if (i == nodeList.Length - 1)
                {
                    node.countChangeFunc -= callBack;
                    return;
                }
            }
        }

        public void Set(string nodePath, int rdCount = 1, bool autoSave = true)
        {
            string[] nodeList = nodePath.Split('/');

            if (nodeList.Length == 1)
            {
                if (nodeList[0] != Root)
                {
                    Debug.Log("Get Wrong RootNod? current is " + nodeList[0]);
                    return;
                }
            }


            RedDotNode node = mRootNode;
            for (int i = 1; i < nodeList.Length; i++)
            {
                if (node.rdChildrenDic.ContainsKey(nodeList[i]))
                {
                    node = node.rdChildrenDic[nodeList[i]];

                    if (i == nodeList.Length - 1)
                    {
                        node.SetRedDotCount(Math.Max(0, rdCount));
                    }
                }
                else
                {
                    break;
                }
            }

            if (autoSave)
            {
                SaveData();
            }
        }

        public int GetRedDotCount(string nodePath)
        {
            string[] nodeList = nodePath.Split('/');

            int count = 0;
            if (nodeList.Length >= 1)
            {
                RedDotNode node = mRootNode;
                for (int i = 1; i < nodeList.Length; i++)
                {
                    if (node.rdChildrenDic.ContainsKey(nodeList[i]))
                    {
                        node = node.rdChildrenDic[nodeList[i]];

                        if (i == nodeList.Length - 1)
                        {
                            count = node.rdCount;
                            break;
                        }
                    }
                }
            }

            return count;
        }

        #endregion

        public interface ISave
        {
            void SaveData(JObject data, bool writeImmediately);
            JObject LoadData();
            void WriteData();
            void ClearData();
        }
    }
}
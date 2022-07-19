using System;
using System.Collections.Generic;
using UnityEngine;

namespace Kultie.Notification
{

    public class RedDotSystem
    {
        private static List<string> lstRedDotTreeList;
        string Root => lstRedDotTreeList[0];
        public RedDotSystem()
        {
            InitRedDotTreeNode();
        }

        public RedDotSystem(string[] keys)
        {
            lstRedDotTreeList = new List<string>(keys);
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
        }

        #endregion

        #region External interface
        public void SetRedDotNodeCallBack(string strNode, OnRdCountChange callBack)
        {
            var nodeList = strNode.Split('/');

            if (nodeList.Length == 1)
            {
                if (nodeList[0] != Root)
                {
                    Debug.LogError("Get Wrong Root Node! current is " + nodeList[0]);
                    return;
                }
            }

            var node = mRootNode;
            for (int i = 1; i < nodeList.Length; i++)
            {
                if (!node.rdChildrenDic.ContainsKey(nodeList[i]))
                {
                    Debug.LogError("Does Not Contain child Node: " + nodeList[i]);
                    return;
                }

                node = node.rdChildrenDic[nodeList[i]];

                if (i == nodeList.Length - 1)
                {
                    node.countChangeFunc = callBack;
                    return;
                }
            }
        }

        public void Set(string nodePath, int rdCount = 1)
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
                    return;
                }
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
    }
}
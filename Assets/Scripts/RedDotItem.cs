using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Kultie.Notification
{
    public class RedDotItem : MonoBehaviour
    {
        public string nodePath;
        public GameObject dotObj;
        public Text dotCountText;
        public UnityEvent<RedDotNode> onTrigger;
        private void Start()
        {
            string _nodePath = "Root/" + nodePath;
            RedDotNotification.System.SetRedDotNodeCallBack(_nodePath, OnRedDotCallback);
            int count = RedDotNotification.System.GetRedDotCount(_nodePath);
            dotObj.SetActive(count > 0);
            dotCountText.text = count.ToString();
        }

        private void OnRedDotCallback(RedDotNode node)
        {
            dotObj.SetActive(node.rdCount > 0);
            dotCountText.text = node.rdCount.ToString();
            onTrigger?.Invoke(node);
        }
    }
}
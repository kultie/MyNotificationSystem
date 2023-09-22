using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Kultie.Notification
{
    public class RedDotItem : MonoBehaviour
    {
        public string nodePath;
        public GameObject dotObj;
        public TMPro.TextMeshProUGUI dotCountText;
        public UnityEvent<RedDotNode> onTrigger;
        private bool hasUpdateNodePath;
        private string _currentNodePath => $"Root/{nodePath}";

        private void Start()
        {
            if (!hasUpdateNodePath)
            {
                Refresh();
            }
        }

        void Refresh()
        {
            dotObj.SetActive(false);
            if (dotCountText)
            {
                dotCountText.text = string.Empty;
            }

            if (string.IsNullOrEmpty(nodePath)) return;
            if (!RedDotSystem.Instance.IsPathValid(_currentNodePath))
            {
                return;
            }

            RedDotSystem.Instance.AddRedDotCallback(_currentNodePath, OnRedDotCallback);
            int count = RedDotSystem.Instance.GetRedDotCount(_currentNodePath);
            dotObj.SetActive(count > 0);
            if (dotCountText)
            {
                dotCountText.text = count.ToString();
            }
        }

        public void UpdateNodePath(string newPath)
        {
            nodePath = newPath;
            if (!RedDotSystem.Instance.IsPathValid(_currentNodePath))
            {
                return;
            }

            if (!string.IsNullOrEmpty(nodePath))
            {
                RedDotSystem.Instance.RemoveRedDotCallback(_currentNodePath, OnRedDotCallback);
            }
            hasUpdateNodePath = true;
            Refresh();
        }

        private void OnDestroy()
        {
            if (!string.IsNullOrEmpty(nodePath))
            {
                if (!RedDotSystem.Instance.IsPathValid(_currentNodePath))
                {
                    return;
                }

                RedDotSystem.Instance.RemoveRedDotCallback(_currentNodePath, OnRedDotCallback);
            }
        }

        private void OnRedDotCallback(RedDotNode node)
        {
            dotObj.SetActive(node.rdCount > 0);
            if (dotCountText)
            {
                dotCountText.text = node.rdCount.ToString();
            }

            onTrigger?.Invoke(node);
        }

        public void SetValue(int value = 1)
        {
            RedDotSystem.Instance.Set("Root/" + nodePath, value);
        }
    }
}
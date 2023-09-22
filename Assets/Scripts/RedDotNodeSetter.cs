using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Kultie.Notification
{
    public class RedDotNodeSetter : MonoBehaviour
    {
        [SerializeField]
        string nodePath;
        public void SetValue(int value)
        {
            RedDotSystem.Instance.Set("Root/"+nodePath, value);
        }
    }
}
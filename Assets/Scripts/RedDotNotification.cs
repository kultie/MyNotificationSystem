using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Kultie.Notification
{
    public class RedDotNotification : MonoBehaviour
    {
        public List<string> RedDotKey;
        private static RedDotSystem system;
        public static RedDotSystem System => system;
        private void Awake()
        {
            for (int i = 0; i < RedDotKey.Count; i++) {
                RedDotKey[i] = RedDotKey[i].Insert(0, "Root/");
            }
            var _keys = new List<string>(RedDotKey);
            _keys.Insert(0, "Root");
            system = new RedDotSystem(_keys.ToArray());
        }

    }
}
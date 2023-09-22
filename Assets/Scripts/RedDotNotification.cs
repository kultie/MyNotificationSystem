using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kultie.Notification
{
    public class RedDotNotification : MonoBehaviour
    {
        public List<string> RedDotKey;
        private static RedDotSystem system;
        [SerializeReference] private RedDotSystem.ISave _saveService;

        private void Awake()
        {
            if (system == null)
            {
                var keys = new List<string>(RedDotKey);
                system = new RedDotSystem(keys.ToArray(), _saveService);
            }
        }

        [Button]
        public void SetTrigger(string key, int value)
        {
            system.Set(key, value);
        }

        [Button]
        public void TestLoad()
        {
            system.LoadData();
        }
    }
}
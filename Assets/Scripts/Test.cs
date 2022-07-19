using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Kultie.Notification.Test
{
    public class Test : MonoBehaviour
    {
        public string path;
        public int count;

        /// <summary>
        /// Root
        /// Root/Mail
        /// Root/Mail/System
        /// Root/Mail/Team
        /// </summary>
        public void SetNotePath() {
            RedDotNotification.System.Set("Root/"+path, count);
        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Managers
{
    public class EventHandler : ManagerBase<EventHandler>
    {
        public void FireEvent(Action func)
        {
            if (func != null) func();
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Managers
{
    public class InventoryManager : ManagerBase<InventoryManager>
    {
        private float funds = 100.0f;

        public float Funds
        {
            get
            {
                return funds;
            }
        }

        //Planets cost more based on how massive they are.
        //Each planet has a price per mass ratio.


    }
}

using UnityEngine;
using Modding;
using System.IO;
using System;

namespace HPBar
{
    class HPBarComponent : MonoBehaviour
    {
        private static GameManager gm;
        private static UIManager uim;

        public void Start()
        {
            gm = GameManager.instance;
            uim = UIManager.instance;
        }

        public void Update()
        {
            if (HPBar.bossFSM != null)
            {
                Modding.Logger.Log(HPBar.bossFSM.FsmVariables.GetFsmInt("HP").Value);
                
            }
        }
    }
}

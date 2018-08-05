using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestPlugin
{
    class AI
    {
        enum STATE
        {
            IDLE,
            CHASE,
            RUN,
        }
        public MonsterInfo _info;
        public bool isAlive = false;
        StateManager<STATE> m_statemanager;
        public void Update(object _plugin)
        {
            RaiseEventTestPlugin RETP = (RaiseEventTestPlugin)_plugin;
            isAlive = true;
            m_statemanager = new StateManager<STATE>();
            m_statemanager.CurrstateReqUpdate = true;
            m_statemanager.AddUpdateFunction(STATE.IDLE, Idle);
            m_statemanager.AddUpdateFunction(STATE.CHASE, Chase);
            m_statemanager.AddUpdateFunction(STATE.RUN, Run);
            m_statemanager.Currstate = m_statemanager.Prevstate = STATE.IDLE;
            while (isAlive)
            {
                //run AI Code
                m_statemanager.Update();
                RETP.SendMessage(103, MonsterInfo.Serialize(_info));
                System.Threading.Thread.Sleep(500);
            }
        }

        public void Idle()
        {
            _info.posx += 1;
        }

        public void Chase()
        {

        }

        public void Run()
        {

        }
    }
}

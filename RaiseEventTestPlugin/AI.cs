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
            ROAM,
            ATTACK,
        }
        public MonsterInfo _info;
        public MonsterInfo _newinfo;


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
            m_statemanager.AddUpdateFunction(STATE.ROAM, Roam);
            m_statemanager.AddUpdateFunction(STATE.ATTACK, Attack);

            m_statemanager.Currstate = m_statemanager.Prevstate = STATE.IDLE;
            while (isAlive)
            {
                //run AI Code
                lock (_info)
                {
                    m_statemanager.Update();
                    RETP.SendMessage(103, MonsterInfo.Serialize(_info));
                    if (_newinfo != null)
                    {
                        lock (_newinfo)
                        {

                            _info.playerposx = _newinfo.playerposx;
                            _info.playerposz = _newinfo.playerposz;
                            _info.playerhp = _newinfo.playerhp;
                            _info.hp = _newinfo.hp;
                            _newinfo = null;
                        }
                    }
                }
                CustomObject co = new CustomObject();
                co.Init();
                co.message = "Update Success " + m_statemanager.Currstate.ToString();
                RETP.SendMessage(169,
                CustomObject.Serialize(co)
);
                System.Threading.Thread.Sleep(500);


            }
        }

        public void Idle()
        {
            _info.newposx = _info.posx;
            _info.newposz = _info.posz;

            if (_info.posx > _info.playerposx - 20 && _info.posx < _info.playerposx + 20
                && _info.posz > _info.playerposz - 20 && _info.posz < _info.playerposz + 20)
                 m_statemanager.Currstate = STATE.CHASE;
            else
            {
                Random rand = new Random();

                if (rand.Next(0, 100) > 60)
                {
                    m_statemanager.Currstate = STATE.ROAM;
                    _info.newposx = _info.posx + rand.Next(-10, 10);
                    _info.newposz = _info.posx + rand.Next(-10, 10);

                }
            }

        }

        public void Chase()
        {
            if (!(_info.posx > _info.playerposx - 10 && _info.posx < _info.playerposx + 10
               && _info.posz > _info.playerposz - 10 && _info.posz < _info.playerposz + 10))
            {
                m_statemanager.Currstate = STATE.ROAM;
            }
            else
            {
                if (_info.posx > _info.playerposx - 1 && _info.posx < _info.playerposx + 1
              && _info.posz > _info.playerposz - 1 && _info.posz < _info.playerposz + 1)
                {
                    m_statemanager.Currstate = STATE.ATTACK;
                }
                else
                {
                    _info.posx += ((_info.playerposx - _info.posx) < 0 ? -0.1f : 0.1f);
                    _info.posz += ((_info.playerposz - _info.posz) < 0 ? -0.1f : 0.1f);
                }
            }
        }

        public void Roam()
        {
            if (_info.posx > _info.playerposx - 10 && _info.posx < _info.playerposx + 10
               && _info.posz > _info.playerposz - 10 && _info.posz < _info.playerposz + 10)
                m_statemanager.Currstate = STATE.CHASE;

            else
            {
                _info.posx += ((_info.newposx - _info.posx) < 0 ? -0.1f : 0.1f);
                _info.posz += ((_info.newposz - _info.posx) < 0 ? -0.1f : 0.1f);

                if (_info.posx > _info.newposx - 5 && _info.posx < _info.newposx + 5
               && _info.posz > _info.newposz - 5 && _info.posz < _info.newposz + 5)
                {
                    m_statemanager.Currstate = STATE.IDLE;
                }
            }
        }

        public void Attack()
        {
            if ((_info.posx > _info.playerposx - 1 && _info.posx < _info.playerposx + 1
              && _info.posz > _info.playerposz - 1 && _info.posz < _info.playerposz + 1))
            {
                _info.playerhp -= 1;
            }
            else
            {
                m_statemanager.Currstate = STATE.CHASE;
            }
        }
    }
}

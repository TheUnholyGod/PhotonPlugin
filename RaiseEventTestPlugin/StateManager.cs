using System.Collections;
using System.Collections.Generic;

public class StateManager<State> where State : struct, System.IConvertible
{
    State m_prevstate;
    State m_currstate;
    public delegate void ChangeStateTrigger();
    private Dictionary<int, ChangeStateTrigger> m_changeStateFunctions = new Dictionary<int, ChangeStateTrigger>();
    private Dictionary<State, ChangeStateTrigger> m_stateUpdateFunctions = new Dictionary<State, ChangeStateTrigger>();
    bool m_currstateReqUpdate = false;

    public State Prevstate
    {
        get
        {
            return m_prevstate;
        }

        set
        {
            m_prevstate = value;
        }
    }

    public State Currstate
    {
        get
        {
            return m_currstate;
        }

        set
        {
            m_currstate = value;
        }
    }

    public bool CurrstateReqUpdate
    {
        get
        {
            return m_currstateReqUpdate;
        }

        set
        {
            m_currstateReqUpdate = value;
        }
    }

    public void AddFunction(int _key, ChangeStateTrigger _func)
    {
        m_changeStateFunctions.Add(_key, _func);
    }

    public void AddUpdateFunction(State _key, ChangeStateTrigger _func)
    {
        m_stateUpdateFunctions.Add(_key, _func);
    }

    public void Update()
    {
        if (m_currstateReqUpdate)
            m_stateUpdateFunctions[m_currstate]();
        if (m_currstate.GetHashCode() != m_prevstate.GetHashCode())
        {
            int stateChange = StateDoubleKey<State>.GetHashValue(m_prevstate, m_currstate);
            if (m_changeStateFunctions.ContainsKey(stateChange))
            {
                m_changeStateFunctions[stateChange]();
            }
            m_prevstate = m_currstate;
            if (m_stateUpdateFunctions.ContainsKey(m_currstate))
                m_currstateReqUpdate = true;
            else
                m_currstateReqUpdate = false;
        }
    }

}

struct StateDoubleKey<State> where State : struct, System.IConvertible
{
    State prevstate;
    State currstate;

    public StateDoubleKey(State _prev, State _curr)
    {
        currstate = _curr;
        prevstate = _prev;
    }

    public override bool Equals(object obj)
    {
        StateDoubleKey<State> other = (StateDoubleKey<State>)obj;
        if (other.prevstate.ToString() == prevstate.ToString() && other.currstate.ToString() == currstate.ToString())
            return true;

        return false;
    }

    public override int GetHashCode()
    {
        int somePrimeNumer = 7919;
        return currstate.GetHashCode() * somePrimeNumer + prevstate.GetHashCode();
    }

    public static int GetHashValue(State _prev, State _curr)
    {
        int somePrimeNumer = 7919;
        return _curr.GetHashCode() * somePrimeNumer + _prev.GetHashCode();
    }
}

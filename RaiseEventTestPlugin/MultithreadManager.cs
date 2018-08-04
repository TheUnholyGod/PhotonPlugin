using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace TestPlugin
{
    /// <summary>
    /// Handles multiple threads for the application and automatically closes threads when done
    /// </summary>
    class MultithreadManager : Singleton<MultithreadManager>
    {
        Dictionary<string, ThreadObject> m_threads = new Dictionary<string, ThreadObject>();

        /// <summary>
        /// Adds a thread to the manager [This function does not allow parameters]
        /// </summary>
        /// <param name="_name"></param>
        /// <param name="_func"></param>
        /// <returns>
        /// True:
        ///     Adding thread was successful
        /// False:
        ///     The dictionary already contains that name
        /// </returns>
        public bool AddThread(string _name,ThreadStart _func)
        {
            if (m_threads.ContainsKey(_name))
                return false;
            Thread _t = new Thread(_func);
            m_threads.Add(_name, new ThreadObject(_t));
            return true;
        }

        /// <summary>
        /// Adds a thread to the manager [This function allows parameters]
        /// </summary>
        /// <param name="_name"></param>
        /// <param name="_func"></param>
        /// <returns>
        /// True:
        ///     Adding thread was successful
        /// False:
        ///     The dictionary already contains that name
        /// </returns>
        public bool AddThread(string _name, ThreadStart _func, object _params)
        {
            if (m_threads.ContainsKey(_name))
                return false;
            Thread _t = new Thread(_func);
            m_threads.Add(_name, new ThreadObject(_t, _params));
            return true;
        }

        /// <summary>
        /// Starts a thread with the coresponding name
        /// </summary>
        /// <param name="_name"></param>
        /// <returns>
        /// True:
        ///     Thread has been successfully started
        /// False:
        ///     Thread does not exists/ thread already running
        /// </returns>
        public bool StartThread(string _name)
        {
            if (!m_threads.ContainsKey(_name))
                return false;

            ThreadObject _thread = m_threads[_name];
            if (_thread.IsRunning)
                return false;

            return _thread.Start();
        }

        /// <summary>
        /// Stops a thread with the coresponding name
        /// </summary>
        /// <param name="_name"></param>
        /// <returns></returns>
        public bool AbortThread(string _name)
        {
            if (!m_threads.ContainsKey(_name))
                return false;

            ThreadObject _thread = m_threads[_name];
            if (!_thread.IsRunning)
                return false;

            return _thread.Start();
        }
    }
    
    /// <summary>
    /// Thread object that holds information regarding a specific thread
    /// </summary>
    class ThreadObject
    {
        Thread m_thread;
        bool m_isRunning;
        bool m_hasParameters;
        object m_parameter;

        public bool IsRunning
        {
            get
            {
                return m_isRunning;
            }

            set
            {
                m_isRunning = value;
            }
        }

        public ThreadObject(Thread _thread)
        {
            m_thread = _thread;
            m_hasParameters = false;
            Init();
        }

        public ThreadObject(Thread _thread, object _params)
        {
            m_thread = _thread;
            m_parameter = _params;
            m_hasParameters = true;
            Init();
        }

        void Init()
        {
            m_isRunning = false;
        }

        public bool Start()
        {
            if (m_isRunning)
                return false;

            if (!m_hasParameters)
                m_thread.Start();
            else
                m_thread.Start(m_parameter);

            m_isRunning = true;

            return true;
        }

        public bool Abort()
        {
            if (!m_isRunning)
                return false;

            m_thread.Abort();
            m_isRunning = false;
            return true;
        }
    }
}

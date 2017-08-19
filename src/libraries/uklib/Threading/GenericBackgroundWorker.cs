using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace UKLib
{
    public class RunWorkerCompletedEventArgs<T>
    {
        public RunWorkerCompletedEventArgs() { }
        public RunWorkerCompletedEventArgs(RunWorkerCompletedEventArgs e)
        {
            // TODO change in internal hold of e and then parse in get properties
            this.cancelled = e.Cancelled; 
            this.error = e.Error;
            if (!this.cancelled & this.error == null)
                this.result = (T)e.Result;
                            
            this.userState = e.UserState;
        }

        protected bool cancelled;
        public bool Cancelled { get { return cancelled; } }
        protected Exception error;
        public Exception Error { get { return error; } }
        protected T result;
        public T Result { get { return result; } }
        protected object userState;
        public object UserState { get { return userState; } }
    }

    public class RunWorkerCompletedEventArgs<Targ, Tresult>
    {
        public RunWorkerCompletedEventArgs() { }
        public RunWorkerCompletedEventArgs(RunWorkerCompletedEventArgs e)
        {
            // TODO change in internal hold of e and then parse in get properties
            this.cancelled = e.Cancelled; 
            this.error = e.Error;

            if (!this.cancelled & this.error == null)
                this.result = (Targ)e.Result;

            this.userState = (Tresult)e.UserState;
        }

        protected bool cancelled;
        public bool Cancelled { get { return cancelled; } }
        protected Exception error;
        public Exception Error { get { return error; } }
        protected Targ result;
        public Targ Result { get { return result; } }
        protected Tresult userState;
        public Tresult UserState { get { return userState; } }
    }

    public class DoWorkEventArgs<T>
    {
        public DoWorkEventArgs() { }
        public DoWorkEventArgs(DoWorkEventArgs e) { this.argument = e.Argument; this.Cancel = e.Cancel; this.Result = (T)e.Result; }

        protected object argument;
        public object Argument { get { return argument; } }
        public bool Cancel { get; set; }
        public T Result { get; set; }
    }

    public class DoWorkEventArgs<Targ, Tresult>
    {
        public DoWorkEventArgs() { }
        public DoWorkEventArgs(DoWorkEventArgs e) { if (e.Argument != null) this.argument = (Targ)e.Argument; this.Cancel = e.Cancel; if (e.Result != null) this.Result = (Tresult)e.Result; }

        protected Targ argument;
        public Targ Argument { get { return argument; } }
        public bool Cancel { get; set; }
        public Tresult Result { get; set; }
    }

    [Obsolete("Use BackgroundWorker<Targ, Tresult> instead, fix workaround  BackgroundWorker<object, Tresult> ", true)]
    public class BackgroundWorker<T> : BackgroundWorker
    {
        public BackgroundWorker()
        {
            base.DoWork += new DoWorkEventHandler(BackgroundWorker_DoWork);
            base.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BackgroundWorker_RunWorkerCompleted);
        }

        private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            DoWorkEventArgs<T> eNew = new DoWorkEventArgs<T>(e);
            DoWork(sender, eNew);
            e.Result = eNew.Result;
            e.Cancel = eNew.Cancel;
        }

        private void BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            RunWorkerCompleted(sender, new RunWorkerCompletedEventArgs<T>(e));
        }

        new public event DoWorkEventHandler<DoWorkEventArgs<T>> DoWork;
        new public event RunWorkerCompletedEventHandler<RunWorkerCompletedEventArgs<T>> RunWorkerCompleted;
    }

    public class BackgroundWorker<Targ, Tresult> : BackgroundWorker
    {
        public BackgroundWorker()
        {
            base.DoWork += new DoWorkEventHandler(BackgroundWorker_DoWork);
            base.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BackgroundWorker_RunWorkerCompleted);
        }

        private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (DoWork != null)
            {
                DoWorkEventArgs<Targ, Tresult> eNew = new DoWorkEventArgs<Targ, Tresult>(e);
                DoWork(sender, eNew);
                e.Result = eNew.Result;
                e.Cancel = eNew.Cancel;
            }
        }

        public void RunWorkerAsync(Targ argument)
        {
            base.RunWorkerAsync(argument);
        }

        private void BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {            
            RunWorkerCompleted(sender, new RunWorkerCompletedEventArgs<Tresult>(e));
        }

        new public event DoWorkEventHandler<DoWorkEventArgs<Targ, Tresult>> DoWork;
        new public event RunWorkerCompletedEventHandler<RunWorkerCompletedEventArgs<Tresult>> RunWorkerCompleted;
    }

    public delegate void DoWorkEventHandler<T>(object sender, T e);
    public delegate void DoWorkEventHandler<Targ, Tresult>(object sender, Targ e);
    public delegate void RunWorkerCompletedEventHandler<T>(object sender, T e);
}

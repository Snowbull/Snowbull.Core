using System;
using System.Reflection;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Snowbull.API {
    public abstract class Plugin<TContext> : IPlugin where TContext : class, IContext {
        private readonly Dictionary<Type, MethodInfo> events = new Dictionary<Type, MethodInfo>();
        private readonly Dictionary<Type, MethodInfo> packets = new Dictionary<Type, MethodInfo>();
        private Events.IEvent raised = null;
        private bool cancelled = false;
        private TContext context = null;
        internal Task deferred = null;

        public virtual string Name {
            get {
                throw new NotImplementedException("Please override the Name property in your plugin.");
            }
        }

        public virtual Guid UID {
            get {
                throw new NotImplementedException("Please override the UID property in your plugin.");
            }
        }

        public virtual Version Version {
            get {
                throw new NotImplementedException("Please override the Version property in your plugin.");
            }
        }

        protected TContext Context {
            get {
                if(context != null) return context;
                throw new NotSupportedException("Plugin currently has no context. This is most likely caused by using an asynchronous operation without using Defer().");
            }
        }

        protected void On<TEvent>(Action<TEvent> action) where TEvent : class, Events.IEvent {
            events[typeof(TEvent)] = action.Method;
        }

        protected void Handle<TPacket>(Action<TPacket> handler) where TPacket : class, Packets.IReceivePacket {
            packets[typeof(TPacket)] = handler.Method;
        }

        internal void Raise(TContext context, Events.IEvent e) {
            MethodInfo handler = events[e.GetType()];
            if(handler != null) {
                raised = e;
                this.context = context;
                handler.Invoke(this, new[] { e });
                this.context = null;
                raised = null;
            }
        }

        internal bool Raise(TContext context, Events.ICancellableEvent e) {
            cancelled = false;
            MethodInfo handler = events[e.GetType()];
            if(handler != null) {
                raised = e;
                this.context = context;
                handler.Invoke(this, new[] { e });
                this.context = null;
                raised = null;
            }
            return cancelled;
        }

        protected void Cancel() {
            if(raised != null) {
                if(raised is Events.ICancellableEvent) {
                    cancelled = true;
                }else{
                    throw new NotSupportedException("A non-cancellable event cannot be cancelled.");
                }
            }else{
                throw new NotSupportedException("Plugin currently has no event. This is most likely caused by using an asynchronous operation without using Defer().");
            }
        }

        internal void Received(TContext context, Packets.ISendPacket p) {
            this.context = context;
            events[p.GetType()].Invoke(this, new [] { p });
            this.context = null;
        }

        protected void Defer<TReturn>(Task<TReturn> task) {
            deferred = task.ContinueWith(t => t);
        }
    }
}


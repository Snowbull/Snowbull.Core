using System;
using Akka.Actor;
using System.Collections.Generic;

namespace Snowbull.API.Observer {
	internal sealed class ObservableActor : ReceiveActor {
		private IActorRef oparent;
		private List<IActorRef> observers = new List<IActorRef>();
		private Dictionary<Events.CancellableEvent, Poll> polls = new Dictionary<Events.CancellableEvent, Poll>();
		private Dictionary<IActorRef, IObservable> children = new Dictionary<IActorRef, IObservable>();

		private Observable Observable {
			get;
			set;
		}

		public static Props Props(Observable observable, IActorRef parent) {
			return Akka.Actor.Props.Create(() => new ObservableActor(observable, parent));
		}

		protected override void PreStart() {
			base.PreStart();
			if(oparent != null)
				oparent.Tell(new NewChild(Observable));
		}

		public ObservableActor(Observable observable, IActorRef parent) {
			Observable = observable;
			oparent = parent;
			Receive<Events.IEvent>(Event);
			Receive<RegisterObserver>(RegisterObserver);
			Receive<CancellableEventResponse>(Vote);
			Receive<Terminated>(Terminated);
			Receive<NewChild>(NewChild);
		}

		private void RegisterObserver(RegisterObserver ro) {
			observers.Add(ro.Observer);
			Context.Watch(ro.Observer);
			IObservable[] childs = new IObservable[children.Count];
			children.Values.CopyTo(childs, 0);
			ro.Observer.Tell(new Notification(Observable, new Events.Notifications.Registered(Observable, childs)));
		}

		private void Event(Events.IEvent e) {
			Events.CancellableEvent c = e as Events.CancellableEvent;
			if(c != null)
				polls.Add(c, new Poll(c, observers.ToArray()));
			foreach(IActorRef observer in observers)
				observer.Tell(new Notification(Observable, e), Self);
		}

		private void Vote(CancellableEventResponse response) {
			if(polls.ContainsKey(response.Event))
				if(polls[response.Event].Vote(response))
					ReturnConsensus(polls[response.Event]);
		}

		private void Terminated(Terminated t) {
			if(children.ContainsKey(t.ActorRef))
				children.Remove(t.ActorRef);
			else
				foreach(Poll poll in polls.Values)
					if(poll.Left(t.ActorRef))
						ReturnConsensus(poll);
		}

		private void NewChild(NewChild nc) {
			children.Add(nc.Child.Actor, nc.Child);
			Context.Watch(nc.Child.Actor);
			Events.Notifications.NewObservable n = new Events.Notifications.NewObservable(Observable, nc.Child);
			foreach(IActorRef observer in observers)
				observer.Tell(new Notification(Observable, n));
		}

		private void ReturnConsensus(Poll poll) {
			Consensus consensus = poll.Consensus();
			polls.Remove(poll.Event);
			Context.Parent.Tell(consensus);
		}
	}


	internal class RegisterObserver {
		public IActorRef Observer {
			get;
			private set;
		}

		public RegisterObserver(IActorRef observer) {
			Observer = observer;
		}
	}

	internal class CancellableEventResponse {
		public Events.CancellableEvent Event {
			get;
			private set;
		}
		
		public bool Cancelled {
			get;
			private set;
		}

		public CancellableEventResponse(Events.CancellableEvent e, bool cancelled) {
			Event = e;
			Cancelled = cancelled;
		}
	}

	internal class NewChild {
		public Observable Child {
			get;
			private set;
		}

		public NewChild(Observable child) {
			Child = child;
		}
	}
}


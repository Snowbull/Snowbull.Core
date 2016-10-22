using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Akka.Actor;

namespace Snowbull.Core.Game.Rooms {
    public class RoomActor : SnowbullActor {
        protected readonly Room room;
        protected readonly List<Player.Player> players = new List<Player.Player>();
        protected int capacity;

        protected int InternalID {
            get { return room.InternalID; }
        }

        protected int ExternalID {
            get { return room.ExternalID; }
        }

        protected string Name {
            get { return room.Name; }
        }

        public RoomActor(Room room, int capacity) {
            this.room = room;
            this.capacity = capacity;
            Become(Running);
        }

        protected virtual void Running() {
            Receive<JoinRoom>(new Action<JoinRoom>(Join));
            Receive<LeaveRoom>(new Action<LeaveRoom>(Leave));
            Receive<Packets.ISendPacket>(new Action<Packets.ISendPacket>(Send));
            Receive<Terminated>(new Action<Terminated>(Terminated));
        }

        protected virtual void Join(JoinRoom jr) {
            if(players.Count < capacity) {
                Send(new Packets.Xt.Send.Rooms.AddPlayer(jr.Player.User.Id, jr.Player.ToString(), InternalID));
                players.Add(jr.Player);
                Sender.Tell(new JoinedRoom(room, players.ToImmutableList()));
                Context.Watch(jr.Player.User.ActorRef);
            }else{
                Sender.Tell(new RoomFull(room));
            }
        }

        protected virtual void Leave(LeaveRoom lr) {
            foreach(Player.Player player in players) {
                if(player.User == lr.User) {
                    Remove(player);
                    break;
                }
            }
        }

        private void Terminated(Terminated t) {
            foreach(Player.Player player in players) {
                if(player.User.ActorRef == t.ActorRef) {
                    Remove(player);
                    break;
                }
            }
        }

        protected virtual void Remove(Player.Player player) {
            players.Remove(player);
            Send(new Packets.Xt.Send.Rooms.RemovePlayer(player.User.Id, InternalID));
        }

        private void Send(Packets.ISendPacket packet) {
            foreach(Player.Player player in players)
                player.User.ActorRef.Tell(packet, Self);
        }
    }

    public sealed class JoinRoom {
        public int ExternalID {
            get;
            private set;
        }

        public Player.Player Player {
            get;
            private set;
        }

        public JoinRoom(int externalId, Player.Player player) {
            ExternalID = externalId;
            Player = player;
        }
    }

    public sealed class LeaveRoom {
        public GameUser User { // It is safer to use User here because Player can change.
            get;
            private set;
        }

        public LeaveRoom(GameUser user) {
            User = user;
        }
    }
}


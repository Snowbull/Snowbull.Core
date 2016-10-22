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
            Receive<JoinRoom>(new Action<JoinRoom>(JoinRoom));
            Receive<LeaveRoom>(new Action<LeaveRoom>(LeaveRoom));
            Receive<Packets.ISendPacket>(new Action<Packets.ISendPacket>(Send));
        }

        protected virtual void JoinRoom(JoinRoom jr) {
            if(players.Count < capacity) {
                Send(new Packets.Xt.Send.Rooms.AddPlayer(jr.Player.User.Id, jr.Player.ToString(), InternalID));
                players.Add(jr.Player);
                jr.Player.User.ActorRef.Tell(new JoinedRoom(room, players.ToImmutableList()));
            }else{
                jr.Player.User.ActorRef.Tell(new RoomFull(room));
            }
        }

        protected virtual void LeaveRoom(LeaveRoom lr) {
            foreach(Player.Player player in players) {
                if(player.User == lr.User) {
                    players.Remove(player);
                    Send(new Packets.Xt.Send.Rooms.RemovePlayer(lr.User.Id, InternalID));
                    return;
                }
            }
        }

        private void Send(Packets.ISendPacket packet) {
            foreach(Player.Player player in players)
                player.User.Connection.ActorRef.Tell(packet, Self);
        }
    }

    public sealed class JoinRoom {
        public Packets.Xt.Receive.Rooms.JoinRoom Request {
            get;
            private set;
        }

        public Player.Player Player {
            get;
            private set;
        }

        public JoinRoom(Packets.Xt.Receive.Rooms.JoinRoom request, Player.Player player) {
            Request = request;
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


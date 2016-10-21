using System;
using System.Collections.Generic;
using Akka.Actor;

namespace Snowbull.Core.Game.Rooms {
    public class RoomActor : SnowbullActor {
        protected readonly Room room;
        protected readonly List<GameUser> users = new List<GameUser>();
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
            if(users.Count < capacity) {
                users.Add(jr.User);
                jr.User.ActorRef.Tell(new JoinedRoom(room));
                Send(new Packets.Xt.Send.Rooms.AddPlayer(jr.User.Id, "", InternalID));
            }else{
                jr.User.ActorRef.Tell(new RoomFull(room));
            }
        }

        protected virtual void LeaveRoom(LeaveRoom lr) {
            users.Remove(lr.User);
            Send(new Packets.Xt.Send.Rooms.RemovePlayer(lr.User.Id, InternalID));
        }

        private void Send(Packets.ISendPacket packet) {
            foreach(GameUser user in users)
                user.ActorRef.Tell(packet);
        }
    }

    public sealed class JoinRoom {
        public GameUser User {
            get;
            private set;
        }

        public JoinRoom(GameUser user) {
            User = user;
        }
    }

    public sealed class LeaveRoom {
        public GameUser User {
            get;
            private set;
        }

        public LeaveRoom(GameUser user) {
            User = user;
        }
    }
}


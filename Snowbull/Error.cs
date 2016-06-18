/**
 * Error Enumeration for Snowbull's Plugin API ("Snowbull").
 *
 * Copyright 2016 by Lewis Hazell <staticabc@live.co.uk>
 *
 * This file is part of "Snowbull".
 * 
 * "Snowbull" is free software: you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation, either version
 * 3 of the License, or (at your option) any later version.
 * 
 * "Snowbull" is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty 
 * of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See
 * the GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with "Snowbull". If not, see <http://www.gnu.org/licenses/>.
 *
 * License: GPL-3.0 <https://www.gnu.org/licenses/gpl-3.0.txt>
 */

using System;

namespace Snowbull {
    public enum Errors {
        NO_CONNECTION = 0,
        CONNECTION_LOST = 1,
        TIME_OUT = 2,
        MULTI_CONNECTIONS = 3,
        DISCONNECT = 4,
        KICK = 5,
        NAME_NOT_FOUND = 100,
        PASSWORD_WRONG = 101,
        SERVER_FULL = 103,
        PASSWORD_REQUIRED = 130,
        PASSWORD_SHORT = 131,
        PASSWORD_LONG = 132,
        NAME_REQUIRED = 140,
        NAME_SHORT = 141,
        NAME_LONG = 142,
        LOGIN_FLOODING = 150,
        PLAYER_IN_ROOM = 200,
        ROOM_FULL = 210,
        GAME_FULL = 211,
        ROOM_CAPACITY_RULE = 212,
        ITEM_IN_HOUSE = 400,
        NOT_ENOUGH_COINS = 401,
        ITEM_NOT_EXIST = 402,
        NOT_ENOUGH_MEDALS = 405,
        NAME_NOT_ALLOWED = 441,
        PUFFLE_LIMIT_M = 440,
        PUFFLE_LIMIT_NM = 442,
        BAN_DURATION = 601,
        BAN_AN_HOUR = 602,
        BAN_FOREVER = 603,
        AUTO_BAN = 610,
        GAME_CHEAT = 800,
        ACCOUNT_NOT_ACTIVATE = 900,
        BUDDY_LIMIT = 901,
        NO_PLAY_TIME = 910,
        OUT_PLAY_TIME = 911,
        GROUNDED = 913,
        PLAY_TIME_OVER = 914,
        SYSTEM_REBOOT = 990,
        NOT_MEMBER = 999,
        NO_DB_CONNECTION = 1000,
        TIME_WARNING = 10001,
        TIMEOUT = 10002,
        PASSWORD_SAVE_PROMPT = 10003,
        SOCKET_LOST_CONNECTION = 10004,
        LOAD_ERROR = 10005,
        MAX_IGLOO_FURNITURE_ERROR = 10006,
        MULTIPLE_CONNECTIONS = 10007,
        CONNECTION_TIMEOUT = 10008,
        MAX_STAMPBOOK_COVER_ITEMS = 10009,
        REDEMPTION_CONNECTION_LOST = 20001,
        REDEMPTION_ALREADY_HAVE_ITEM = 20002,
        REDEMPTION_SERVER_FULL = 20103,
        REDEMPTION_BOOK_ID_NOT_EXIST = 20710,
        REDEMPTION_BOOK_ALREADY_REDEEMED = 20711,
        REDEMPTION_WRONG_BOOK_ANSWER = 20712,
        REDEMPTION_BOOK_TOO_MANY_ATTEMPTS = 20713,
        REDEMPTION_CODE_NOT_FOUND = 20720,
        REDEMPTION_CODE_ALREADY_REDEEMED = 20721,
        REDEMPTION_TOO_MANY_ATTEMPTS = 20722,
        REDEMPTION_CATALOG_NOT_AVAILABLE = 20723,
        REDEMPTION_NO_EXCLUSIVE_REDEEMS = 20724,
        REDEMPTION_CODE_GROUP_REDEEMED = 20725,
        REDEMPTION_PUFFLES_MAX = 20730,
        REDEMPTION_PUFFLE_INVALID = 21700,
        REDEMPTION_PUFFLE_CODE_MAX = 21701,
        REDEMPTION_CODE_TOO_SHORT = 21702,
        REDEMPTION_CODE_TOO_LONG = 21703,
        GOLDEN_CODE_NOT_READY = 21704
    }
}


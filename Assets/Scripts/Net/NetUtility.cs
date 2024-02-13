using System;
using Unity.Networking.Transport;
using UnityEngine;

public enum OpCode
{
    KEEP_ALIVE = 1,
    WELCOME = 2,
    START_GAME = 3,
    MAKE_MOVE = 4,
    MAKE_ATTACK = 5,
    SWITCH_TURNS = 6,
    END_GAME = 7
}
public static class NetUtility
{
    public static void OnData(DataStreamReader stream, NetworkConnection cnn, Server server = null)
    {
        NetMessage msg = null;
        var opCode = (OpCode)stream.ReadByte();
        switch (opCode)
        {
            case OpCode.KEEP_ALIVE: msg = new NetKeepAlive(stream); break;  
            case OpCode.WELCOME: msg = new NetWelcome(stream); break;  
            case OpCode.START_GAME: msg = new NetStartGame(stream); break;  
            case OpCode.MAKE_MOVE: msg = new NetMakeMove(stream); break;  
            case OpCode.MAKE_ATTACK: msg = new NetMakeAttack(stream); break;
            case OpCode.SWITCH_TURNS: msg = new NetSwitchTurns(stream); break;
            case OpCode.END_GAME: msg = new NetEndGame(stream); break;
            default:
                Debug.LogError("Message received has no opCode");
                break;
        }

        if (server != null)
            msg.ReceivedOnServer(cnn);
        else
            msg.ReceivedOnClient();

    }

    // Net Utility
    public static Action<NetMessage> C_KEEP_ALIVE;
    public static Action<NetMessage> C_WELCOME;
    public static Action<NetMessage> C_START_GAME;
    public static Action<NetMessage> C_MAKE_MOVE;
    public static Action<NetMessage> C_MAKE_ATTACK;
    public static Action<NetMessage> C_SWITCH_TURNS;
    public static Action<NetMessage> C_END_GAME;
    public static Action<NetMessage, NetworkConnection> S_KEEP_ALIVE;
    public static Action<NetMessage, NetworkConnection> S_WELCOME;
    public static Action<NetMessage, NetworkConnection> S_START_GAME;
    public static Action<NetMessage, NetworkConnection> S_MAKE_MOVE;
    public static Action<NetMessage, NetworkConnection> S_MAKE_ATTACK;
    public static Action<NetMessage, NetworkConnection> S_SWITCH_TURNS;
    public static Action<NetMessage, NetworkConnection> S_END_GAME;

}

using Unity.Networking.Transport;

public class NetSwitchTurns : NetMessage
{
    public int isPlayerOneTurn;
    public NetSwitchTurns()
    {
        Code = OpCode.SWITCH_TURNS;
    }
    public NetSwitchTurns(DataStreamReader reader)
    {
        Code = OpCode.SWITCH_TURNS;
        Deserialize(reader);
    }

    public override void Serialize(ref DataStreamWriter writer)
    {
        writer.WriteByte((byte)Code);
        writer.WriteInt(isPlayerOneTurn);
    }
    public override void Deserialize(DataStreamReader reader)
    {
        isPlayerOneTurn = reader.ReadInt();
    }

    public override void ReceivedOnClient()
    {
        NetUtility.C_SWITCH_TURNS?.Invoke(this);
    }

    public override void ReceivedOnServer(NetworkConnection cnn)
    {
        NetUtility.S_SWITCH_TURNS?.Invoke(this, cnn);
    }
}

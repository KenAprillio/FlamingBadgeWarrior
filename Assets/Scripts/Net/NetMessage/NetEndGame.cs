using Unity.Networking.Transport;

public class NetEndGame : NetMessage
{
    public int losingTeamId;
    public NetEndGame()
    {
        Code = OpCode.END_GAME;
    }
    public NetEndGame(DataStreamReader reader)
    {
        Code = OpCode.END_GAME;
        Deserialize(reader);
    }

    public override void Serialize(ref DataStreamWriter writer)
    {
        writer.WriteByte((byte)Code);
        writer.WriteInt(losingTeamId);
    }
    public override void Deserialize(DataStreamReader reader)
    {
        losingTeamId = reader.ReadInt();
    }

    public override void ReceivedOnClient()
    {
        NetUtility.C_END_GAME?.Invoke(this);
    }

    public override void ReceivedOnServer(NetworkConnection cnn)
    {
        NetUtility.S_END_GAME?.Invoke(this, cnn);
    }
}

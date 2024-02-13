using Unity.Networking.Transport;
using UnityEngine;

public class NetMakeAttack : NetMessage
{
    public int unitX;
    public int unitY;
    public int teamId;
    public int isMagic;
    public float damage;

    public NetMakeAttack()
    {
        Code = OpCode.MAKE_ATTACK;
    }
    public NetMakeAttack(DataStreamReader reader)
    {
        Code = OpCode.MAKE_ATTACK;
        Deserialize(reader);
    }

    public override void Serialize(ref DataStreamWriter writer)
    {
        writer.WriteByte((byte)Code);
        writer.WriteInt(unitX);
        writer.WriteInt(unitY);
        writer.WriteFloat(damage);
        writer.WriteInt(teamId);
        writer.WriteInt(isMagic);
    }
    public override void Deserialize(DataStreamReader reader)
    {
        unitX = reader.ReadInt();
        unitY = reader.ReadInt();
        damage = reader.ReadFloat();
        teamId = reader.ReadInt();
        isMagic = reader.ReadInt();
    }

    public override void ReceivedOnClient()
    {
        NetUtility.C_MAKE_ATTACK?.Invoke(this);
    }

    public override void ReceivedOnServer(NetworkConnection cnn)
    {
        NetUtility.S_MAKE_ATTACK?.Invoke(this, cnn);
    }
}

namespace Flr.Channel;

public interface IChannel<in T>
{
    public bool Consume(CodeReader code, T output);
}

namespace Domain
{
    public interface IQuery<out T>
    {
        T Execute();
    }
}
using JetBrains.Annotations;
namespace Domain
{
    public interface IQuery<out T>
    {
        [CanBeNull]
        T Execute();
    }
}
using OS.Infrastructure.Common.Annotations;

namespace Domain
{
    public interface IQuery<out T>
    {
        [CanBeNull]
        T Execute();
    }
}
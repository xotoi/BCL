using System.Threading.Tasks;

namespace BCL.Interfaces
{
    public interface IFilesDistributor<TModel>
    {
        Task MoveAsync(TModel item);
    }
}

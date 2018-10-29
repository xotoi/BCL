using System;
using BCL.Entity;

namespace BCL.Interfaces
{
    public interface IFileWatcher<TModel>
    {
        event EventHandler<ItemCreatedIventArgs<TModel>> Created;
    }
}

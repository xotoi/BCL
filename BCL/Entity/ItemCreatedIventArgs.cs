namespace BCL.Entity
{
    public class ItemCreatedIventArgs<TModel> : System.EventArgs
    {
        public TModel CreatedItem { get; set; }
    }
}

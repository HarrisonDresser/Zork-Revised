namespace Zork.Common
{
    public class Item
    {
        public string Name { get; }

        //public string LookDescription { get;  }

      //  public string InventoryDescription { get; }

        public string Description { get; }

        //public string Description { get; }

        public Item(string name, string description)
        {
            Name = name;
            Description = description;
            //LookDescription = lookDescription;
            //InventoryDescription = InventoryDescription;
        }

        public override string ToString() => Name;
    }
}
namespace Fougerite
{
    /// <summary>
    /// This class is created when an Item is added or removed to/from an inventory.
    /// </summary>
    public class FInventory
    {
        private Inventory _inv;
        private EntityItem[] _items;

        public FInventory(Inventory inv)
        {
            this._inv = inv;
            this._items = new EntityItem[inv.slotCount];
            for (var i = 0; i < inv.slotCount; i++)
                this._items[i] = new EntityItem(this._inv, i);
        }

        /// <summary>
        /// Adds one item to the inventory.
        /// </summary>
        /// <param name="name"></param>
        public void AddItem(string name)
        {
            this.AddItem(name, 1);
        }

        /// <summary>
        /// Adds an Item with the given amount to the inventory.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="amount"></param>
        public void AddItem(string name, int amount)
        {
            ItemDataBlock item = DatablockDictionary.GetByName(name);
            this._inv.AddItemAmount(item, amount);
        }

        /// <summary>
        /// Adds an item to the specified slot.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="slot"></param>
        public void AddItemTo(string name, int slot)
        {
            this.AddItemTo(name, slot, 1);
        }

        /// <summary>
        /// Adds an item to the specified slot with the given amount.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="slot"></param>
        /// <param name="amount"></param>
        public void AddItemTo(string name, int slot, int amount)
        {
            ItemDataBlock byName = DatablockDictionary.GetByName(name);
            if (byName != null)
            {
                Inventory.Slot.Kind place = Inventory.Slot.Kind.Default;
                this._inv.AddItemSomehow(byName, new Inventory.Slot.Kind?(place), slot, amount);
            }
        }

        /// <summary>
        /// Deletes all items from the inventory.
        /// </summary>
        public void ClearAll()
        {
            this._inv.Clear();
        }

        private int GetFreeSlots()
        {
            int num = 0;
            for (int i = 0; i < this._inv.slotCount; i++)
            {
                if (this._inv.IsSlotFree(i))
                {
                    num++;
                }
            }
            return num;
        }

        /// <summary>
        /// Checks if the inventory has the specified item.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        public bool HasItem(string name, int amount = 1)
        {
            int num = 0;
            foreach (EntityItem item in this.Items)
            {
                if (item.Name == name)
                    num += item.UsesLeft;
            }
            return (num >= amount);
        }

        /// <summary>
        /// Moves the item from s1 slot to s2 slot.
        /// </summary>
        /// <param name="s1"></param>
        /// <param name="s2"></param>
        public void MoveItem(int s1, int s2)
        {
            this._inv.MoveItemAtSlotToEmptySlot(this._inv, s1, s2);
        }

        /// <summary>
        /// Removes the specific item with the given amount.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="amount"></param>
        public void RemoveItem(string name, int amount = 1)
        {
            foreach (EntityItem item in this.Items)
            {
                if (item.Name == name)
                {
                    if (item.UsesLeft > amount)
                    {
                        this._inv.RemoveItem(item.RInventoryItem);
                        this.AddItem(item.Name, (item.UsesLeft - amount));
                        return;
                    }
                    else if (item.UsesLeft == amount)
                    {
                        this._inv.RemoveItem(item.RInventoryItem);
                        return;
                    }
                    else
                    {
                        this._inv.RemoveItem(item.RInventoryItem);
                        amount -= item.UsesLeft;
                    }
                }
            }
        }

        /// <summary>
        /// Removes an item from the specified slot with the given amount.
        /// </summary>
        /// <param name="slot"></param>
        /// <param name="amount"></param>
        public void RemoveItem(int slot, int amount = 1)
        {
            EntityItem item = this.Items[slot];
            if (item == null)
                return;
            if (item.UsesLeft > amount)
            {
                this._inv.RemoveItem(item.RInventoryItem);
                this.AddItem(item.Name, (item.UsesLeft - amount));
                return;
            }
            this._inv.RemoveItem(item.RInventoryItem);
        }

        /// <summary>
        /// Counts the freeslots in the inventory.
        /// </summary>
        public int FreeSlots
        {
            get
            {
                return this.GetFreeSlots();
            }
        }

        /// <summary>
        /// Gets the maximum slot amount rom the inventory.
        /// </summary>
        public int SlotCount
        {
            get
            {
                return this._inv.slotCount;
            }
        }

        /// <summary>
        /// Gets the items from the inventory.
        /// </summary>
        public EntityItem[] Items
        {
            get
            {
                return this._items;
            }
        }
    }
}

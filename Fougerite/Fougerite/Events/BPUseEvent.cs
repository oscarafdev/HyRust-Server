namespace Fougerite.Events
{
    /// <summary>
    /// This class is used on Blueprint Use Hook.
    /// </summary>
    public class BPUseEvent
    {
        private BlueprintDataBlock _bdb;
        private bool _cancel;
        private readonly IBlueprintItem _item;

        public BPUseEvent(BlueprintDataBlock bdb, IBlueprintItem item)
        {
            this.DataBlock = bdb;
            this.Cancel = false;
            this._item = item;
        }

        /// <summary>
        /// Gets if the event is cancelled or can be set to cancelled.
        /// </summary>
        public bool Cancel
        {
            get
            {
                return this._cancel;
            }
            set
            {
                this._cancel = value;
            }
        }

        /// <summary>
        /// Gets the blueprint's datablock.
        /// </summary>
        public BlueprintDataBlock DataBlock
        {
            get
            {
                return this._bdb;
            }
            set
            {
                this._bdb = value;
            }
        }

        /// <summary>
        /// Gets the actual blueprint item.
        /// </summary>
        public IBlueprintItem Item
        {
            get
            {
                return this._item;
            }
        }

        /// <summary>
        /// Gets the name of the blueprint item.
        /// </summary>
        public string ItemName
        {
            get
            {
                return this._bdb.resultItem.name;
            }
        }
    }
}
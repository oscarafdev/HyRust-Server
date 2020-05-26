namespace Fougerite.Events
{
    /// <summary>
    /// This class is created when a player is gathering from an animal or from a resource.
    /// </summary>
    public class GatherEvent
    {
        private string _item;
        private bool _over;
        private int _qty;
        private readonly string _type;
        private readonly ResourceTarget res;
        private readonly ItemDataBlock dataBlock = null;
        private readonly ResourceGivePair resourceGivePair = null;

        public GatherEvent(ResourceTarget r, ItemDataBlock db, int qty)
        {
            this.res = r;
            this._qty = qty;
            this._item = db.name;
            this._type = "Tree";
            this.dataBlock = db;
            this.Override = false;
        }

        public GatherEvent(ResourceTarget r, ResourceGivePair gp, int qty)
        {
            this.res = r;
            this._qty = qty;
            this._item = gp.ResourceItemDataBlock.name;
            this._type = this.res.type.ToString();
            this.resourceGivePair = gp;
            this.Override = false;
        }

        /// <summary>
        /// Obtiene la cantidad de recursos que quedan en el objeto.
        /// </summary>
        public int AmountLeft
        {
            get
            {
                return this.res.GetTotalResLeft();
            }
        }

        /// <summary>
        /// Obtiene el nombre del elemento que estamos recibiendo del gather.
        /// </summary>
        public string Item
        {
            get
            {
                return this._item;
            }
            set
            {
                this._item = value;
            }
        }

        /// <summary>
        /// Obtiene o establece si debemos minimizar la cantidad de recursos que quedan en el recurso.
        /// </summary>
        public bool Override
        {
            get
            {
                return this._over;
            }
            set
            {
                this._over = value;
            }
        }

        /// <summary>
        /// Obtiene el porcentaje de los recursos.
        /// </summary>
        public float PercentFull
        {
            get
            {
                return this.res.GetPercentFull();
            }
        }

        /// <summary>
        /// Gets the Quantity of the items we are gathering.
        /// </summary>
        public int Quantity
        {
            get
            {
                return this._qty;
            }
            set
            {
                this._qty = value;
            }
        }

        /// <summary>
        /// Gets the type of resource we are hitting.
        /// </summary>
        public string Type
        {
            get
            {
                return this._type;
            }
        }

        /// <summary>
        /// Gets the resource target that we are hitting.
        /// </summary>
        public ResourceTarget ResourceTarget
        {
            get
            {
                return this.res;
            }
        }

        /// <summary>
        /// Gets the itemdatablock that we are gathering.
        /// </summary>
        public ItemDataBlock ItemDataBlock
        {
            get
            {
                return this.dataBlock;
            }
        }

        /// <summary>
        /// Gets the original ResourceGivePair class.
        /// </summary>
        public ResourceGivePair ResourceGivePair
        {
            get
            {
                return this.resourceGivePair;
            }
        }
    }
}
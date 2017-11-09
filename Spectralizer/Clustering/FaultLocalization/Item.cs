using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spectralizer.Clustering.FaultLocalization
{
    public class Item : IComparable<Item>
    {
        private string itemName;
        private double suspiciousness;

        public Item(string functionName, double suspiciousness)
        {
            this.itemName = functionName;
            this.suspiciousness = suspiciousness;
        }

        public string ItemName
        {
            get { return itemName; }
        }

        public double Suspiciousness
        {
            get { return suspiciousness; }
        }

        public int CompareTo(Item other)
        {
            double difference = suspiciousness - other.Suspiciousness;

            if (difference < 0)
            {
                return -1;
            }
            else if (difference > 0)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }

        public override bool Equals(object obj)
        {
            // If parameter is null return false:
            if (obj == null)
            {
                return false;
            }

            if (obj is Item)
            {
                Item function = obj as Item;

                if (function.ItemName.Equals(this.itemName)) return true;
                else return false;
            }

            return base.Equals(obj);
        }

        public bool Equals(Item otherItem)
        {
            // If parameter is null return false:
            if ((object)otherItem == null)
            {
                return false;
            }

            if (otherItem.ItemName.Equals(this.itemName)) return true;
            else return false;
        }

        public override int GetHashCode()
        {
            return itemName.GetHashCode() ^ (new Random().Next(99999999));
        }

        public override string ToString()
        {
            return string.Format("{0:N8}", suspiciousness) + " - " + itemName;
        }
    }
}

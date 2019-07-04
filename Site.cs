using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebsiteChecker
{
    public class Site : IComparable<Site>, IComparer<Site>
    {
        public string Address { get; set; }

        public Site(string siteAddress)
        {
            Address = siteAddress;
        }

        public int Compare(Site s1, Site s2)
        {
            if (s1 == null && s2 == null)
            {
                return 0;
            }
            if (s1 != null && s2 == null)
            {
                return 1;
            }
            if (s1 == null && s2 != null)
            {
                return -1;
            }
            //на этом этапе оба объекта гарантированно != null
            return String.Compare(s1.Address, s2.Address);
        }
        public int CompareTo(Site s)
        {
            if (s == null)
            {
                return -1;
            }
            else
            {
                string site1Address = this.Address;
                return site1Address.CompareTo(s.Address);
            }
        }

        public override bool Equals(Object obj)
        {
            if (obj == null)
            {
                return false;
            }
            Site s = obj as Site;
            if (s as Site == null)
            {
                return false;
            }    
            return this.Address == s.Address;
        }
        
        public bool Equals(Site s)
        {
            if (s == null)
            {
                return false;
            }
            return this.Address == s.Address;
        }

        public override int GetHashCode()
        {

            int GetNumberFromText(string processingString)
            {
                int number = 0;
                foreach (char symbol in processingString)
                {
                    number += (int)symbol;
                }
                return number;
            }

            int hash = 17;

            hash *= GetNumberFromText(this.Address);
            //при добавлении полей - дописать сюда их хеширование

            return hash;
        }
    }
}

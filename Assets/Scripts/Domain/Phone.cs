using System;

namespace Scripts.Domain
{
    [Serializable]
    public class Phone
    {
        public string Company;
        public string Name;
        public Resolution Resolution;
        public string Tooltip;
    }
}
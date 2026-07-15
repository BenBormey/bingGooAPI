using System;

namespace unt_bingoo.Class
{
    public class UOM
    {
        public int UOMId { get; set; }

        public string? UOMCode { get; set; }

        public string? UOMName { get; set; }

        public bool IsActive { get; set; }


        public string DisplayName
        {
            get
            {
                return $"{UOMCode} - {UOMName}";
            }
        }
    }
}
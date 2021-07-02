using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrpcWebBlazorWasm.Shared
{
    public partial class TouristRoute
    {      

        [Column(TypeName = "decimal(18,2)")]
        public decimal OriginalPrice { get; set; }

        [Range(0.0, 1.0)]
        public double? DiscountPresent { get; set; }

        //
        public void TouristRouteAppend()
        {
            
                Price = OriginalPrice * (decimal)(DiscountPresent ?? 1);

        }
    }
}

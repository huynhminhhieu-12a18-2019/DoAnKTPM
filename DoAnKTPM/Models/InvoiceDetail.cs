using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace DoAnKTPM.Models
{
    public class InvoiceDetail
    {
        public int Id { get; set; }

        public int InvoiceId { get; set; }

        // Navigation reference property cho khóa ngoại đến Invoices
        [DisplayName("Hóa đơn")]
        public Invoice Invoice { get; set; }

        public int ProductId { get; set; }

        // Navigation reference property cho khóa ngoại đến Products
        [DisplayName("Sản phẩm")]
        public Product Product { get; set; }

        [DisplayName("Số lượng")]
        [DefaultValue(1)]
        public int Quantity { get; set; } = 1;

        [DisplayName("Đơn giá")]
        [DefaultValue(0)]
        public int UnitPrice { get; set; } = 0;
    }
}

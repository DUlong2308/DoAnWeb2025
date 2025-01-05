using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebApplicationlaptop.Models
{
    public class OrderModel
    {
        public int Id { get; set; }

        [Required]
        public string OrderCode { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        public DateTime CreateDate { get; set; }

        [Required]
        public int Status { get; set; }

        public string Hovaten { get; set; }  // Thêm thuộc tính này
      
        public List<OrderDetail> OrderDetail { get; set; }
       


    }

}
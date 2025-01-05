﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApplicationlaptop.Responsibilty.Validation;

namespace WebApplicationlaptop.Models
{
    public class SliderModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Yêu cầu không được bỏ trống tên slider")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Yêu cầu không được bỏ trống mô tả")]
        public string Description { get; set; }

        public int? Status { get; set; }

        public string Image { get; set; }

        [FileExtension(new string[] { ".jpg", ".png", ".jpeg", ".gif" })]
        [NotMapped]
        public IFormFile ImageUpload { get; set; }
    }
}
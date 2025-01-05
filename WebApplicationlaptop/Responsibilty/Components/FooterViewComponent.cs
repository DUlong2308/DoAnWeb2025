﻿using HandlebarsDotNet;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplicationlaptop.Responsibility;

namespace WebApplicationlaptop.Responsibilty.Components
{
    public class FooterViewComponent : ViewComponent
    {
        private readonly DataContext _dataContext;

        public FooterViewComponent(DataContext context)
        {
            _dataContext = context;
        }
        public async Task<IViewComponentResult> InvokeAsync() => View(await _dataContext.Informationshops.ToListAsync());
    }
}

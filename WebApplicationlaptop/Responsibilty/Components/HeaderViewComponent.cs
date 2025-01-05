using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplicationlaptop.Responsibility;

namespace WebApplicationlaptop.Responsibilty.Components
{

    public class HeaderViewComponent : ViewComponent
    {

        private readonly DataContext _dataContext;

        public HeaderViewComponent(DataContext context)
        {
            _dataContext = context;
        }
        public async Task<IViewComponentResult> InvokeAsync() => View(await _dataContext.Informationshops.ToListAsync());
    }
}

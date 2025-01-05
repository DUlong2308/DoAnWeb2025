using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplicationlaptop.Responsibility;


namespace WebApplicationlaptop.Responsibilty.Components
{
    public class CategoriesViewComponent : ViewComponent
    {
		private readonly DataContext Context;
        public CategoriesViewComponent(DataContext context)
        {
			Context = context;
        }
        public async Task<IViewComponentResult> InvokeAsync() => View(await Context.Categories.ToListAsync());
    }
}

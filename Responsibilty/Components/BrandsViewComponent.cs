using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplicationlaptop.Responsibility;

namespace WebApplicationlaptop.Responsibilty.Components
{
	public class BrandsViewComponent : ViewComponent
	{
		private readonly DataContext Context;
		public BrandsViewComponent(DataContext context)
		{
			Context = context;
		}
		public async Task<IViewComponentResult> InvokeAsync() => View(await Context.Brands.ToListAsync());

	}
}

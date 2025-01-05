using WebApplicationlaptop.Models.Momo;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using WebApplicationlaptop.Models;

namespace WebApplicationlaptop.Services.Momo
{
    public interface IMomoService
    {
        Task<MomoCreatePaymentResponseModel> CreatePaymentMomo(OrderInfo model);
        Task<MomoExecuteResponseModel> PaymentExecuteAsync(IQueryCollection collection);
    }
}

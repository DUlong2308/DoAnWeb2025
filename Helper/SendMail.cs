using RazorLight;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using WebApplicationlaptop.Models;

namespace WebApplicationlaptop.Helper
{
    public class SendMail
    {
        public static bool SendEmail(string to, string subject, string body, string attachFile)
        {
            try
            {
                using (MailMessage msg = new MailMessage(ConstantHelper.emailSender, to, subject, body))
                {
                    msg.IsBodyHtml = true; // Ensure the email body is HTML

                    using (SmtpClient client = new SmtpClient(ConstantHelper.hostEmail, ConstantHelper.portEmail))
                    {
                        client.EnableSsl = true;

                        // Attach file if provided
                        if (!string.IsNullOrEmpty(attachFile))
                        {
                            Attachment attachment = new Attachment(attachFile);
                            msg.Attachments.Add(attachment);
                        }

                        // Set credentials
                        NetworkCredential credential = new NetworkCredential(ConstantHelper.emailSender, ConstantHelper.passwordSender);
                        client.UseDefaultCredentials = false;
                        client.Credentials = credential;

                        // Send email
                        client.Send(msg);
                    }
                }

                return true; // Email sent successfully
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending email: {ex.Message}");
                return false; // Failed to send email
            }
        }

        public static async Task<bool> SendEmail2(OrderModel order)
        {
            try
            {
                // Prepare email body with product names, quantities, prices, and total
                string body = $@"
                    <html>
                    <body style='font-family: Arial, sans-serif;'>
                        <h2 style='color: #333333;'> Shop LAPTOP LongQuanDinh</h2> 
                        <h2>Cảm Ơn Đã Tin Tưởng Và Mua Hàng của chúng tôi !</h2>
                        <p>Dear {order.UserName},</p>
                        <p>Dưới đây là đơn hàng của bạn đã thanh toán bên shop</p>
                        <table style='width: 100%; border-collapse: collapse;'>
                            <thead style='background-color: #f2f2f2;'>
                                <tr>
                                    <th style='padding: 10px; text-align: left;'>Sản Phẩm</th>
                                    <th style='padding: 10px; text-align: left;'>Số lượng</th>
                                    <th style='padding: 10px; text-align: left;'>Giá</th>
                                    <th style='padding: 10px; text-align: left;'>Tổng giá</th>
                                </tr>
                            </thead>
                            <tbody>";

                foreach (var detail in order.OrderDetail)
                {
                    body += $@"
                                <tr>
                                    <td style='padding: 8px; border-bottom: 1px solid #ddd;'>{detail.ProductName}</td>
                                    <td style='padding: 8px; border-bottom: 1px solid #ddd;'>{detail.Quantity}</td>
                                    <td style='padding: 8px; border-bottom: 1px solid #ddd;'>{detail.Price:C}</td>
                                    <td style='padding: 8px; border-bottom: 1px solid #ddd;'>{(detail.Price * detail.Quantity):C}</td>
                                </tr>";
                }

                body += $@"
                            </tbody>
                            <tfoot>
                                <tr>
                                    <td colspan='3' style='text-align: right; padding: 10px;'>Tổng Tiền:</td>
                                    <td style='padding: 10px; font-weight: bold;'>{order.OrderDetail.Sum(d => d.Price * d.Quantity):C}</td>
                                </tr>
                            </tfoot>
                        </table>
                        <p style='margin-top: 20px;'>Thank you for shopping with us!</p>
                        <p style='color: #999999;'>This is an automated message. Please do not reply.</p>
                    </body>
                    </html>";

                // Send email using SendEmail method
                return SendEmail(order.UserName, "Your Order Summary", body, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending email: {ex.Message}");
                return false;
            }
        }
    }
}

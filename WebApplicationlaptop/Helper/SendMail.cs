using System.Net;
using System.Net.Mail;
using WebApplicationlaptop.Models;

namespace WebApplicationlaptop.Helper
{
    public class SendMail
    {
        /// <summary>
        /// Gửi email đơn giản với file đính kèm (nếu có)
        /// </summary>
        public static bool SendEmail(string to, string subject, string body, string attachFile = null)
        {
            try
            {
                using (MailMessage msg = new MailMessage(ConstantHelper.emailSender, to, subject, body))
                {
                    msg.IsBodyHtml = true;

                    if (!string.IsNullOrEmpty(attachFile))
                    {
                        Attachment attachment = new Attachment(attachFile);
                        msg.Attachments.Add(attachment);
                    }

                    using (SmtpClient client = new SmtpClient(ConstantHelper.hostEmail, ConstantHelper.portEmail))
                    {
                        client.EnableSsl = true;
                        client.Credentials = new NetworkCredential(ConstantHelper.emailSender, ConstantHelper.passwordSender);
                        client.Send(msg);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                // Ghi lại chi tiết lỗi vào Console hoặc log file
                Console.WriteLine($"Lỗi khi gửi email: {ex.Message}");

                // Nếu có lỗi, trả về false
                return false;
            }
        }

        /// <summary>
        /// Gửi email với thông tin đơn hàng
        /// </summary>
        public static async Task<bool> SendOrderSummaryEmailAsync(OrderModel order)
        {
            try
            {
                // Tính tổng giá trị sản phẩm
                decimal totalProductPrice = order.OrderDetail.Sum(d => d.Price * d.Quantity);

                // Lấy phí ship từ OrderDetail (nếu có)
                decimal shippingPrice = order.ShippingCost ?? 0; // Sử dụng ShippingCost từ OrderModel

                // Tính tổng cộng (sản phẩm + phí ship)
                decimal totalPrice = totalProductPrice + shippingPrice;

                // Tính số tiền giảm giá
                decimal discountAmount = order.DiscountAmount ?? 0; // Lấy DiscountAmount từ OrderModel

                // Tính tổng tiền sau khi áp dụng giảm giá
                decimal finalAmount = totalPrice - discountAmount;

                // Chuẩn bị phương thức thanh toán hiển thị
                string paymentMethodDisplay = order.PaymentMethod?.ToLower() switch
                {
                    "cod" => "Thanh toán khi nhận hàng",  // Hiển thị nếu phương thức là COD
                    _ => "Thanh toán qua MoMo"            // Hiển thị mặc định nếu phương thức khác là MoMo
                };

                // Lấy địa chỉ và số điện thoại từ OrderDetail (lấy chi tiết đầu tiên hoặc có thể chọn chi tiết khác nếu cần)
                var firstDetail = order.OrderDetail.FirstOrDefault();
                string address = firstDetail?.Address ?? "Chưa có địa chỉ";
                string phoneNumber = firstDetail?.PhoneNumber ?? "Chưa có số điện thoại";

                // Chuẩn bị nội dung email
                string body = $@"
    <html>
    <body style='font-family: Arial, sans-serif;'>
        <h2 style='color: #333;'>Shop LAPTOP LongQuanDinh</h2>
        <h3>Cảm ơn bạn đã tin tưởng và mua hàng của chúng tôi!</h3>
        <p>Dear {order.UserName},</p>
        <p><strong>Địa chỉ nhận hàng:</strong> {address}</p>
        <p><strong>Số điện thoại:</strong> {phoneNumber}</p>
        <p><strong>Phương thức thanh toán:</strong> {paymentMethodDisplay}</p>
        <p>Dưới đây là chi tiết đơn hàng của bạn:</p>
        <table style='width: 100%; border-collapse: collapse;'>
            <thead>
                <tr style='background-color: #f2f2f2;'>
                    <th style='padding: 10px;'>Sản Phẩm</th>
                    <th style='padding: 10px;'>Số Lượng</th>
                    <th style='padding: 10px;'>Giá</th>
                    <th style='padding: 10px;'>Tổng Giá</th>
                </tr>
            </thead>
            <tbody>";

                // Lặp qua tất cả các chi tiết sản phẩm trong đơn hàng
                foreach (var detail in order.OrderDetail)
                {
                    body += $@"
            <tr>
                <td style='padding: 8px; border-bottom: 1px solid #ddd;'>{detail.ProductName}</td>
                <td style='padding: 8px; border-bottom: 1px solid #ddd;'>{detail.Quantity}</td>
                <td style='padding: 8px; border-bottom: 1px solid #ddd;'>{detail.Price.ToString("N0")} ₫</td>
                <td style='padding: 8px; border-bottom: 1px solid #ddd;'>{(detail.Price * detail.Quantity).ToString("N0")} ₫</td>
            </tr>";
                }

                body += $@"
        </tbody>
        <tfoot>
            <tr>
                <td colspan='3' style='text-align: right; padding: 10px;'>Tổng Sản Phẩm:</td>
                <td style='padding: 10px; font-weight: bold;'>{totalProductPrice.ToString("N0")} ₫</td>
            </tr>
            <tr>
                <td colspan='3' style='text-align: right; padding: 10px;'>Phí Vận Chuyển:</td>
                <td style='padding: 10px; font-weight: bold;'>{shippingPrice.ToString("N0")} ₫</td>
            </tr>";

                // Kiểm tra nếu có giảm giá thì hiển thị
                if (discountAmount > 0)
                {
                    body += $@"
            <tr>
                <td colspan='3' style='text-align: right; padding: 10px;'>Giảm Giá:</td>
                <td style='padding: 10px; font-weight: bold;'>-{discountAmount.ToString("N0")} ₫</td>
            </tr>";
                }

                body += $@"
            <tr>
                <td colspan='3' style='text-align: right; padding: 10px;'>Tổng Cộng:</td>
                <td style='padding: 10px; font-weight: bold;'>{finalAmount.ToString("N0")} ₫</td>
            </tr>
        </tfoot>
    </table>
    <p style='margin-top: 20px;'>Cảm ơn bạn đã mua hàng!</p>
    <p style='color: #999;'>Đây là email tự động, vui lòng không trả lời.</p>
</body>
    </html>";

                // Gửi email bằng hàm SendEmail
                return SendEmail(order.UserName, "Chi Tiết Đơn Hàng Của Bạn", body);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi gửi email: {ex.Message}");
                return false;
            }
        }


        public static async Task<bool> SendEmailAsync(string to, string subject, string body, string attachFile = null)
        {
            try
            {
                using (MailMessage msg = new MailMessage(ConstantHelper.emailSender, to, subject, body))
                {
                    msg.IsBodyHtml = true;

                    if (!string.IsNullOrEmpty(attachFile))
                    {
                        Attachment attachment = new Attachment(attachFile);
                        msg.Attachments.Add(attachment);
                    }

                    using (SmtpClient client = new SmtpClient(ConstantHelper.hostEmail, ConstantHelper.portEmail))
                    {
                        client.EnableSsl = true;
                        client.Credentials = new NetworkCredential(ConstantHelper.emailSender, ConstantHelper.passwordSender);
                        await client.SendMailAsync(msg);  // Dùng SendMailAsync thay vì Send
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                // Ghi lại chi tiết lỗi vào Console hoặc log file
                Console.WriteLine($"Lỗi khi gửi email: {ex.Message}");

                // Nếu có lỗi, trả về false
                return false;
            }
        }

        public static async Task<bool> SendEmail12Async(string to, string subject, string body)
        {
            try
            {
                // Cấu hình SMTP client
                var smtpClient = new SmtpClient(ConstantHelper.hostEmail)
                {
                    Port = ConstantHelper.portEmail,
                    Credentials = new NetworkCredential(ConstantHelper.emailSender, ConstantHelper.passwordSender),
                    EnableSsl = true,
                };

                // Tạo mail message
                var mailMessage = new MailMessage
                {
                    From = new MailAddress(ConstantHelper.emailSender),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true,
                };

                mailMessage.To.Add(to); // Thêm người nhận vào

                // Gửi email
                await smtpClient.SendMailAsync(mailMessage); // Dùng SendMailAsync thay vì Send
                return true; // Gửi thành công
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi gửi email: {ex.Message}");
                return false; // Gửi thất bại
            }
        }
    }
}
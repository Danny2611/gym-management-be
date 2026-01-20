
using GymManagement.Application.Interfaces.Services.User;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;

namespace GymManagement.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendOTPEmailAsync(string email, string otp)
        {
            var message = new MimeMessage();

            // From
            var fromName = _configuration["EmailSettings:FromName"];
            var fromEmail = _configuration["EmailSettings:FromEmail"];
            message.From.Add(new MailboxAddress(fromName, fromEmail));

            // To
            message.To.Add(new MailboxAddress("", email));

            // Subject
            message.Subject = "Xác thực tài khoản FittLife";

            // Body
            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = $@"
                    <div style=""font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;"">
                        <h2 style=""color: #333;"">Xác thực tài khoản FittLife</h2>
                        <p>Cảm ơn bạn đã đăng ký tài khoản FittLife. Vui lòng sử dụng mã OTP sau để xác thực tài khoản:</p>
                        <div style=""background-color: #f4f4f4; padding: 20px; text-align: center; font-size: 32px; letter-spacing: 5px; margin: 20px 0; font-weight: bold; color: #4CAF50;"">
                            {otp}
                        </div>
                        <p style=""color: #666;"">Mã OTP có hiệu lực trong vòng <strong>10 phút</strong>.</p>
                        <p style=""color: #999; font-size: 14px;"">Nếu bạn không đăng ký tài khoản, vui lòng bỏ qua email này.</p>
                        <hr style=""border: none; border-top: 1px solid #eee; margin: 30px 0;"">
                        <p style=""color: #666;"">Trân trọng,<br><strong>Đội ngũ FittLife</strong></p>
                    </div>
                "
            };

            message.Body = bodyBuilder.ToMessageBody();

            // Send email
            using (var client = new SmtpClient())
            {
                var smtpHost = _configuration["EmailSettings:SmtpHost"];
                var smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"]);
                var smtpUser = _configuration["EmailSettings:SmtpUser"];
                var smtpPassword = _configuration["EmailSettings:SmtpPassword"];

                await client.ConnectAsync(smtpHost, smtpPort, SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(smtpUser, smtpPassword);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
        }

        public async Task SendWelcomeEmailAsync(string email, string name)
        {
            var message = new MimeMessage();

            var fromName = _configuration["EmailSettings:FromName"];
            var fromEmail = _configuration["EmailSettings:FromEmail"];
            message.From.Add(new MailboxAddress(fromName, fromEmail));
            message.To.Add(new MailboxAddress(name, email));
            message.Subject = "Chào mừng đến với FittLife!";

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = $@"
                    <div style=""font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;"">
                        <h2 style=""color: #4CAF50;"">Chào mừng {name} đến với FittLife! 🎉</h2>
                        <p>Tài khoản của bạn đã được xác thực thành công!</p>
                        <p>Bạn có thể bắt đầu sử dụng các dịch vụ của chúng tôi ngay bây giờ.</p>
                        <div style=""margin: 30px 0; padding: 20px; background-color: #f8f9fa; border-radius: 5px;"">
                            <h3 style=""color: #333; margin-top: 0;"">Các bước tiếp theo:</h3>
                            <ul style=""color: #666;"">
                                <li>Hoàn thiện thông tin cá nhân</li>
                                <li>Chọn gói tập phù hợp với bạn</li>
                                <li>Đặt lịch tập với huấn luyện viên</li>
                            </ul>
                        </div>
                        <p>Chúc bạn có trải nghiệm tuyệt vời!</p>
                        <hr style=""border: none; border-top: 1px solid #eee; margin: 30px 0;"">
                        <p style=""color: #666;"">Trân trọng,<br><strong>Đội ngũ FittLife</strong></p>
                    </div>
                "
            };

            message.Body = bodyBuilder.ToMessageBody();

            using (var client = new SmtpClient())
            {
                var smtpHost = _configuration["EmailSettings:SmtpHost"];
                var smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"]);
                var smtpUser = _configuration["EmailSettings:SmtpUser"];
                var smtpPassword = _configuration["EmailSettings:SmtpPassword"];

                await client.ConnectAsync(smtpHost, smtpPort, SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(smtpUser, smtpPassword);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
        }

        public async Task SendChangeEmailOtpAsync(string email, string otp)
        {
            var message = new MimeMessage();

            var fromName = _configuration["EmailSettings:FromName"];
            var fromEmail = _configuration["EmailSettings:FromEmail"];
            message.From.Add(new MailboxAddress(fromName, fromEmail));

            message.To.Add(new MailboxAddress("", email));
            message.Subject = "Xác nhận thay đổi email – FittLife";

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = $@"
        <div style=""font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;"">
            <h2 style=""color: #ff9800;"">Xác nhận thay đổi email</h2>
            <p>Bạn vừa yêu cầu <strong>thay đổi địa chỉ email</strong> cho tài khoản FittLife.</p>

            <p>Vui lòng nhập mã OTP bên dưới để xác nhận email mới:</p>

            <div style=""background-color: #f4f4f4; padding: 20px; text-align: center;
                        font-size: 32px; letter-spacing: 5px; margin: 20px 0;
                        font-weight: bold; color: #ff9800;"">
                {otp}
            </div>

            <p style=""color: #666;"">
                Mã OTP có hiệu lực trong vòng <strong>10 phút</strong>.
            </p>

            <p style=""color: #999; font-size: 14px;"">
                Nếu bạn không thực hiện yêu cầu này, vui lòng bỏ qua email hoặc đổi mật khẩu ngay.
            </p>

            <hr style=""border: none; border-top: 1px solid #eee; margin: 30px 0;"">

            <p style=""color: #666;"">
                Trân trọng,<br>
                <strong>Đội ngũ FittLife</strong>
            </p>
        </div>"
            };

            message.Body = bodyBuilder.ToMessageBody();

            using (var client = new SmtpClient())
            {
                var smtpHost = _configuration["EmailSettings:SmtpHost"];
                var smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"]);
                var smtpUser = _configuration["EmailSettings:SmtpUser"];
                var smtpPassword = _configuration["EmailSettings:SmtpPassword"];

                await client.ConnectAsync(smtpHost, smtpPort, SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(smtpUser, smtpPassword);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
        }

        public async Task SendResetPasswordOtpAsync(string email, string otp)
        {
            var message = new MimeMessage();

            var fromName = _configuration["EmailSettings:FromName"];
            var fromEmail = _configuration["EmailSettings:FromEmail"];
            message.From.Add(new MailboxAddress(fromName, fromEmail));

            message.To.Add(new MailboxAddress("", email));
            message.Subject = "Đặt lại mật khẩu FittLife";

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = $@"
        <div style=""font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;"">
            <h2 style=""color: #f44336;"">Yêu cầu đặt lại mật khẩu</h2>

            <p>Chúng tôi nhận được yêu cầu <strong>đặt lại mật khẩu</strong> cho tài khoản FittLife của bạn.</p>

            <p>Vui lòng sử dụng mã OTP bên dưới để tiếp tục:</p>

            <div style=""background-color: #f4f4f4; padding: 20px; text-align: center;
                        font-size: 32px; letter-spacing: 5px; margin: 20px 0;
                        font-weight: bold; color: #f44336;"">
                {otp}
            </div>

            <p style=""color: #666;"">
                Mã OTP có hiệu lực trong vòng <strong>10 phút</strong>.
            </p>

            <p style=""color: #999; font-size: 14px;"">
                Nếu bạn không yêu cầu đặt lại mật khẩu, vui lòng bỏ qua email này
                hoặc đổi mật khẩu ngay để đảm bảo an toàn.
            </p>

            <hr style=""border: none; border-top: 1px solid #eee; margin: 30px 0;"">

            <p style=""color: #666;"">
                Trân trọng,<br>
                <strong>Đội ngũ FittLife</strong>
            </p>
        </div>"
            };

            message.Body = bodyBuilder.ToMessageBody();

            using (var client = new SmtpClient())
            {
                var smtpHost = _configuration["EmailSettings:SmtpHost"];
                var smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"]);
                var smtpUser = _configuration["EmailSettings:SmtpUser"];
                var smtpPassword = _configuration["EmailSettings:SmtpPassword"];

                await client.ConnectAsync(smtpHost, smtpPort, SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(smtpUser, smtpPassword);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
        }
        public async Task SendPasswordChangedNotificationAsync(string email, string name)
        {
            var message = new MimeMessage();

            var fromName = _configuration["EmailSettings:FromName"];
            var fromEmail = _configuration["EmailSettings:FromEmail"];
            message.From.Add(new MailboxAddress(fromName, fromEmail));

            message.To.Add(new MailboxAddress(name, email));
            message.Subject = "Mật khẩu đã được thay đổi";

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = $@"
        <div style=""font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;"">
            <h2 style=""color: #4CAF50;"">Mật khẩu đã được thay đổi</h2>

            <p>Xin chào <strong>{name}</strong>,</p>

            <p>
                Mật khẩu tài khoản FittLife của bạn vừa được <strong>thay đổi thành công</strong>.
            </p>

            <div style=""margin: 20px 0; padding: 15px; background-color: #f8f9fa; border-radius: 5px;"">
                <p style=""margin: 0; color: #333;"">
                    📌 Thời gian: <strong>{DateTime.UtcNow.AddHours(7):dd/MM/yyyy HH:mm}</strong>
                </p>
            </div>

            <p style=""color: #666;"">
                Nếu <strong>không phải bạn</strong> thực hiện thay đổi này,
                vui lòng <strong>liên hệ hỗ trợ ngay</strong> hoặc đặt lại mật khẩu.
            </p>

            <hr style=""border: none; border-top: 1px solid #eee; margin: 30px 0;"">

            <p style=""color: #666;"">
                Trân trọng,<br>
                <strong>Đội ngũ FittLife</strong>
            </p>
        </div>"
            };

            message.Body = bodyBuilder.ToMessageBody();

            using (var client = new SmtpClient())
            {
                var smtpHost = _configuration["EmailSettings:SmtpHost"];
                var smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"]);
                var smtpUser = _configuration["EmailSettings:SmtpUser"];
                var smtpPassword = _configuration["EmailSettings:SmtpPassword"];

                await client.ConnectAsync(smtpHost, smtpPort, SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(smtpUser, smtpPassword);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
        }

    }


}
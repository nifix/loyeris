using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Text.Encodings.Web;
using Loyeris.IdentityAccess.App.Notifications;
using Loyeris.Shared.Configuration;
using Microsoft.Extensions.Options;

namespace Loyeris.IdentityAccess.Infrastructure.Notifications;

/// <summary>
/// Sends branded account verification emails through the configured SMTP server.
/// </summary>
public class SmtpEmailVerificationSender(
    IOptions<ApplicationUrlOptions> applicationUrlOptions,
    IOptions<SmtpOptions> smtpOptions) : IEmailVerificationSender
{
    /// <inheritdoc />
    public async Task SendAsync(
        string email,
        string firstName,
        string plainTextToken,
        CancellationToken cancellationToken)
    {
        var applicationUrls = applicationUrlOptions.Value;
        var smtp = smtpOptions.Value;
        var verificationUrl = $"{applicationUrls.FrontendBaseUrl.TrimEnd('/')}/verify-email?token={Uri.EscapeDataString(plainTextToken)}";

        using var message = CreateMessage(smtp, email, firstName, verificationUrl);
        using var client = new SmtpClient(smtp.Host, smtp.Port);
        client.EnableSsl = smtp.EnableSsl;
        client.DeliveryMethod = SmtpDeliveryMethod.Network;
        client.UseDefaultCredentials = false;

        // Local capture servers usually accept anonymous SMTP, while production can opt into credentials.
        if (!string.IsNullOrWhiteSpace(smtp.Username))
            client.Credentials = new NetworkCredential(smtp.Username, smtp.Password);

        await client.SendMailAsync(message, cancellationToken);
    }

    private static MailMessage CreateMessage(
        SmtpOptions smtp,
        string recipientEmail,
        string firstName,
        string verificationUrl)
    {
        var message = new MailMessage
        {
            From = new MailAddress(smtp.FromAddress, smtp.FromName, Encoding.UTF8),
            Subject = "Vérifiez votre adresse email — Loyeris",
            SubjectEncoding = Encoding.UTF8,
            BodyEncoding = Encoding.UTF8
        };

        message.To.Add(new MailAddress(recipientEmail));

        // Multipart content keeps the message readable in clients that disable or strip HTML.
        message.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(
            CreatePlainTextBody(firstName, verificationUrl),
            Encoding.UTF8,
            MediaTypeNames.Text.Plain));
        message.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(
            CreateHtmlBody(firstName, verificationUrl),
            Encoding.UTF8,
            MediaTypeNames.Text.Html));

        return message;
    }

    private static string CreatePlainTextBody(string firstName, string verificationUrl)
        => $"""
           Bonjour {firstName},

           Bienvenue sur Loyeris ! Votre espace de gestion locative est prêt.

           Vérifiez votre adresse email en ouvrant ce lien :
           {verificationUrl}

           Ce lien est valable pendant 24 heures. Si vous n'êtes pas à l'origine de cette inscription, vous pouvez ignorer cet email.

           L'équipe Loyeris
           Gestion locative claire pour vos SCI
           """;

    private static string CreateHtmlBody(string firstName, string verificationUrl)
    {
        // Both values contain request-derived data and must be encoded before entering HTML markup.
        var safeFirstName = HtmlEncoder.Default.Encode(firstName);
        var safeVerificationUrl = HtmlEncoder.Default.Encode(verificationUrl);

        // Tables and inline styles provide predictable rendering across webmail and desktop clients.
        return $$"""
                 <!doctype html>
                 <html lang="fr">
                   <head>
                     <meta charset="utf-8">
                     <meta name="viewport" content="width=device-width, initial-scale=1">
                     <title>Vérifiez votre adresse email</title>
                   </head>
                   <body style="margin:0;padding:0;background:#f4f7fb;color:#172033;font-family:'Plus Jakarta Sans',Inter,Arial,sans-serif;">
                     <div style="display:none;max-height:0;overflow:hidden;opacity:0;">
                       Activez votre espace Loyeris en vérifiant votre adresse email.
                     </div>
                     <table role="presentation" width="100%" cellspacing="0" cellpadding="0" border="0" style="background:#f4f7fb;">
                       <tr>
                         <td align="center" style="padding:40px 16px;">
                           <table role="presentation" width="100%" cellspacing="0" cellpadding="0" border="0" style="max-width:600px;">
                             <tr>
                               <td style="padding:0 8px 22px;">
                                 <table role="presentation" cellspacing="0" cellpadding="0" border="0">
                                   <tr>
                                     <td style="width:44px;height:44px;border-radius:13px;background:#4b6bfb;color:#ffffff;text-align:center;font-size:22px;font-weight:800;box-shadow:0 8px 20px rgba(75,107,251,.28);">L</td>
                                     <td style="padding-left:13px;">
                                       <div style="font-size:21px;font-weight:800;letter-spacing:-.4px;color:#172033;">Loyeris</div>
                                       <div style="margin-top:2px;font-size:12px;color:#667085;">Gestion locative claire pour vos SCI</div>
                                     </td>
                                   </tr>
                                 </table>
                               </td>
                             </tr>
                             <tr>
                               <td style="overflow:hidden;border:1px solid #e4e9f2;border-radius:22px;background:#ffffff;box-shadow:0 18px 50px rgba(23,32,51,.09);">
                                 <div style="height:7px;background:linear-gradient(90deg,#4b6bfb,#7157d9);"></div>
                                 <div style="padding:42px 42px 36px;">
                                   <div style="display:inline-block;padding:7px 12px;border-radius:999px;background:#edf1ff;color:#3f58d4;font-size:12px;font-weight:700;letter-spacing:.3px;">NOUVEL ESPACE</div>
                                   <h1 style="margin:22px 0 14px;font-size:30px;line-height:1.2;letter-spacing:-.8px;color:#172033;">Bienvenue sur Loyeris, {{safeFirstName}} !</h1>
                                   <p style="margin:0;font-size:16px;line-height:1.7;color:#526078;">Votre espace de gestion locative est prêt. Il ne reste plus qu'à confirmer votre adresse email pour l'activer.</p>
                                   <table role="presentation" width="100%" cellspacing="0" cellpadding="0" border="0" style="margin:30px 0 28px;">
                                     <tr>
                                       <td align="center">
                                         <table role="presentation" cellspacing="0" cellpadding="0" border="0">
                                           <tr>
                                             <td style="border-radius:13px;background:#4b6bfb;box-shadow:0 10px 24px rgba(75,107,251,.28);">
                                               <a href="{{safeVerificationUrl}}" style="display:inline-block;padding:15px 25px;color:#ffffff;text-decoration:none;font-size:15px;font-weight:750;">Vérifier mon adresse email</a>
                                             </td>
                                           </tr>
                                         </table>
                                       </td>
                                     </tr>
                                   </table>
                                   <div style="padding:16px 18px;border-radius:13px;background:#f7f9fc;border:1px solid #e8ecf3;font-size:13px;line-height:1.6;color:#667085;">
                                     Ce lien est valable pendant <strong style="color:#344054;">24 heures</strong>. Si vous n'êtes pas à l'origine de cette inscription, vous pouvez ignorer cet email.
                                   </div>
                                   <p style="margin:28px 0 0;font-size:14px;line-height:1.6;color:#667085;">À très vite,<br><strong style="color:#344054;">L'équipe Loyeris</strong></p>
                                 </div>
                               </td>
                             </tr>
                             <tr>
                               <td align="center" style="padding:22px 20px 0;font-size:11px;line-height:1.6;color:#98a2b3;">
                                 Cet email automatique a été envoyé dans le cadre de la création de votre compte Loyeris.
                               </td>
                             </tr>
                           </table>
                         </td>
                       </tr>
                     </table>
                   </body>
                 </html>
                 """;
    }
}

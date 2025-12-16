using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Helpers
{
    public class EmailTemplateBuilder
    {
        public static string BuildPasswordResetEmail(string userName, string token, string link)
        {
            return $@"
                <div style='font-family: Arial; padding: 20px;'>
                    <h2>Olá, {userName}</h2>
                    <p>Você solicitou a troca de senha.</p>
                    <div style='background: #f4f4f4; padding: 10px; margin: 10px 0;'>
                        <strong>Código: {token}</strong>
                    </div>
                    <p>Ou clique aqui: <a href='{link}'>Redefinir Senha</a></p>
                </div>";
        }
    }
}

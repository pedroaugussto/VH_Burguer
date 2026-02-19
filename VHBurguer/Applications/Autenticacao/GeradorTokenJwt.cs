using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using VHBurguer.Domains;
using VHBurguer.Exceptions;

namespace VHBurguer.Applications.Autenticacao
{
    public class GeradorTokenJwt
    {
        private readonly IConfiguration _config;

        //Recebe as configuracies di appsettings.json
        public GeradorTokenJwt(IConfiguration config)
        {
            _config = config;
        }

        public string GerarToken(Usuario usuario)
        {
            //Key -> chave secreta usada para assinar o token
            // garante que o token nao foi alterado
            var chave = _config["Jwt:Key"]!;

            //issuer -> quem gerou o token (nome da API / Sistema que gerou)
            // a API valida se o token veio do emissor correto
            var issuer = _config["Jwt:Issuer"]!;

            // Audience -> para quem o token foi criado
            // define qual sistema pode usar o token
            var audience = _config["Jwt:Audience"]!;

            // Tempo de Expiricao -> define quantos minutos o token sera valido
            // Depois disso o usuario precisa logar novamente
            var expiracaoEmMinutos = int.Parse(_config["Jwt:ExpiraEmMinutos"]!);

            //Converte a chave para bytes (Necessario para criar a assinatura)
            var keyBytes = Encoding.UTF8.GetBytes(chave);

            // Seguranca: exige uma chave com pelo menos 32 caracteres
            if (keyBytes.Length < 32)
            {
                throw new DomainExceptions("Jwt: Key precisa ter pelo menos 32 caracteres");
            }

            // Cria a chave de seguranca usada para assinar o token
            var securityKey = new SymmetricSecurityKey(keyBytes);

            // Define o algoritmo de assinatura do token
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // Claims -> Informacoes do usuario que vao dentro do token
            // essas informacoes podem ser recuperadas na API para identificar quem esta logado
            var claims = new List<Claim>
            {
                // ID do Usuario para saber quem fez a acao
                new Claim(ClaimTypes.NameIdentifier, usuario.UsuarioID.ToString()),

                // Nome do Usuario
                new Claim(ClaimTypes.Name, usuario.Nome),

                //Email do Usuario
                new Claim(ClaimTypes.Email, usuario.Email)
            };

            //Cria o token Jwt com todas as informacoes
            var token = new JwtSecurityToken(
                    issuer: issuer,                                             // quem gerou token
                    audience: audience,                                         // quem pode usar o token
                    claims: claims,                                             // dados do usuario
                    expires: DateTime.Now.AddMinutes(expiracaoEmMinutos),       // Validade do token
                    signingCredentials: credentials                             // assinatura de seguranca
            );

            // Converte o token para string e essa string eh enviada para o cliente
            return new JwtSecurityTokenHandler().WriteToken(token);

        }
    }
}

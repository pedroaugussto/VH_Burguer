using System.Security.Cryptography;
using System.Text;
using VHBurguer.Domains;
using VHBurguer.DTOs;
using VHBurguer.Exceptions;
using VHBurguer.Interfaces;

namespace VHBurguer.Applications.Services
{
    // Service concentra o "Como fazer"
    public class UsuarioService
    {
        // _repository eh o canal para acessar os dados.
        private readonly IUsuarioRepository _repository;

        // injecao de dependencia
        // implementamos o repositorio e o service so depende da interface
        public UsuarioService(IUsuarioRepository repository)
        {
            _repository = repository;
        }

        //O metodo eh private pq ele nao eh regra de negocio e nao faz sentido existir fora do UsuarioService
        private static LerUsuarioDto LerDto(Usuario usuario) // pega a entidade usuario e gera um DTO
        {
            LerUsuarioDto lerUsuario = new LerUsuarioDto
            {
                UsuarioID = usuario.UsuarioID,
                Nome = usuario.Nome,
                Email = usuario.Email,
                StatusUsuario = usuario.StatusUsuario ?? true // Se nao tiver status no banco, deixa como true
            };
            return lerUsuario;
        }
        public List<LerUsuarioDto> Listar()
        {
            List<Usuario> usuarios = _repository.Listar();

            List<LerUsuarioDto> usuarioDtos = usuarios
                .Select(usuarioBanco => LerDto(usuarioBanco)) //Select que percorre cada usuario e lerDto(usuario)
                .ToList(); //ToList() -> devolve uma lista de DTOs
            return usuarioDtos;
        }

        private static void ValidarEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email) || !email.Contains("@") {
                throw new DomainExceptions("Email invalido.");
            }
        }

        private static byte[] HashSenha(string senha)
        {
            if (string.IsNullOrWhiteSpace(senha)) //garante que a senha nao seja vazia
            {
                throw new DomainExceptions("Senha eh obrigatoria");
            }

            using var sha256 = SHA256.Create(); //gera um hash SHA256 e devolve em byte[]
            return sha256.ComputeHash(Encoding.UTF8.GetBytes(senha));
        }

        public LerUsuarioDto ObterPorId(int id)
        {
            Usuario usuario = _repository.ObterPorId(id);
            {
                if (usuario == null)
                {
                    throw new DomainExceptions("Usuario nao encontrado.");
                }

                return LerDto(usuario); // Se existe usuario, converte para DTO e devolve o usuario
            }
        }
        public LerUsuarioDto ObterPorEmail(int email)
        {
         Usuario? usuario = _repository.ObterPorEmail(email);

            if (usuario == null)
            {
                throw new DomainExceptions("Usuario nao encontrado.");
            }
             return LerDto(usuario); // Se existe usuario, converte para DTO e devolve o usuario
        }

    }
}

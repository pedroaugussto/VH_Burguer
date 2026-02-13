using System.Security.Cryptography;
using System.Text;
using VHBurguer.Domains;
using VHBurguer.DTOs.UsuarioDto;
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
            if (string.IsNullOrWhiteSpace(email) || !email.Contains("@"))
             {
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
        public LerUsuarioDto ObterPorEmail(string email)
        {
            Usuario? usuario = _repository.ObterPorEmail(email);

            if (usuario == null)
            {
                throw new DomainExceptions("Usuario nao encontrado.");
            }
            return LerDto(usuario); // Se existe usuario, converte para DTO e devolve o usuario
        }

        public LerUsuarioDto Adicionar(CriarUsuarioDto usuarioDto)
        {
            ValidarEmail(usuarioDto.Email);

            if (_repository.EmailExiste(usuarioDto.Email))
            {
                throw new DomainExceptions("Ja existe um usuario com esse email.");
            }

            Usuario usuario = new Usuario //Criando entidade Usuario
            {
                Nome = usuarioDto.Nome,
                Email = usuarioDto.Email,
                Senha = HashSenha(usuarioDto.Senha),
                StatusUsuario = true
            };

            _repository.Adicionar(usuario);

            return LerDto(usuario); //retorna LerDto para nao retornar o objeto com a senha
        }

        public LerUsuarioDto Atualizar(int id, CriarUsuarioDto usuarioDto)
        {
    
            Usuario usuarioBanco = _repository.ObterPorId(id);

            if (usuarioBanco == null)
            {
                throw new DomainExceptions("Usuario nao encontrado.");
            }

            ValidarEmail(usuarioDto.Email);

            Usuario usuarioComMesmoEmail = _repository.ObterPorEmail(usuarioDto.Email);
            
            if(usuarioComMesmoEmail != null && usuarioComMesmoEmail.UsuarioID != id)
            {
                throw new DomainExceptions("Ja existe um usuario com esse email.");
            }

            //Substitui as informacoes do banco (usuarioBanco)
            //Inserindo as alteracoes que estao vindo de usuarioDto
            usuarioBanco.Nome = usuarioDto.Nome;
            usuarioBanco.Email = usuarioDto.Email;
            usuarioBanco.Senha = HashSenha(usuarioDto.Senha);

            _repository.Atualizar(usuarioBanco);

            return LerDto(usuarioBanco);
        }

        public void Remover(int id)
        {
            Usuario usuario = _repository.ObterPorId(id);

            if (usuario == null)
            {
                throw new DomainExceptions("Usuario nao encontrado");
            }

            _repository.Remover(id);

        }
    }
}

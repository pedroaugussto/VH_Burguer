using VHBurguer.Domains;

namespace VHBurguer.Interfaces
{
    public interface IUsuarioRepository
    {
        List<Usuario> Listar();
        
        //Pode ser que nao venha nenhum usuario na busca, entao colocamos o "?"

        Usuario? ObterPorId(int id);

        Usuario? ObterPorEmail(string email);

        bool EmailExiste(string email);

        void Adcionar(Usuario usuario);

        void Atualizar(Usuario usuario);

        void Remover(int id);
        Usuario? ObterPorEmail(int email);
    }
}

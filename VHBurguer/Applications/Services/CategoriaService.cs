using VHBurguer.Domains;
using VHBurguer.DTOs.CategoriaDto;
using VHBurguer.Exceptions;
using VHBurguer.Interfaces;

namespace VHBurguer.Applications.Services
{
    public class CategoriaService
    {
        private readonly ICategoriaRepository _repository;

        public CategoriaService(ICategoriaRepository repository)
        {
            _repository = repository;   
        }

        public List<LerCategoriaDto> Listar()
        {
            List<Categoria> categorias = _repository.Listar();

            //converte cada categoria para LerCategoriaDto
            List<LerCategoriaDto> categoriaDto = categorias.Select(categoria => new LerCategoriaDto
            {
                CategoriaId = categoria.CategoriaID,
                Nome = categoria.Nome,
            }).ToList();

            //Retorna a lista ja convertida em Dto
            return categoriaDto;
            
        }

        public LerCategoriaDto ObterPorId(int id)
        {
            Categoria categoria = _repository.ObterPorId(id);

            if (categoria == null)
            {
                throw new DomainExceptions("Categoria nao encontrada");
            }

            LerCategoriaDto categoriaDto = new LerCategoriaDto
            {
                CategoriaId = categoria.CategoriaID,
                Nome = categoria.Nome
            };

            return categoriaDto;
        }

        private static void ValidarNome(string nome)
        {
            if(string.IsNullOrWhiteSpace(nome))
            {
                throw new DomainExceptions("Nome eh obrigatorio");
            }
        }

        public void Adicionar(CriarCategoriaDto criarDto)
        {
            ValidarNome(criarDto.Nome);

            if(_repository.NomeExiste(criarDto.Nome))
            {
                throw new DomainExceptions("Categoria ja existente");
            }

            Categoria categoria = new Categoria
            {
                Nome = criarDto.Nome
            };

            _repository.Adicionar(categoria);
        }

        public void Atualizar(int id, CriarCategoriaDto criarDto)
        {
            ValidarNome(criarDto.Nome); // Valida se o campo nome foi preenchido

            Categoria categoriaBanco = _repository.ObterPorId(id);

            if(categoriaBanco == null)
            {
                throw new DomainExceptions("Categoria nao encontrada");
            }

            // categoriaIdAtual: id -> categoriaIdAtual recebe id
            if(_repository.NomeExiste(criarDto.Nome, categoriaIdAtual: id))
            {
                throw new DomainExceptions("Ja existe outra categoria com esse nome");
            }

            categoriaBanco.Nome = criarDto.Nome;
            _repository.Atualizar(categoriaBanco);
        }

        public void Remover (int id)
        {
            Categoria categoriaBanco = _repository.ObterPorId (id);

            if(categoriaBanco == null)
            {
                throw new DomainExceptions("Categoria nao encontrada");
            }

            _repository.Remover(id);
        }


    }
}

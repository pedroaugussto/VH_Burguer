using VHBurguer.Applications.Conversoes;
using VHBurguer.Applications.Regras;
using VHBurguer.Domains;
using VHBurguer.DTOs.ProdutoDto;
using VHBurguer.Exceptions;
using VHBurguer.Interfaces;

namespace VHBurguer.Applications.Services
{
    public class ProdutoService
    {
        private readonly IProdutoRepository _repositoy;

        public ProdutoService(IProdutoRepository repositoy)
        {
            _repositoy = repositoy;
        }

        //Para cada produto que veio do banco
        //Crie um Dto so com o que a requisicao/front precisa
        public List<LerProdutoDto> Listar()
        {
            List<Produto> produtos = _repositoy.Listar();

            // SELECT percorre cada Produto e transforma em Dto -> LerProdutoDto
            List<LerProdutoDto> produtosDto = produtos.Select(ProdutoParaDto.ConverterParaDto).ToList();

            return produtosDto;
        }

        public LerProdutoDto ObterPorId(int id)
        {
            Produto produto = _repositoy.ObterPorId(id);

            if (produto == null)
            {
                throw new DomainExceptions("Produto nao encontrado");
            }

            //Converte o produto encontrado para Dto e devolve
            return ProdutoParaDto.ConverterParaDto(produto);
        }

        private static void ValidarCadastro(CriarProdutoDto produtoDto)
        {
            if (string.IsNullOrWhiteSpace(produtoDto.Nome))
            {
                throw new DomainExceptions("Nome eh obrigatorio");
            }

            if (produtoDto.Preco < 0)
            {
                throw new DomainExceptions("Preco deve ser maior que zero");
            }

            if (string.IsNullOrWhiteSpace(produtoDto.Descricao))
            {
                throw new DomainExceptions("Descricao eh obrigatoria");
            }

            if (produtoDto.Imagem == null || produtoDto.Imagem.Length == 0)
            {
                throw new DomainExceptions("Imagem eh obrigatoria");
            }

            if (produtoDto.CategoriaIds == null || produtoDto.CategoriaIds.Count == 0)
            {
                throw new DomainExceptions("Produto deve ter ao menos uma categoria");
            }
        }

        public byte[] ObterImagem(int id)
        {
            byte[] imagem = _repositoy.ObterImagem(id);

            if (imagem == null || imagem.Length == 0)
            {
                throw new DomainExceptions("Imagem nao encontrada");
            }

            return imagem;
        }

        public LerProdutoDto Adicionar(CriarProdutoDto produtoDto, int usuarioId)
        {
            ValidarCadastro(produtoDto);

            if(_repositoy.NomeExiste(produtoDto.Nome))
            {
                throw new DomainExceptions("Produto ja existente");
            }

            Produto produto = new Produto
            {
                Nome = produtoDto.Nome,
                Preco = produtoDto.Preco,
                Descricao = produtoDto.Descricao,
                Imagem = ImagemParaBytes.ConverterImagem(produtoDto.Imagem),
                StatusProduto = true,
                UsuarioID = usuarioId
            };

            _repositoy.Adicionar(produto, produtoDto.CategoriaIds);

            return ProdutoParaDto.ConverterParaDto(produto);
        }

        public LerProdutoDto Atualizar(int id, AtualizarProdutoDto produtoDto)
        {
            HorarioAlteracaoProduto.ValidarHorario();

            Produto produtoBanco = _repositoy.ObterPorId(id);

            if (produtoBanco == null)
            {
                throw new DomainExceptions("Produto nao encontrado");
            }

            //dois pontos (:) serve para pasar o valor do parametro
            if(_repositoy.NomeExiste(produtoDto.Nome, produtoIdAtual: id))
            {
                throw new DomainExceptions("Ja existe outro produto com esse nome");
            }

            if (produtoDto.CategoriaIds == null || produtoDto.CategoriaIds.Count == 0)
            {
                throw new DomainExceptions("Produto deve ter ao menos uma categoria.");
            }

            if(produtoDto.Preco < 0)
            {
                throw new DomainExceptions("Preco deve ser maior que zero");
            }

            produtoBanco.Nome = produtoDto.Nome;
            produtoBanco.Preco = produtoDto.Preco;
            produtoBanco.Descricao = produtoDto.Descricao;

            if (produtoDto.Imagem != null && produtoDto.Imagem.Length > 0)
            {
                produtoBanco.Imagem = ImagemParaBytes.ConverterImagem(produtoDto.Imagem);
            }

            if(produtoDto.StatusProduto.HasValue)
            {
                produtoBanco.StatusProduto = produtoDto.StatusProduto.Value;
            }

            _repositoy.Atualizar(produtoBanco, produtoDto.CategoriaIds);

            return ProdutoParaDto.ConverterParaDto(produtoBanco);
        }

        public void Remover(int id)
        {
            HorarioAlteracaoProduto.ValidarHorario();

            Produto produto = _repositoy.ObterPorId(id);

            if (produto == null)
            {
                throw new DomainExceptions("Produto nao encontrado");
            }

            _repositoy.Remover(id);
        }
    }
}

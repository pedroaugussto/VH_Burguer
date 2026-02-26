using VHBurguer.Applications.Regras;
using VHBurguer.Domains;
using VHBurguer.DTOs.PromocaoDto;
using VHBurguer.Exceptions;
using VHBurguer.Interfaces;

namespace VHBurguer.Applications.Services
{
    public class PromocaoService
    {
        private readonly IPromocaoRepository _repository;

        public PromocaoService(IPromocaoRepository repository)
        {
            _repository = repository;  
        }

        public List<LerPromocaoDto> Listar()
        {
            List<Promocao> promocoes = _repository.Listar();

            List<LerPromocaoDto> promocoesDto = promocoes.Select(promocao => 
            new LerPromocaoDto
            {
                    PromocaoId = promocao.PromocaoID,
                    Nome = promocao.Nome, 
                    DataExpiracao = promocao.DataExpiracao,
                    StatusPromocao = promocao.StatusPromocao
            }).ToList();

            return promocoesDto;
        }

        public LerPromocaoDto ObterPorId(int id)
        {
            Promocao promocao = _repository.ObterPorId(id);

            if(promocao == null)
            {
                throw new DomainExceptions("Promocao nao encontrada");
            }

            LerPromocaoDto promocaoDto = new LerPromocaoDto
            {
                PromocaoId = promocao.PromocaoID,
                Nome = promocao.Nome,
                DataExpiracao = promocao.DataExpiracao,
                StatusPromocao = promocao.StatusPromocao
            };

            return promocaoDto;
        }

        public static void ValidarNome (string nome)
        {
            if(string.IsNullOrEmpty(nome))
            {
                throw new DomainExceptions("Nome eh obrigatorio");
            }
        }

        public void Adicionar(CriarPromocaoDto promoDto)
        {
            ValidarNome(promoDto.Nome);
            ValidarDataExpiracaoPromocao.ValidarDataExpiracao(promoDto.DataExpiracao);

            if(_repository.NomeExiste(promoDto.Nome))
            {
                throw new DomainExceptions("Promocao ja existente");
            }

            Promocao promocao = new Promocao
            {
                Nome = promoDto.Nome,
                DataExpiracao = promoDto.DataExpiracao,
                StatusPromocao = promoDto.StatusPromocao
            };

            _repository.Adicionar(promocao);
        }

        public void Atualizar(int id, CriarPromocaoDto promoDto)
        {
            ValidarNome(promoDto.Nome);

            Promocao promocaoBanco = _repository.ObterPorId(id);

            if (promocaoBanco == null)
            {
                throw new DomainExceptions("Promocao nao encontrada");
            }

            if(_repository.NomeExiste(promoDto.Nome, promocaoIdAtual:id))
            {
                throw new DomainExceptions("Ja existe outra promocao com esse nome");
            }

            promocaoBanco.Nome = promoDto.Nome;
            promocaoBanco.DataExpiracao = promoDto.DataExpiracao;
            promocaoBanco.StatusPromocao = promoDto.StatusPromocao;

            _repository.Atualizar(promocaoBanco);
        }

        public void Remover(int id)
        {
            Promocao promocaoBanco = _repository.ObterPorId(id);

            if (promocaoBanco == null)
            {
                throw new DomainExceptions("Promocao nao encontrada");
            }

            _repository.Remover(id);
        }
    }
}

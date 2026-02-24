using Microsoft.Identity.Client;
using VHBurguer.DTOs.PromocaoDto;
using VHBurguer.Exceptions;

namespace VHBurguer.Applications.Regras
{
    public class ValidarDataExpiracaoPromocao
    {
        public static void ValidarDataExpiracao(DateTime dataExpiracao)
        {
            if (dataExpiracao <= DateTime.Now)
            {
                throw new DomainExceptions("Data de expiracao deve ser futura.");
            }
        }
    }
}

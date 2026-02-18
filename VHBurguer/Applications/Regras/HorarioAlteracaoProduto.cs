using VHBurguer.Exceptions;
namespace VHBurguer.Applications.Regras
{
    public class HorarioAlteracaoProduto
    {
        public static void ValidarHorario()
        {
            var agora = DateTime.Now.TimeOfDay;
            var abertura = new TimeSpan(10, 0, 0); //16h
            var fechamento = new TimeSpan(23, 0, 0);
            
            //retorna um true ou false
            var estaAberto = agora >= abertura && agora <= fechamento;

            //Se retornar true
            if (estaAberto)
            {
                throw new DomainExceptions("Produto so pode ser alterado fora do horario de funcionamento");
            }
        }
    }
}

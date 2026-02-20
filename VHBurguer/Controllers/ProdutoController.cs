using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using VHBurguer.Applications.Services;
using VHBurguer.DTOs.ProdutoDto;
using VHBurguer.Exceptions;

namespace VHBurguer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProdutoController : ControllerBase
    {
        private readonly ProdutoService _service;

        public ProdutoController(ProdutoService service)
        {
            _service = service;
        }

        //Autenticacao do usuario
        private int ObterUsuarioIdLogado()
        {
            // Busca no token/claims o valor armazenado como id do usuario
            // ClaimTypes.NameIdentifier geralmente guarda o ID do usuario no JWT
            string? idTexto = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(idTexto))
            {
                throw new DomainExceptions("Usuario nao autenticado");
            }

            //Converte o ID que veio como texto para inteiro
            //Nosso UsuarioID no sistema esta como int
            //as Claims (informacoes do usuario dentro do token) sempre sao armazenadas como texto.
            return int.Parse(idTexto);
        }

        [HttpGet]
        public ActionResult<List<LerProdutoDto>>Listar()
        {
            List<LerProdutoDto> produtos = _service.Listar();

            return Ok(produtos);
            //return StatusCode(200, produtos);
        }

        [HttpGet("{id}")]
        public ActionResult<LerProdutoDto> ObterPorId(int id)
        {
            LerProdutoDto produto = _service.ObterPorId(id);

            if(produto == null)
            {
                //return StatusCode(404);
                return NotFound();
            }

            return Ok(produto);
        }

        // GET -> api/produto/5/imagem
        [HttpGet("{id}/imagem")]
        public ActionResult ObterImagem(int id)
        {
            try
            {
                var imagem = _service.ObterImagem(id);

                // Retorna o arquivo para o navegador
                // "image/jpeg" informa o tipo da imagem (MIME type)
                // O navegador entende que deve renderizar como imagem
                return File(imagem, "image/jpeg");
            }
            catch (DomainExceptions ex)
            {
                return NotFound(ex.Message); // NotFound -> nao encontrado
            }
        }

        [HttpPost]
        //Indica qye receve dados no formato multipart/form-data
        //Necessario quando enviamos arquivos (ex. imagem do produto)
        [Consumes("multipart/form-data")]
        [Authorize] //Exige login para adicionar produtos

        // [FromForm] -> diz que os dados vem do formulario da requisicao
        public ActionResult Adicionar([FromForm]CriarProdutoDto produtoDto)
        {
            try
            {
                int usuarioId = ObterUsuarioIdLogado();

                // O cadastro fica associado ao usuario logado
                _service.Adicionar(produtoDto, usuarioId);

                return StatusCode(201); //Created
            }
            catch (DomainExceptions ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        [Consumes("multipart/form-data")]
        [Authorize]
        public ActionResult Atualizar(int id, [FromForm] AtualizarProdutoDto produtoDto)
        {
            try
            {
                _service.Atualizar(id, produtoDto);
                return NoContent();
            }
            catch (DomainExceptions ex)
            {
                return BadRequest(ex.Message); 
            }
        }

        [HttpDelete("{id}")]
        [Authorize]
        public ActionResult Remover(int id)
        {
            try
            {
                _service.Remover(id);
                return NoContent();
            }
            catch (DomainExceptions ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VHBurguer.Domains;
using VHBurguer.Exceptions;
using VHBurguer.Applications.Services;
using VHBurguer.DTOs.UsuarioDto;

namespace VHBurguer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly UsuarioService _service;

        public UsuarioController(UsuarioService service)
        {
            _service = service;
        }

        // GET -> Lista informacoes
        [HttpGet]
        public ActionResult <List<LerUsuarioDto>> Listar()
        {
            List<LerUsuarioDto> usuarios = _service.Listar();

            // Retorna a lista de usuarios apartir da DTO de Services
            return Ok(usuarios); //OK - HTTP 200 - Deu certo
        }

        [HttpGet("{id}")]
        public ActionResult<LerUsuarioDto> ObterPorId(int id)
        {
            try
            {
                LerUsuarioDto usuario = _service.ObterPorId(id);
                return Ok(usuario);
            }
            catch (DomainExceptions ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("email/{email}")]
        public ActionResult<LerUsuarioDto> ObterPorEmail(string email)
        {
            try
            {
                LerUsuarioDto usuario = _service.ObterPorEmail(email);
                return Ok(usuario);
            }
            catch (DomainExceptions ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Post -> Envia dados (cadastrar, enviar dados...)
        [HttpPost]
        public ActionResult<LerUsuarioDto> Adicionar(CriarUsuarioDto usuarioDto)
        {
            try
            {
                LerUsuarioDto usuarioCriado = _service.Adicionar(usuarioDto);

                return StatusCode(201, usuarioCriado);
            }
            catch (DomainExceptions ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public ActionResult<LerUsuarioDto> Atualizar(int id, CriarUsuarioDto usuarioDto)
        {
            try
            {
                LerUsuarioDto usuarioAtualizado = _service.Atualizar(id, usuarioDto);
                return StatusCode(200, usuarioAtualizado);
            }
            catch(DomainExceptions ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //Remove os dados
        // no nosso banco o delete vai inativar o usuario por conta da trigger (soft delete)
        [HttpDelete("{id}")]
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

using GMTasker.API.Models;
using GMTasker.API.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GMTasker.API.Controllers.Usuario
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuarioController : ControllerBase
    {
        private readonly AppDbContext? _context;
        public UsuarioController(AppDbContext context){
            _context = context!;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UsuarioModel>>> GetUsuario(){
            return await _context!.tb_usuario!.OrderBy(g => g.nome).ToListAsync();
        }

        [HttpGet("{id_usuario}")]
        public async Task<ActionResult<UsuarioModel>> GetUsuario(int id_usuario){
            var usuario = await _context!.tb_usuario!.FindAsync(id_usuario);
            
            if(usuario == null){
                return NotFound();
            }
            return usuario;
        }

        [HttpPut("{id_usuario}")]
        public ActionResult PutUsuario(int id_usuario, UsuarioModel usuario)
        {
            if (id_usuario != usuario.id_usuario)
            {
                return BadRequest();
            }

            var model = _context!.tb_usuario!.FirstOrDefault(x => x.id_usuario == id_usuario);

            if (model == null)
            {
                return NotFound();
            }

            if (BCrypt.Net.BCrypt.Verify(usuario.senha, model!.senha))
            {
                model.nome = usuario.nome;
                model.cpf = usuario.cpf;
                model.telefone = usuario.cpf;
                model.email = usuario.email;

                _context.tb_usuario!.Update(model);

                _context.SaveChanges();
                return Ok(model);
            }
            else
            {
                Console.WriteLine("Nao esta igual!");
                return BadRequest();
            }
        }

        [HttpPut("edit/password/{id_usuario}")]
        public ActionResult PutUsuarioSenha(int id_usuario, UsuarioModel usuario)
        {
            var modelUsuario = _context!.tb_usuario!.FirstOrDefault(x => x.email == usuario.email);
            
            if (usuario.email == "")
            {
                return NotFound("Email não digitado");
            }
            if (modelUsuario != null)
            {
                return NotFound("Esse email já esta cadastrado!");
            }
            if (id_usuario != usuario.id_usuario)
            {
                return BadRequest();
            }

            var model = _context!.tb_usuario!.FirstOrDefault(x => x.id_usuario == id_usuario);

            if (model == null)
            {
                return NotFound();
            }
            // usuario.senha_antiga = BCrypt.Net.BCrypt.HashPassword(usuario.senha_antiga);
            Console.WriteLine("Senha Nova: " + usuario.senha);
            Console.WriteLine("Senha Antiga: " + usuario.senha_antiga);

            if (BCrypt.Net.BCrypt.Verify(usuario.senha_antiga, model!.senha))
            {
                model.senha = BCrypt.Net.BCrypt.HashPassword(usuario.senha);
                model.senha_antiga = BCrypt.Net.BCrypt.HashPassword(usuario.senha_antiga);
                
                _context.tb_usuario!.Update(model);

                _context.SaveChanges();
                return Ok(model);
            }
            else
            {
                Console.WriteLine("A senha não esta igual!");
                return NotFound("A senha não esta igual!");
            }
        }

        [HttpPost]
        public async Task<ActionResult<UsuarioModel>> PostUsuario(UsuarioModel usuario){
            var modelUsuario = _context!.tb_usuario!.FirstOrDefault(x => x.email == usuario.email);
            
            if (usuario.email == "")
            {
                return NotFound("Email não digitado");
            }
            if (modelUsuario != null)
            {
                return NotFound("Esse email já esta cadastrado!");
            }

            usuario.senha = BCrypt.Net.BCrypt.HashPassword(usuario.senha);
            usuario.senha_antiga = usuario.senha;
            
            _context!.tb_usuario!.Add(usuario);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUsuario", new{id = usuario.id_usuario}, usuario);
        }

        [HttpDelete("{id_usuario:int}")]
        public async Task<ActionResult> DeleteUsuario(int id_usuario){
            var usuario = await _context!.tb_usuario!.FindAsync(id_usuario);
            if(usuario == null){
                return NotFound();
            }

            _context.tb_usuario.Remove(usuario);
            await _context.SaveChangesAsync();
            
            return NoContent();
        }
    }
}
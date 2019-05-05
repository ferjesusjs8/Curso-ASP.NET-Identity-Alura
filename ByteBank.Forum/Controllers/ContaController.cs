using ByteBank.Forum.Models;
using ByteBank.Forum.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace ByteBank.Forum.Controllers
{
    public class ContaController : Controller
    {
        private UserManager<UsuarioAplicacao> _userManager;
        public UserManager<UsuarioAplicacao> UserManager
        {
            get
            {
                if (_userManager == null)
                {
                    var contextOwin = HttpContext.GetOwinContext();
                    _userManager = contextOwin.GetUserManager<UserManager<UsuarioAplicacao>>();
                }
                return _userManager;
            }
            set
            {
                _userManager = value;
            }
        }
        public ActionResult Registrar()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Registrar(ContaRegistrarViewModel modelo)
        {
            if (ModelState.IsValid)
            {

                var novoUsuario = new UsuarioAplicacao();

                novoUsuario.UserName = modelo.UserName;
                novoUsuario.Email = modelo.Email;
                novoUsuario.NomeCompleto = modelo.NomeCompleto;

                var usuario = await UserManager.FindByEmailAsync(modelo.Email);

                if (usuario != null)
                    return View("AguardandoConfirmacao");

                var resultado = await UserManager.CreateAsync(novoUsuario, modelo.Senha);

                if (resultado.Succeeded)
                {
                    await EnviarEmailConfirmacaoAsync(novoUsuario);
                    return View("AguardandoConfirmacao");
                }
                else
                    AdicionaErros(resultado);
            }

            return View(modelo);
        }

        public async Task<ActionResult> ConfirmacaoEmail(string userId, string token)
        {
            if (userId == null || token == null)
                return View("Error");

            var resultado = await UserManager.ConfirmEmailAsync(userId, token);

            if (resultado.Succeeded)
                return RedirectToAction("Index", "Home");
            else
                return View("Error");
        }

        private async Task EnviarEmailConfirmacaoAsync(UsuarioAplicacao usuario)
        {
            var token = await UserManager.GenerateEmailConfirmationTokenAsync(usuario.Id);

            var linkCallBack = Url.Action(
                "ConfirmacaoEmail",
                "Conta",
                new { userId = usuario.Id, token = token },
                Request.Url.Scheme);

            await UserManager.SendEmailAsync(
                usuario.Id,
                "Forum ByteBank Fernando JS - Confirmação de Email",
                $"Olá seja muito bem-vindo ao nosso fórum, por favor clique neste {linkCallBack} para que possamos confirmar o seu cadastro");
        }

        private void AdicionaErros(IdentityResult resultado)
        {
            foreach (var erro in resultado.Errors)
                ModelState.AddModelError("", erro);
        }
    }
}
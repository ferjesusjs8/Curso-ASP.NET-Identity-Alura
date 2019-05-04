using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace ByteBank.Forum.App_Start.Identity
{
    public class SenhaValidador : IIdentityValidator<string>
    {
        public int TamanhoMinimo { get; set; }
        public bool CaracteresEspeciais { get; set; }
        public bool LowerCase { get; set; }
        public bool UpperCase { get; set; }
        public bool Digitos { get; set; }
        public async Task<IdentityResult> ValidateAsync(string item)
        {
            var erros = new List<string>();
            if (CaracteresEspeciais && !VerificaCaracteresEspeciais(item))
                erros.Add("A senha deve conter pelo menos 1 caracter especial!");
            if (UpperCase && !VerificaUpperCase(item))
                erros.Add("A senha deve conter pelo menos 1 letra maiúscula!");
            if (LowerCase && !VerificaLowerCase(item))
                erros.Add("A senha deve conter pelo menos 1 letra minúscula!");
            if (Digitos && !VerificaDigitos(item))
                erros.Add("A senha deve conter pelo menos 1 número!");

            if (erros.Any())
                return IdentityResult.Failed(erros.ToArray());

            return IdentityResult.Success;
        }

        private bool VerificaTamanhoMinimo(string senha) => senha?.Length >= TamanhoMinimo;

        private bool VerificaCaracteresEspeciais(string senha) => Regex.IsMatch(senha, @"[~`!@#$%^&*()+=|\\{}':;.,<>/?[\]""_-]");

        private bool VerificaLowerCase(string senha) => senha.Any(char.IsLower);

        private bool VerificaUpperCase(string senha) => senha.Any(char.IsUpper);

        private bool VerificaDigitos(string senha) => senha.Any(char.IsNumber);
    }
}
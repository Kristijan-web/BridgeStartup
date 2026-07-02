using Application.DTO.User;
using Backend;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ASPLAB2.API.JWT
{

    // Cemu sluzi ova klasa?
    // - Da kreira JWT token, to radi preko metode MakeToken

    public class JwtHandler
    {
        private readonly AppSettings _appSettings;


        public JwtHandler(AppSettings appSettings)
        {
            this._appSettings = appSettings;
        }


        public string MakeToken(UserDbDTO user)
        {

            // Kada su sve pozivati ovu metodu
            // - Kada se korisnik uloguje
            // - Kada hocu da refresh-ujem token
            // Za kada hocu da refreshujem token, da li cu onda izvlaciti user-a, pa izvucicu userova podatke iz jwt-a, i tu ce biti njegovi allowedUsecases, tada necu imati ceo user objekat 

            // Koji tip podatka treba da koristim kao parametar ove metode?
            // Da li da primim user-a iz users tabele ili eager loadovanog sa svojim funkcionalnostima
            // Kod logovanja ja svakako odlazim do baze, ako vec odlazim do baze sto tu ne bih dohvatio njegove funkcionalnosti, zasto dohvatim delic user-a pa onda opet u jwtu idem do baze da dohvatim ostale delove, svaka metoda koja bude za sada pozivala ovu metodu ce morati da ode do baze da dohvati usera, posto ide do baze svakako neka onda dohvati i korisnikove funkcionalnosti i to je to, ako vec znam da ce mi trebati sigurno u ovoj metodu.


            Guid tokenGuid = Guid.NewGuid();

            string tokenId = tokenGuid.ToString();

            // kod ispod ce u odnosu na korisnikov role-u dohvatiti njegove funkcionalnosti

            //IEnumerable<string> useCaseIds = user.Role.RoleUseCases.Select(x => x.UseCases.UseCaseId).ToList();


            var claims = new List<Claim>
            {

                 new Claim(JwtRegisteredClaimNames.Iss, _appSettings.JwtSettings.Issuer, ClaimValueTypes.String), // Iss je issuer
                 new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64), // iat je issuedAt

                 new Claim(ClaimTypes.Role, user.Role),
                 new Claim("Id", user.Id.ToString()),
                 new Claim("Username", user.Username),
                 new Claim("Email", user.Email),
                 // mora biblioteka da se instalira
                 new Claim("AllowedUseCases", JsonConvert.SerializeObject(user.AllowedUseCases)),




            };


            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appSettings.JwtSettings.SecretKey));

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);



            var now = DateTime.UtcNow;
            var token = new JwtSecurityToken(
                issuer: _appSettings.JwtSettings.Issuer,
                audience: "Any",
                claims: claims,
                notBefore: now,
                expires: now.AddSeconds(_appSettings.JwtSettings.DurationSeconds),
                signingCredentials: credentials);


            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}

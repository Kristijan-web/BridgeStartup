namespace Backend
{
    public class AppSettings
    {
        public string ConnectionStringSQL { get; set; }
        // treba da definisej jwt settings/ kako to da uradim kad je u pitanju objeat?
        // koji tip podatka bih uopste koristio?
        // string, int, bool, IEnumerble???
        public JwtSettings JwtSettings { get; set; }
    }

    public class JwtSettings
    {
        public string SecretKey { get; set; }
        public string Issuer { get; set; }
        public int DurationSeconds { get; set; }
    }
}

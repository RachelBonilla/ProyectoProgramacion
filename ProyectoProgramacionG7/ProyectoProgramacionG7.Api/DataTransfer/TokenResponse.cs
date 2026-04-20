namespace ProyectoProgramacionG7.Api.DataTransfer
{
    public class TokenResponse
    {
        public bool EsValido { get; set; }
        public string Mensaje { get; set; } = string.Empty;
        public string? Token { get; set; }
    }
}
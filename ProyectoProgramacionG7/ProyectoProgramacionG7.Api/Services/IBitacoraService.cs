namespace ProyectoProgramacionG7.Api.Services
{
    public interface IBitacoraService
    {
        Task RegistrarEvento(
            string tabla,
            string tipoEvento,
            string descripcion,
            string stackTrace,
            string datosAnteriores,
            string datosPosteriores
        );
    }
}
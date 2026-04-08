namespace ProyectoProgramacionG7.Services
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
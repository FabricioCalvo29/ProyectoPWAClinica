namespace Proyecto_PWA_Clinica_API.Services
{
    public interface IUtilitario
    {
        string Encrypt(string texto);
        string Decrypt(string texto);
        void EnviarCorreo(string destinatario, string asunto, string contenido);
    }
}
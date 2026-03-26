namespace Proyecto_PWA_Clinica.Services
{
    public interface IUtilitario
    {
        string Encrypt(string texto);
        string Decrypt(string texto);
    }
}
namespace api_casos.Datos.Interfaces
{
    public interface IRefreshTokenGenerator
    {
        string GenerateToken(string username);
    }
}

namespace smERP.Application.Features.Auth.Commands.Results
{
    public record LoginResult(string Token, string RefreshToken, DateTime RefreshTokenExpirationDate);
}

using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Chat.Utils.Token;

public class JwtBuilder(string _key, int _expirationMinutes)
{
    private readonly string _key = string.IsNullOrWhiteSpace(_key)
            ? throw new ArgumentNullException(nameof(_key))
            : _key;
    private readonly ClaimsIdentity _claimsIdentity = new();
    private DateTime _created = DateTime.UtcNow;
    private DateTime? _expiration;

    public JwtBuilder AddClaim(Claim claim)
    {
        _claimsIdentity.AddClaim(claim);
        return this;
    }

    public JwtBuilder AddClaims(IEnumerable<Claim> claims)
    {
        _claimsIdentity.AddClaims(claims);
        return this;
    }

    public JwtBuilder SetCreationDate(DateTime created)
    {
        _created = created;
        return this;
    }

    public JwtBuilder SetExpiration(DateTime expiration)
    {
        _expiration = expiration;
        return this;
    }

    public SecurityToken Build()
    {
        var expiration = _expiration ?? _created.AddMinutes(_expirationMinutes);
        var keyBytes = Encoding.UTF8.GetBytes(_key);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = _claimsIdentity,
            NotBefore = _created,
            Expires = expiration,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(keyBytes), SecurityAlgorithms.HmacSha256Signature)
        };

        return new JwtSecurityTokenHandler().CreateToken(tokenDescriptor);
    }
}
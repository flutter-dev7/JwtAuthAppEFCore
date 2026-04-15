using JwtAuthApp.Application.Interfaces.Repositories;
using JwtAuthApp.Application.Interfaces.Services;
using JwtAuthApp.Infrastructure.Data;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    public IUserRepository UserRepository { get; }
    public IVerificationCodeRepository VerificationCodeRepository { get; }

    public IEmailService Email { get; }
    public IJwtService Jwt { get; }

    public UnitOfWork(
        AppDbContext context,
        IUserRepository userRepository,
        IVerificationCodeRepository verificationCodeRepository,
        IEmailService emailService,
        IJwtService jwtService)
    {
        _context = context;
        UserRepository = userRepository;
        VerificationCodeRepository = verificationCodeRepository;
        Email = emailService;
        Jwt = jwtService;
    }

    public async Task<int> SaveChangeAsync()
    {
        return await _context.SaveChangesAsync();
    }
}
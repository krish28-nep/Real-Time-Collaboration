using RealTimeCollaboration.Data;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RealTimeCollaboration.Modules.Auth;
using RealTimeCollaboration.Modules.Auth.Interfaces;
using RealTimeCollaboration.Modules.Channel;
using RealTimeCollaboration.Modules.Channel.Interfaces;
using RealTimeCollaboration.Modules.Message;
using RealTimeCollaboration.Modules.Message.Interfaces;
using RealTimeCollaboration.Modules.Reaction;
using RealTimeCollaboration.Modules.Reaction.Interfaces;
using RealTimeCollaboration.Modules.SignalR;
using RealTimeCollaboration.Modules.User;
using RealTimeCollaboration.Modules.User.Interfaces;
using RealTimeCollaboration.Modules.WorkSpace;
using RealTimeCollaboration.Modules.WorkSpace.Interfaces;

var builder = WebApplication.CreateBuilder(args);
const string accessTokenCookieName = "accessToken";

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddSignalR();
var corsOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>();
if (corsOrigins == null || corsOrigins.Length == 0)
{
    corsOrigins = ["http://localhost:3000", "http://localhost:3001"];
}

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy
            .WithOrigins(corsOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException("Jwt:Key is not configured.");
var jwtIssuer = builder.Configuration["Jwt:Issuer"]
    ?? throw new InvalidOperationException("Jwt:Issuer is not configured.");
var jwtAudience = builder.Configuration["Jwt:Audience"]
    ?? throw new InvalidOperationException("Jwt:Audience is not configured.");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                if (string.IsNullOrEmpty(context.Token))
                {
                    var accessToken = context.Request.Query["access_token"];
                    var path = context.HttpContext.Request.Path;

                    if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/api/signalr"))
                    {
                        context.Token = accessToken;
                    }
                    else if (context.Request.Cookies.TryGetValue(accessTokenCookieName, out var cookieAccessToken))
                    {
                        context.Token = cookieAccessToken;
                    }
                }

                return Task.CompletedTask;
            }
        };
    });
builder.Services.AddAuthorization();

builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IWorkSpaceRepository, WorkSpaceRepository>();
builder.Services.AddScoped<IWorkSpaceService, WorkSpaceService>();
builder.Services.AddScoped<IChannelRepository, ChannelRepository>();
builder.Services.AddScoped<IChannelService, ChannelService>();
builder.Services.AddScoped<IMessageRepository, MessageRepository>();
builder.Services.AddScoped<IMessageService, MessageService>();
builder.Services.AddScoped<IReactionRepository, ReactionRepository>();
builder.Services.AddScoped<IReactionService, ReactionService>();
builder.Services.AddScoped<IPasswordHasher<RealTimeCollaboration.Modules.User.Models.User>, PasswordHasher<RealTimeCollaboration.Modules.User.Models.User>>();
builder.Services.AddScoped<RealTimeCollaboration.Modules.Invitation.Interfaces.IInvitationRepository, RealTimeCollaboration.Modules.Invitation.InvitationRepository>();
builder.Services.AddScoped<RealTimeCollaboration.Modules.Invitation.Interfaces.IInvitationService, RealTimeCollaboration.Modules.Invitation.InvitationService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    if (db.Database.GetPendingMigrations().Any())
    {
        db.Database.Migrate();
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseHttpsRedirection();
}
app.UseStaticFiles();

app.UseCors("Frontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<ChatHub>("/api/signalr");

app.Run();

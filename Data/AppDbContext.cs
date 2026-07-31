using Microsoft.EntityFrameworkCore;
using RealTimeCollaboration.Modules.Channel.Models;
using RealTimeCollaboration.Modules.Invitation.Models;
using RealTimeCollaboration.Modules.Message.Models;
using RealTimeCollaboration.Modules.Reaction.Models;
using RealTimeCollaboration.Modules.User.Models;
using RealTimeCollaboration.Modules.WorkSpace.Models;

namespace RealTimeCollaboration.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users => Set<User>();
        public DbSet<WorkSpace> WorkSpaces => Set<WorkSpace>();

        public DbSet<UserWorkSpace> UserWorkSpaces => Set<UserWorkSpace>();
        public DbSet<Invitation> Invitations => Set<Invitation>();

        public DbSet<Channel> Channels => Set<Channel>();
        public DbSet<Message> Messages => Set<Message>();
        public DbSet<Reaction> Reactions => Set<Reaction>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<WorkSpace>()
                .HasOne(workspace => workspace.Owner)
                .WithMany(user => user.WorkSpaces)
                .HasForeignKey(workspace => workspace.OwnerId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserWorkSpace>()
                .HasKey(userWorkSpace => new { userWorkSpace.UserId, userWorkSpace.WorkSpaceById });

            modelBuilder.Entity<UserWorkSpace>()
                .HasOne(userWorkSpace => userWorkSpace.User)
                .WithMany()
                .HasForeignKey(userWorkSpace => userWorkSpace.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserWorkSpace>()
                .HasOne(userWorkSpace => userWorkSpace.WorkSpace)
                .WithMany()
                .HasForeignKey(userWorkSpace => userWorkSpace.WorkSpaceById)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Channel>()
            .HasOne(channel => channel.WorkSpace)
            .WithMany(workSpace => workSpace.Channels)
            .HasForeignKey(channel => channel.WorkSpaceId)
            .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Message>()
                .HasOne(message => message.Channel)
                .WithMany(channel => channel.Messages)
                .HasForeignKey(message => message.ChannelId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Message>()
                .HasOne(message => message.User)
                .WithMany(user => user.Messages)
                .HasForeignKey(message => message.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Message>()
                .HasIndex(message => new { message.ChannelId, message.CreatedAt, message.Id });

            modelBuilder.Entity<Reaction>()
                .HasOne(reaction => reaction.Message)
                .WithMany(message => message.Reactions)
                .HasForeignKey(reaction => reaction.MessageId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Reaction>()
                .HasOne(reaction => reaction.User)
                .WithMany(user => user.Reactions)
                .HasForeignKey(reaction => reaction.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Reaction>()
                .HasIndex(reaction => new { reaction.MessageId, reaction.UserId, reaction.Emoji })
                .IsUnique();

            modelBuilder.Entity<Invitation>()
                .HasOne(invitation => invitation.WorkSpace)
                .WithMany()
                .HasForeignKey(invitation => invitation.WorkSpaceId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Invitation>()
                .HasOne(invitation => invitation.InvitedByUser)
                .WithMany()
                .HasForeignKey(invitation => invitation.InvitedByUserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Invitation>()
                .HasOne(invitation => invitation.InvitedUser)
                .WithMany()
                .HasForeignKey(invitation => invitation.InvitedUserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

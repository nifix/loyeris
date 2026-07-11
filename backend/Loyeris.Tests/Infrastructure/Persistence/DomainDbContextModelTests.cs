using FluentAssertions;
using Loyeris.IdentityAccess.Core.Entities;
using Loyeris.IdentityAccess.Infrastructure.Persistence;
using Loyeris.Leasing.Core.Entities;
using Loyeris.Leasing.Infrastructure.Persistence;
using Loyeris.Messaging.Core.Entities;
using Loyeris.Messaging.Infrastructure.Persistence;
using Loyeris.Portfolio.Core.Entities;
using Loyeris.Portfolio.Infrastructure.Persistence;
using Loyeris.RentCollection.Core.Entities;
using Loyeris.RentCollection.Infrastructure.Persistence;
using Loyeris.TaxPreparation.Core.Entities;
using Loyeris.TaxPreparation.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Loyeris.Tests.Infrastructure.Persistence;

/// <summary>
/// Ensures domain <see cref="DbContext"/> models expose the expected DAL shape.
/// </summary>
public class DomainDbContextModelTests
{
    private const string ConnectionString = "Host=localhost;Port=5432;Database=loyeris;Username=postgres;Password=moobidoo";

    /// <summary>
    /// Verifies every domain context maps its owned tables to its expected PostgreSQL schema.
    /// </summary>
    [Test]
    public void DbContexts_ShouldMapTables_ToExpectedSchemas()
    {
        // Arrange
        using var identityAccess = CreateIdentityAccessDbContext();
        using var portfolio = CreatePortfolioDbContext();
        using var leasing = CreateLeasingDbContext();
        using var rentCollection = CreateRentCollectionDbContext();
        using var taxPreparation = CreateTaxPreparationDbContext();
        using var messaging = CreateMessagingDbContext();

        // Act
        var mappings = new[]
        {
            GetTable(identityAccess, typeof(AppUser)),
            GetTable(identityAccess, typeof(AuthSession)),
            GetTable(identityAccess, typeof(RefreshToken)),
            GetTable(identityAccess, typeof(AuthOneTimeToken)),
            GetTable(identityAccess, typeof(AuthEvent)),
            GetTable(identityAccess, typeof(Workspace)),
            GetTable(identityAccess, typeof(WorkspaceMember)),
            GetTable(identityAccess, typeof(UserPreference)),
            GetTable(portfolio, typeof(Sci)),
            GetTable(portfolio, typeof(SciAssociate)),
            GetTable(portfolio, typeof(Lot)),
            GetTable(leasing, typeof(Tenant)),
            GetTable(leasing, typeof(Lease)),
            GetTable(leasing, typeof(LeaseTenant)),
            GetTable(rentCollection, typeof(RentDeadline)),
            GetTable(rentCollection, typeof(RentPayment)),
            GetTable(rentCollection, typeof(RentReminder)),
            GetTable(taxPreparation, typeof(RentalExpense)),
            GetTable(taxPreparation, typeof(FiscalPeriod)),
            GetTable(messaging, typeof(NotificationPreference)),
            GetTable(messaging, typeof(OutboxMessage))
        };

        // Assert
        mappings.Should().Contain(("identity", "app_users"));
        mappings.Should().Contain(("identity", "auth_sessions"));
        mappings.Should().Contain(("identity", "refresh_tokens"));
        mappings.Should().Contain(("identity", "auth_one_time_tokens"));
        mappings.Should().Contain(("identity", "auth_events"));
        mappings.Should().Contain(("identity", "workspaces"));
        mappings.Should().Contain(("identity", "workspace_members"));
        mappings.Should().Contain(("identity", "user_preferences"));
        mappings.Should().Contain(("portfolio", "scis"));
        mappings.Should().Contain(("portfolio", "sci_associates"));
        mappings.Should().Contain(("portfolio", "lots"));
        mappings.Should().Contain(("leasing", "tenants"));
        mappings.Should().Contain(("leasing", "leases"));
        mappings.Should().Contain(("leasing", "lease_tenants"));
        mappings.Should().Contain(("collection", "rent_deadlines"));
        mappings.Should().Contain(("collection", "rent_payments"));
        mappings.Should().Contain(("collection", "rent_reminders"));
        mappings.Should().Contain(("tax", "rental_expenses"));
        mappings.Should().Contain(("tax", "fiscal_periods"));
        mappings.Should().Contain(("messaging", "notification_preferences"));
        mappings.Should().Contain(("messaging", "outbox_messages"));
    }

    /// <summary>
    /// Verifies the principal unique indexes required by the domain DAL are configured.
    /// </summary>
    [Test]
    public void Models_ShouldDefine_PrincipalUniqueIndexes()
    {
        // Arrange
        using var identityAccess = CreateIdentityAccessDbContext();
        using var portfolio = CreatePortfolioDbContext();
        using var rentCollection = CreateRentCollectionDbContext();

        // Act
        var appUserEmailIndex = FindIndex(GetEntity(identityAccess, typeof(AppUser)), nameof(AppUser.NormalizedEmail));
        var refreshTokenHashIndex = FindIndex(GetEntity(identityAccess, typeof(RefreshToken)), nameof(RefreshToken.TokenHash));
        var oneTimeTokenHashIndex = FindIndex(
            GetEntity(identityAccess, typeof(AuthOneTimeToken)),
            nameof(AuthOneTimeToken.TokenHash));
        var workspaceMemberIndex = FindIndex(
            GetEntity(identityAccess, typeof(WorkspaceMember)),
            nameof(WorkspaceMember.WorkspaceId),
            nameof(WorkspaceMember.UserId));
        var sciNameIndex = FindIndex(GetEntity(portfolio, typeof(Sci)), nameof(Sci.WorkspaceId), nameof(Sci.Name));
        var lotReferenceIndex = FindIndex(GetEntity(portfolio, typeof(Lot)), nameof(Lot.SciId), nameof(Lot.Reference));
        var rentDeadlineMonthIndex = FindIndex(
            GetEntity(rentCollection, typeof(RentDeadline)),
            nameof(RentDeadline.LeaseId),
            nameof(RentDeadline.PeriodMonth));

        // Assert
        appUserEmailIndex.IsUnique.Should().BeTrue();
        refreshTokenHashIndex.IsUnique.Should().BeTrue();
        oneTimeTokenHashIndex.IsUnique.Should().BeTrue();
        workspaceMemberIndex.IsUnique.Should().BeTrue();
        sciNameIndex.IsUnique.Should().BeTrue();
        lotReferenceIndex.IsUnique.Should().BeTrue();
        rentDeadlineMonthIndex.IsUnique.Should().BeTrue();
    }

    /// <summary>
    /// Verifies <see cref="Lease"/> prevents two active leases for the same lot.
    /// </summary>
    [Test]
    public void Lease_ShouldDefine_UniqueFilteredIndex_ForActiveLeasePerLot()
    {
        // Arrange
        using var leasing = CreateLeasingDbContext();

        // Act
        var leaseEntity = GetEntity(leasing, typeof(Lease));
        var activeLeaseIndex = FindIndex(leaseEntity, nameof(Lease.LotId));

        // Assert
        activeLeaseIndex.IsUnique.Should().BeTrue();
        activeLeaseIndex.GetFilter().Should().Be("status = 'Active'");
    }

    /// <summary>
    /// Verifies <see cref="RefreshToken"/> prevents more than one active refresh token per session.
    /// </summary>
    [Test]
    public void RefreshToken_ShouldDefine_UniqueFilteredIndex_ForActiveTokenPerSession()
    {
        // Arrange
        using var identityAccess = CreateIdentityAccessDbContext();

        // Act
        var refreshTokenEntity = GetEntity(identityAccess, typeof(RefreshToken));
        var activeRefreshTokenIndex = FindIndex(refreshTokenEntity, nameof(RefreshToken.SessionId));

        // Assert
        activeRefreshTokenIndex.IsUnique.Should().BeTrue();
        activeRefreshTokenIndex.GetFilter().Should().Be("consumed_at IS NULL AND revoked_at IS NULL");
    }

    /// <summary>
    /// Verifies <see cref="AuthOneTimeToken"/> prevents duplicate active tokens for a user and purpose.
    /// </summary>
    [Test]
    public void AuthOneTimeToken_ShouldDefine_UniqueFilteredIndex_ForActiveTokenPerUserAndPurpose()
    {
        // Arrange
        using var identityAccess = CreateIdentityAccessDbContext();

        // Act
        var oneTimeTokenEntity = GetEntity(identityAccess, typeof(AuthOneTimeToken));
        var activeOneTimeTokenIndex = FindIndex(
            oneTimeTokenEntity,
            nameof(AuthOneTimeToken.UserId),
            nameof(AuthOneTimeToken.Purpose));

        // Assert
        activeOneTimeTokenIndex.IsUnique.Should().BeTrue();
        activeOneTimeTokenIndex.GetFilter().Should().Be("consumed_at IS NULL AND revoked_at IS NULL");
    }

    /// <summary>
    /// Verifies email verification consumption uses optimistic concurrency protection.
    /// </summary>
    [Test]
    public void AuthOneTimeToken_ShouldUse_ConsumedAtAsConcurrencyToken()
    {
        // Arrange
        using var identityAccess = CreateIdentityAccessDbContext();

        // Act
        var consumedAt = GetEntity(identityAccess, typeof(AuthOneTimeToken))
            .FindProperty(nameof(AuthOneTimeToken.ConsumedAt));

        // Assert
        consumedAt.Should().NotBeNull();
        consumedAt!.IsConcurrencyToken.Should().BeTrue();
    }

    /// <summary>
    /// Verifies refresh-token rotation uses optimistic concurrency protection.
    /// </summary>
    [Test]
    public void RefreshToken_ShouldUse_ConsumedAtAsConcurrencyToken()
    {
        // Arrange
        using var identityAccess = CreateIdentityAccessDbContext();

        // Act
        var consumedAt = GetEntity(identityAccess, typeof(RefreshToken))
            .FindProperty(nameof(RefreshToken.ConsumedAt));

        // Assert
        consumedAt.Should().NotBeNull();
        consumedAt!.IsConcurrencyToken.Should().BeTrue();
    }

    /// <summary>
    /// Verifies enum status properties are stored as text values.
    /// </summary>
    [Test]
    public void StatusEnums_ShouldBeMapped_ToTextColumns()
    {
        // Arrange
        using var identityAccess = CreateIdentityAccessDbContext();
        using var portfolio = CreatePortfolioDbContext();
        using var leasing = CreateLeasingDbContext();
        using var rentCollection = CreateRentCollectionDbContext();
        using var taxPreparation = CreateTaxPreparationDbContext();
        using var messaging = CreateMessagingDbContext();

        // Act
        var enumMappings = new[]
        {
            GetProviderType(identityAccess, typeof(AppUser), nameof(AppUser.Status)),
            GetProviderType(identityAccess, typeof(AuthSession), nameof(AuthSession.Status)),
            GetProviderType(identityAccess, typeof(AuthOneTimeToken), nameof(AuthOneTimeToken.Purpose)),
            GetProviderType(identityAccess, typeof(AuthEvent), nameof(AuthEvent.Type)),
            GetProviderType(identityAccess, typeof(Workspace), nameof(Workspace.Status)),
            GetProviderType(portfolio, typeof(Sci), nameof(Sci.Status)),
            GetProviderType(portfolio, typeof(Sci), nameof(Sci.TaxRegime)),
            GetProviderType(portfolio, typeof(Lot), nameof(Lot.Type)),
            GetProviderType(portfolio, typeof(Lot), nameof(Lot.Status)),
            GetProviderType(leasing, typeof(Tenant), nameof(Tenant.Status)),
            GetProviderType(leasing, typeof(Lease), nameof(Lease.Status)),
            GetProviderType(leasing, typeof(LeaseTenant), nameof(LeaseTenant.Role)),
            GetProviderType(rentCollection, typeof(RentDeadline), nameof(RentDeadline.Status)),
            GetProviderType(rentCollection, typeof(RentPayment), nameof(RentPayment.Method)),
            GetProviderType(rentCollection, typeof(RentReminder), nameof(RentReminder.Channel)),
            GetProviderType(rentCollection, typeof(RentReminder), nameof(RentReminder.Status)),
            GetProviderType(taxPreparation, typeof(FiscalPeriod), nameof(FiscalPeriod.Status)),
            GetProviderType(messaging, typeof(OutboxMessage), nameof(OutboxMessage.Status))
        };

        // Assert
        enumMappings.Should().OnlyContain(providerType => providerType == typeof(string));
    }

    /// <summary>
    /// Verifies the primary read indexes for list, dashboard, rent and outbox screens exist.
    /// </summary>
    [Test]
    public void Models_ShouldDefine_OperationalReadIndexes()
    {
        // Arrange
        using var identityAccess = CreateIdentityAccessDbContext();
        using var portfolio = CreatePortfolioDbContext();
        using var leasing = CreateLeasingDbContext();
        using var rentCollection = CreateRentCollectionDbContext();
        using var messaging = CreateMessagingDbContext();

        // Act
        var sessionListIndex = FindIndex(
            GetEntity(identityAccess, typeof(AuthSession)),
            nameof(AuthSession.UserId),
            nameof(AuthSession.Status),
            nameof(AuthSession.LastSeenAt));
        var sessionExpiryIndex = FindIndex(
            GetEntity(identityAccess, typeof(AuthSession)),
            nameof(AuthSession.Status),
            nameof(AuthSession.ExpiresAt));
        var refreshExpiryIndex = FindIndex(GetEntity(identityAccess, typeof(RefreshToken)), nameof(RefreshToken.ExpiresAt));
        var oneTimeTokenExpiryIndex = FindIndex(
            GetEntity(identityAccess, typeof(AuthOneTimeToken)),
            nameof(AuthOneTimeToken.ExpiresAt));
        var authEventUserIndex = FindIndex(
            GetEntity(identityAccess, typeof(AuthEvent)),
            nameof(AuthEvent.UserId),
            nameof(AuthEvent.OccurredAt));
        var authEventEmailIndex = FindIndex(
            GetEntity(identityAccess, typeof(AuthEvent)),
            nameof(AuthEvent.NormalizedEmail),
            nameof(AuthEvent.OccurredAt));
        var authEventTypeIndex = FindIndex(
            GetEntity(identityAccess, typeof(AuthEvent)),
            nameof(AuthEvent.Type),
            nameof(AuthEvent.OccurredAt));
        var sciStatusIndex = FindIndex(GetEntity(portfolio, typeof(Sci)), nameof(Sci.WorkspaceId), nameof(Sci.Status));
        var lotStatusIndex = FindIndex(GetEntity(portfolio, typeof(Lot)), nameof(Lot.SciId), nameof(Lot.Status));
        var tenantListIndex = FindIndex(
            GetEntity(leasing, typeof(Tenant)),
            nameof(Tenant.WorkspaceId),
            nameof(Tenant.Status),
            nameof(Tenant.LastName));
        var deadlinePeriodIndex = FindIndex(
            GetEntity(rentCollection, typeof(RentDeadline)),
            nameof(RentDeadline.PeriodMonth),
            nameof(RentDeadline.Status));
        var deadlineDueIndex = FindIndex(
            GetEntity(rentCollection, typeof(RentDeadline)),
            nameof(RentDeadline.DueOn),
            nameof(RentDeadline.Status));
        var outboxIndex = FindIndex(
            GetEntity(messaging, typeof(OutboxMessage)),
            nameof(OutboxMessage.Status),
            nameof(OutboxMessage.AvailableAt));

        // Assert
        sessionListIndex.Should().NotBeNull();
        sessionExpiryIndex.Should().NotBeNull();
        refreshExpiryIndex.Should().NotBeNull();
        oneTimeTokenExpiryIndex.Should().NotBeNull();
        authEventUserIndex.Should().NotBeNull();
        authEventEmailIndex.Should().NotBeNull();
        authEventTypeIndex.Should().NotBeNull();
        sciStatusIndex.Should().NotBeNull();
        lotStatusIndex.Should().NotBeNull();
        tenantListIndex.Should().NotBeNull();
        deadlinePeriodIndex.Should().NotBeNull();
        deadlineDueIndex.Should().NotBeNull();
        outboxIndex.Should().NotBeNull();
    }

    /// <summary>
    /// Verifies authentication relationships are owned by the Identity Access model.
    /// </summary>
    [Test]
    public void AuthenticationEntities_ShouldDefine_IdentityAccessRelationships()
    {
        // Arrange
        using var identityAccess = CreateIdentityAccessDbContext();

        // Act
        var authSessionForeignKeys = GetEntity(identityAccess, typeof(AuthSession)).GetForeignKeys();
        var refreshTokenForeignKeys = GetEntity(identityAccess, typeof(RefreshToken)).GetForeignKeys();
        var oneTimeTokenForeignKeys = GetEntity(identityAccess, typeof(AuthOneTimeToken)).GetForeignKeys();
        var authEventForeignKeys = GetEntity(identityAccess, typeof(AuthEvent)).GetForeignKeys();

        // Assert
        authSessionForeignKeys.Should().Contain(foreignKey => foreignKey.PrincipalEntityType.ClrType == typeof(AppUser));
        refreshTokenForeignKeys.Should().Contain(foreignKey => foreignKey.PrincipalEntityType.ClrType == typeof(AuthSession));
        oneTimeTokenForeignKeys.Should().Contain(foreignKey => foreignKey.PrincipalEntityType.ClrType == typeof(AppUser));
        authEventForeignKeys.Should().Contain(foreignKey => foreignKey.PrincipalEntityType.ClrType == typeof(AppUser));
        authEventForeignKeys.Should().Contain(foreignKey => foreignKey.PrincipalEntityType.ClrType == typeof(AuthSession));
    }

    /// <summary>
    /// Verifies authentication tokens are only persisted as hashes.
    /// </summary>
    [Test]
    public void AuthenticationTokens_ShouldPersist_HashesOnly()
    {
        // Arrange
        using var identityAccess = CreateIdentityAccessDbContext();

        // Act
        var refreshTokenEntity = GetEntity(identityAccess, typeof(RefreshToken));
        var oneTimeTokenEntity = GetEntity(identityAccess, typeof(AuthOneTimeToken));

        // Assert
        refreshTokenEntity.FindProperty("Token").Should().BeNull();
        refreshTokenEntity.FindProperty(nameof(RefreshToken.TokenHash)).Should().NotBeNull();
        oneTimeTokenEntity.FindProperty("Token").Should().BeNull();
        oneTimeTokenEntity.FindProperty(nameof(AuthOneTimeToken.TokenHash)).Should().NotBeNull();
    }

    private static IdentityAccessDbContext CreateIdentityAccessDbContext()
    {
        var options = new DbContextOptionsBuilder<IdentityAccessDbContext>()
            .UseNpgsql(ConnectionString)
            .Options;

        return new IdentityAccessDbContext(options);
    }

    private static PortfolioDbContext CreatePortfolioDbContext()
    {
        var options = new DbContextOptionsBuilder<PortfolioDbContext>()
            .UseNpgsql(ConnectionString)
            .Options;

        return new PortfolioDbContext(options);
    }

    private static LeasingDbContext CreateLeasingDbContext()
    {
        var options = new DbContextOptionsBuilder<LeasingDbContext>()
            .UseNpgsql(ConnectionString)
            .Options;

        return new LeasingDbContext(options);
    }

    private static RentCollectionDbContext CreateRentCollectionDbContext()
    {
        var options = new DbContextOptionsBuilder<RentCollectionDbContext>()
            .UseNpgsql(ConnectionString)
            .Options;

        return new RentCollectionDbContext(options);
    }

    private static TaxPreparationDbContext CreateTaxPreparationDbContext()
    {
        var options = new DbContextOptionsBuilder<TaxPreparationDbContext>()
            .UseNpgsql(ConnectionString)
            .Options;

        return new TaxPreparationDbContext(options);
    }

    private static MessagingDbContext CreateMessagingDbContext()
    {
        var options = new DbContextOptionsBuilder<MessagingDbContext>()
            .UseNpgsql(ConnectionString)
            .Options;

        return new MessagingDbContext(options);
    }

    private static (string? Schema, string? Table) GetTable(DbContext context, Type entityType)
    {
        var entity = GetEntity(context, entityType);

        return (entity.GetSchema(), entity.GetTableName());
    }

    private static IEntityType GetEntity(DbContext context, Type entityType)
    {
        return context.Model.FindEntityType(entityType)
               ?? throw new InvalidOperationException($"Entity {entityType.Name} was not found in the EF model.");
    }

    private static IIndex FindIndex(IEntityType entityType, params string[] propertyNames)
    {
        return entityType.GetIndexes().Single(index =>
            index.Properties.Select(property => property.Name).SequenceEqual(propertyNames));
    }

    private static Type? GetProviderType(DbContext context, Type entityType, string propertyName)
    {
        var property = GetEntity(context, entityType).FindProperty(propertyName)
                       ?? throw new InvalidOperationException($"Property {propertyName} was not found on {entityType.Name}.");

        return property.GetTypeMapping().Converter?.ProviderClrType;
    }
}

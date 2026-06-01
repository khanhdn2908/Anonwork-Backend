using Anonwork.Application.Features.UserSubscriptions;
using Anonwork.Application.Features.UserSubscriptions.DTOs;
using Anonwork.Application.Interfaces;
using Anonwork.Domain.Entities;
using Anonwork.Domain.Enums;
using Moq;

namespace Anonwork.Application.Tests;

public class CreateUserSubscriptionUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_WhenUserDoesNotExist_ThrowsArgumentException()
    {
        var unitOfWork = CreateUnitOfWorkMock();
        var userRepository = CreateRepositoryMock<User>();
        var planRepository = CreateRepositoryMock<SubscriptionPlan>();
        var subscriptionRepository = CreateRepositoryMock<UserSubscription>();

        unitOfWork.Setup(x => x.GetRepository<User>()).Returns(userRepository.Object);
        unitOfWork.Setup(x => x.GetRepository<SubscriptionPlan>()).Returns(planRepository.Object);
        unitOfWork.Setup(x => x.GetRepository<UserSubscription>()).Returns(subscriptionRepository.Object);

        userRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var useCase = new CreateUserSubscriptionUseCase(unitOfWork.Object);
        var request = new CreateUserSubscriptionRequestDto(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());

        var ex = await Assert.ThrowsAsync<ArgumentException>(() => useCase.ExecuteAsync(request));

        Assert.Contains("User with ID", ex.Message);
        unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        subscriptionRepository.Verify(x => x.AddAsync(It.IsAny<UserSubscription>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_WhenPlanDoesNotExist_ThrowsArgumentException()
    {
        var unitOfWork = CreateUnitOfWorkMock();
        var userRepository = CreateRepositoryMock<User>();
        var planRepository = CreateRepositoryMock<SubscriptionPlan>();
        var subscriptionRepository = CreateRepositoryMock<UserSubscription>();

        unitOfWork.Setup(x => x.GetRepository<User>()).Returns(userRepository.Object);
        unitOfWork.Setup(x => x.GetRepository<SubscriptionPlan>()).Returns(planRepository.Object);
        unitOfWork.Setup(x => x.GetRepository<UserSubscription>()).Returns(subscriptionRepository.Object);

        userRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new User { Id = Guid.NewGuid(), Username = "testuser" });
        planRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((SubscriptionPlan?)null);

        var useCase = new CreateUserSubscriptionUseCase(unitOfWork.Object);
        var request = new CreateUserSubscriptionRequestDto(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());

        var ex = await Assert.ThrowsAsync<ArgumentException>(() => useCase.ExecuteAsync(request));

        Assert.Contains("Subscription plan with ID", ex.Message);
        unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        subscriptionRepository.Verify(x => x.AddAsync(It.IsAny<UserSubscription>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_WhenUserAlreadyHasActiveSubscription_ThrowsInvalidOperationException()
    {
        var unitOfWork = CreateUnitOfWorkMock();
        var userRepository = CreateRepositoryMock<User>();
        var planRepository = CreateRepositoryMock<SubscriptionPlan>();
        var subscriptionRepository = CreateRepositoryMock<UserSubscription>();

        unitOfWork.Setup(x => x.GetRepository<User>()).Returns(userRepository.Object);
        unitOfWork.Setup(x => x.GetRepository<SubscriptionPlan>()).Returns(planRepository.Object);
        unitOfWork.Setup(x => x.GetRepository<UserSubscription>()).Returns(subscriptionRepository.Object);

        userRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new User { Id = Guid.NewGuid(), Username = "testuser" });
        planRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SubscriptionPlan { Id = Guid.NewGuid(), Name = "Pro", DurationDays = 30 });
        subscriptionRepository.Setup(x => x.ExistsAsync(It.IsAny<System.Linq.Expressions.Expression<Func<UserSubscription, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var useCase = new CreateUserSubscriptionUseCase(unitOfWork.Object);
        var request = new CreateUserSubscriptionRequestDto(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());

        await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.ExecuteAsync(request));

        unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        subscriptionRepository.Verify(x => x.AddAsync(It.IsAny<UserSubscription>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_WhenValidRequest_CreatesSubscriptionAndReturnsResponse()
    {
        var unitOfWork = CreateUnitOfWorkMock();
        var userRepository = CreateRepositoryMock<User>();
        var planRepository = CreateRepositoryMock<SubscriptionPlan>();
        var subscriptionRepository = CreateRepositoryMock<UserSubscription>();

        var userId = Guid.NewGuid();
        var planId = Guid.NewGuid();
        var orderId = Guid.NewGuid();
        var startedAt = new DateTime(2026, 1, 15, 10, 30, 0, DateTimeKind.Utc);
        var user = new User { Id = userId, Username = "testuser" };
        var plan = new SubscriptionPlan { Id = planId, Name = "Pro", DurationDays = 30 };

        unitOfWork.Setup(x => x.GetRepository<User>()).Returns(userRepository.Object);
        unitOfWork.Setup(x => x.GetRepository<SubscriptionPlan>()).Returns(planRepository.Object);
        unitOfWork.Setup(x => x.GetRepository<UserSubscription>()).Returns(subscriptionRepository.Object);

        userRepository.Setup(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>())).ReturnsAsync(user);
        planRepository.Setup(x => x.GetByIdAsync(planId, It.IsAny<CancellationToken>())).ReturnsAsync(plan);
        subscriptionRepository.Setup(x => x.ExistsAsync(It.IsAny<System.Linq.Expressions.Expression<Func<UserSubscription, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        subscriptionRepository.Setup(x => x.AddAsync(It.IsAny<UserSubscription>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserSubscription entity, CancellationToken _) => entity);
        unitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var useCase = new CreateUserSubscriptionUseCase(unitOfWork.Object);
        var request = new CreateUserSubscriptionRequestDto(userId, planId, orderId, SubscriptionStatus.Active, startedAt);

        var result = await useCase.ExecuteAsync(request);

        Assert.Equal(userId, result.UserId);
        Assert.Equal(planId, result.PlanId);
        Assert.Equal(orderId, result.OrderId);
        Assert.Equal(SubscriptionStatus.Active, result.Status);
        Assert.Equal(startedAt, result.StartedAt);
        Assert.Equal(startedAt.AddDays(plan.DurationDays), result.ExpiresAt);
        Assert.Equal(user.Username, result.UserName);
        Assert.Equal(plan.Name, result.PlanName);
        subscriptionRepository.Verify(x => x.AddAsync(It.Is<UserSubscription>(s =>
            s.UserId == userId &&
            s.PlanId == planId &&
            s.OrderId == orderId &&
            s.Status == SubscriptionStatus.Active &&
            s.StartedAt == startedAt &&
            s.ExpiresAt == startedAt.AddDays(plan.DurationDays)), It.IsAny<CancellationToken>()), Times.Once);
        unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    private static Mock<IUnitOfWork> CreateUnitOfWorkMock() => new(MockBehavior.Strict);

    private static Mock<IGenericRepository<T>> CreateRepositoryMock<T>() where T : class => new(MockBehavior.Strict);
}
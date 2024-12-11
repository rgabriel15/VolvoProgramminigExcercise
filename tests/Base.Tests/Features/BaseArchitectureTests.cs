using System.Reflection;
using Base.Application.DTOs;
using Base.Application.Interfaces.Mappers;
using Base.Domain.Entities;
using Base.Infrastructure.Repositories;
using NetArchTest.Rules;
using Org.BouncyCastle.Security;
using Web.API.Controllers;

namespace Base.Tests.Features;
public abstract class BaseArchitectureTests<TEntity, TRepository, TService>
    where TEntity : BaseEntity
    where TRepository : class
    where TService : class
{
    #region Constants
    private readonly Assembly DomainAssembly = typeof(TEntity).Assembly;
    private readonly Assembly InfrastructureAssembly = typeof(TRepository).Assembly;
    private readonly Assembly ApplicationAssembly = typeof(TService).Assembly;
    private readonly Assembly PresentationAssembly = typeof(BaseController<>).Assembly;
    #endregion

    #region Constructors
    protected BaseArchitectureTests()
    {
        if (!typeof(TRepository).IsClass)
        {
            throw new InvalidParameterException($"[{nameof(TRepository)}] must be a Class.");
        }

        if (!typeof(TService).IsInterface)
        {
            throw new InvalidParameterException($"[{nameof(TService)}] must be an Interface.");
        }
    }
    #endregion

    #region Methods
    [Fact]
    public virtual void DtoShouldResideInDtosNamespace()
    {
        var list = Types
            .InAssembly(ApplicationAssembly)
            .That()
            .Inherit(typeof(BaseDto))
            .Or()
            .HaveNameEndingWith("Dto", StringComparison.InvariantCulture)
            .Should();

        var res = list
            .ResideInNamespaceEndingWith(".Application.DTOs")
            .GetResult();

        Assert.True(res.IsSuccessful);
    }

    [Fact]
    public virtual void DtoNameShouldEndsWithDto()
    {
        var list = Types
            .InAssembly(ApplicationAssembly)
            .That()
            .ResideInNamespaceEndingWith(".Application.DTOs")
            .Should();

        var res = list
            .HaveNameEndingWith("Dto", StringComparison.InvariantCulture)
            .GetResult();

        Assert.True(res.IsSuccessful);
    }

    //[Fact]
    //public virtual void DtoShouldBeSealedImmutableAndInheritBaseDto()
    //{
    //    var list = Types
    //        .InAssembly(ApplicationAssembly)
    //        .That()
    //        .ResideInNamespaceEndingWith(".Application.DTOs")
    //        .And()
    //        .AreNotGeneric()
    //        .And()
    //        .AreNotAbstract()
    //        .Should();

    //    var res = list
    //        .BeSealed()
    //        .And()
    //        .BeImmutable()
    //        .And()
    //        .Inherit(typeof(BaseDto))
    //        .GetResult();

    //    Assert.True(res.IsSuccessful);
    //}

    [Fact]
    public virtual void MapperShouldResideInMappersNamespace()
    {
        var list = Types
            .InAssembly(ApplicationAssembly)
            .That()
            .ImplementInterface(typeof(IBaseMapper<,>))
            .And()
            .AreNotInterfaces()
            .Should();

        var res = list
            .ResideInNamespaceEndingWith(".Application.Mappers")
            .GetResult();

        Assert.True(res.IsSuccessful);
    }

    [Fact]
    public virtual void MapperNameShouldEndsWithMapper()
    {
        var list = Types
            .InAssembly(ApplicationAssembly)
            .That()
            .ResideInNamespaceEndingWith(".Application.Mappers")
            .Should();

        var res = list
            .HaveNameEndingWith("Mapper", StringComparison.InvariantCulture)
            .GetResult();

        Assert.True(res.IsSuccessful);
    }

    [Fact]
    public virtual void MapperShouldBeSealed()
    {
        var list = Types
            .InAssembly(ApplicationAssembly)
            .That()
            .ResideInNamespaceEndingWith(".Application.Mappers")
            .And()
            .AreNotGeneric()
            .And()
            .AreNotAbstract()
            .Should();

        var res = list
            .BeSealed()
            .GetResult();

        Assert.True(res.IsSuccessful);
    }

    [Fact]
    public virtual void EntityShouldResideInEntitiesNamespace()
    {
        var list = Types
            .InAssembly(DomainAssembly)
            .That()
            .Inherit(typeof(BaseEntity))
            .Or()
            .HaveNameEndingWith("Entity", StringComparison.InvariantCulture)
            .Should();

        var res = list
            .ResideInNamespaceEndingWith(".Domain.Entities")
            .GetResult();

        Assert.True(res.IsSuccessful);
    }

    [Fact]
    public virtual void EntityNameShouldEndsWithEntity()
    {
        var list = Types
            .InAssembly(DomainAssembly)
            .That()
            .ResideInNamespaceEndingWith(".Domain.Entities")
            .And()
            .AreNotGeneric()
            .Should();

        var res = list
            .HaveNameEndingWith("Entity", StringComparison.InvariantCulture)
            .GetResult();

        Assert.True(res.IsSuccessful);
    }

    [Fact]
    public virtual void EntityShouldBeSealedClassAndInheritBaseEntity()
    {
        var list = Types
            .InAssembly(DomainAssembly)
            .That()
            .ResideInNamespaceEndingWith(".Domain.Entities")
            .And()
            .AreNotGeneric()
            .And()
            .AreNotAbstract()
            .Should();

        var res = list
            .BeSealed()
            .And()
            .BeClasses()
            .And()
            .Inherit(typeof(BaseEntity))
            .GetResult();

        Assert.True(res.IsSuccessful);
    }

    [Fact]
    public virtual void DomainShouldNotHaveDependencyOnApplication()
    {
        var list = Types
            .InAssembly(DomainAssembly)
            .Should();

        var res = list
            .NotHaveDependencyOn(ApplicationAssembly.GetName().Name)
            .GetResult();

        Assert.True(res.IsSuccessful);
    }

    [Fact]
    public virtual void DomainShouldNotHaveDependencyOnInfrastructure()
    {
        var list = Types
            .InAssembly(DomainAssembly)
            .Should();

        var res = list
            .NotHaveDependencyOn(InfrastructureAssembly.GetName().Name)
            .GetResult();

        Assert.True(res.IsSuccessful);
    }

    [Fact]
    public virtual void DomainShouldNotHaveDependencyOnPresentation()
    {
        var list = Types
            .InAssembly(DomainAssembly)
            .Should();

        var res = list
            .NotHaveDependencyOn(PresentationAssembly.GetName().Name)
            .GetResult();

        Assert.True(res.IsSuccessful);
    }

    [Fact]
    public virtual void RepositoryShouldResideInRepositoriesNamespace()
    {
        var list = Types
            .InAssembly(InfrastructureAssembly)
            .That()
            .DoNotHaveName("Program")
            .And()
            .DoNotHaveNameMatching(".*AnonymousType.*")
            .Should();

        var res = list
            .ResideInNamespaceEndingWith(".Infrastructure.Repositories")
            .GetResult();

        Assert.True(res.IsSuccessful);
    }

    [Fact]
    public virtual void RepositoryNameShouldEndsWithRepository()
    {
        var list = Types
            .InAssembly(InfrastructureAssembly)
            .That()
            .ResideInNamespaceEndingWith(".Infrastructure.Repositories")
            .Should();

        var res = list
            .HaveNameEndingWith("Repository", StringComparison.InvariantCulture)
            .GetResult();

        Assert.True(res.IsSuccessful);
    }

    [Fact]
    public virtual void RepositoryShouldBeSealedClassAndInheritBaseRepository()
    {
        var list = Types
            .InAssembly(InfrastructureAssembly)
            .That()
            .ResideInNamespaceEndingWith(".Infrastructure.Repositories")
            .Should();

        var res = list
            .BeSealed()
            .And()
            .BeClasses()
            .And()
            .Inherit(typeof(BaseRepository<>))
            .GetResult();

        Assert.True(res.IsSuccessful);
    }

    [Fact]
    public virtual void InfrastructureShouldHaveDependencyOnDomain()
    {
        var list = Types
            .InAssembly(InfrastructureAssembly)
            .That()
            .DoNotHaveName("Program")
            .And()
            .DoNotHaveNameMatching(".*AnonymousType.*")
            .Should();

        var res = list
            .HaveDependencyOn(DomainAssembly.GetName().Name)
            .GetResult();

        Assert.True(res.IsSuccessful);
    }

    [Fact]
    public virtual void InfrastructureShouldNotHaveDependencyOnApplication()
    {
        var list = Types
            .InAssembly(InfrastructureAssembly)
            .Should();

        var res = list
            .NotHaveDependencyOn(ApplicationAssembly.GetName().Name)
            .GetResult();

        Assert.True(res.IsSuccessful);
    }

    [Fact]
    public virtual void InfrastructureShouldNotHaveDependencyOnPresentation()
    {
        var list = Types
            .InAssembly(InfrastructureAssembly)
            .Should();

        var res = list
            .NotHaveDependencyOn(PresentationAssembly.GetName().Name)
            .GetResult();

        Assert.True(res.IsSuccessful);
    }

    [Fact]
    public virtual void ServiceShouldResideInServicesNamespace()
    {
        var list = Types
            .InAssembly(ApplicationAssembly)
            .That()
            .ImplementInterface(typeof(TService))
            .Or()
            .HaveNameEndingWith("Service", StringComparison.InvariantCulture)
            .And()
            .AreNotInterfaces()
            .Should();

        var res = list
            .ResideInNamespaceEndingWith(".Application.Services")
            .GetResult();

        Assert.True(res.IsSuccessful);
    }

    [Fact]
    public virtual void ServiceNameShouldEndsWithService()
    {
        var list = Types
            .InAssembly(ApplicationAssembly)
            .That()
            .ResideInNamespaceEndingWith(".Application.Services")
            .Should();

        var res = list
            .HaveNameEndingWith("Service", StringComparison.InvariantCulture)
            .GetResult();

        Assert.True(res.IsSuccessful);
    }

    [Fact]
    public virtual void ServiceShouldBeSealedClass()
    {
        var list = Types
            .InAssembly(ApplicationAssembly)
            .That()
            .ResideInNamespaceEndingWith(".Application.Services")
            .Should();

        var res = list
            .BeSealed()
            .And()
            .BeClasses()
            .GetResult();

        Assert.True(res.IsSuccessful);
    }

    [Fact]
    public virtual void ApplicationShouldHaveDependencyOnDomain()
    {
        var list = Types
            .InAssembly(ApplicationAssembly)
            .That()
            .ResideInNamespaceEndingWith(".Application.Services")
            .Should();

        var res = list
            .HaveDependencyOn(DomainAssembly.GetName().Name)
            .GetResult();

        Assert.True(res.IsSuccessful);
    }

    [Fact]
    public virtual void ApplicationShouldNotHaveDependencyOnInfrastructure()
    {
        var list = Types
            .InAssembly(ApplicationAssembly)
            .Should();

        var res = list
            .NotHaveDependencyOn(InfrastructureAssembly.GetName().Name)
            .GetResult();

        Assert.True(res.IsSuccessful);
    }

    [Fact]
    public virtual void ApplicationShouldNotHaveDependencyOnPresentation()
    {
        var list = Types
            .InAssembly(ApplicationAssembly)
            .Should();

        var res = list
            .NotHaveDependencyOn(PresentationAssembly.GetName().Name)
            .GetResult();

        Assert.True(res.IsSuccessful);
    }
    #endregion
}
